using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour
{
    public float flipSpeed = 100f;
    public float maxAngle = 15f;
    public float smooth = .1f;
    public float circleMoveStep = .01f;
    public float circlingAngle = 20f;
    public float circlingOffset = .03f;
    // -------------private--------------------
    [SerializeField] private PanState panState;
    [SerializeField] private float yVelocity = 0f;
    private static Vector3 originalPos = new Vector3(0, 2f, -4.5f);

    // -------------reference--------------------
    [SerializeField] private GameSO _gameSO;
    private Rigidbody rb;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        // rb.centerOfMass = new Vector3(0, 0.7f, -5f); //手柄上，影响颠勺时的旋转中心，sadly不起作用，最后只能用blender改pivot
        resetPosition();
    }

    /// <summary>
    /// 注册监听输入事件
    /// </summary>
    void OnEnable()
    {
        InputManager.confirmEvent += OnConfirm;
        InputManager.gyroEvent += OnGyro;
    }

    /// <summary>
    /// 取消注册监听
    /// </summary>
    void OnDisable()
    {
        InputManager.confirmEvent -= OnConfirm;
        InputManager.gyroEvent -= OnGyro;
    }

    // void Start()
    // {
    //     OnConfirm();
    // }

    private void Update()
    {
        if (_gameSO.gameState != GameState.Playing)
        {
            resetPosition();
            return;
        }
        if (panState == PanState.Flip)
        {
            resetPosition();
            panState = PanState.Flipping;
        }
        else if (panState == PanState.Flipping)
        {
            if (rb.rotation.eulerAngles.x <= (360 - maxAngle + 0.5f) && rb.rotation.eulerAngles.x > 0.5f)
            {
                yVelocity = 0f;
                panState = PanState.UnFlipping;
                return;
            }
            pitchPan(360 - maxAngle);
        }
        else if (panState == PanState.UnFlipping)
        {
            if (rb.rotation.eulerAngles.x <= 0.5f || rb.rotation.eulerAngles.x >= (360f - 0.5f))
            {
                yVelocity = 0f;
                rb.MoveRotation(Quaternion.identity);
                panState = PanState.Circle;
                return;
            }
            pitchPan(0, true);
        }
        else if (panState == PanState.Circle)
        {
            var targetPos = originalPos + Vector3.back * circlingOffset;
            if (moveToTargetPos(targetPos))
            {
                panState = PanState.Circling;
            }
        }
        else if (panState == PanState.Circling)
        {
            rotateRigidBodyAroundPointPosOnlyBy(rb, originalPos, Vector3.up, circlingAngle);
        }
    }

    void resetPosition()
    {
        rb.MoveRotation(Quaternion.identity);
        rb.MovePosition(originalPos);
    }

    bool moveToTargetPos(Vector3 targetPos)
    {
        if (Vector3.Distance(targetPos, transform.position) < circleMoveStep)
        {
            rb.MovePosition(targetPos);
            return true;
        }
        else
        {
            rb.MovePosition(Vector3.MoveTowards(transform.position, targetPos, circleMoveStep));
        }
        return false;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("BEEF"))
        {
            Debug.Log("[pan]OnCollisionEnter" + other.relativeVelocity + ":" + transform.up);
            if (Vector3.Dot(other.relativeVelocity, transform.up) < -0.5f)
            {
                // to catch the beef;
                if (panState == PanState.Flipping)
                {
                    panState = PanState.UnFlipping;
                }
            }
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("BEEF"))
        {
            if (panState == PanState.Circle || panState == PanState.Circling)
            {
                // stop cirlcling the pan;
                panState = PanState.Tilting;
            }
        }
    }

    private void pitchPan(float angle, bool reverse = false)
    {
        float smoothAngle = Mathf.SmoothDampAngle(rb.rotation.eulerAngles.x, angle, ref yVelocity, smooth, flipSpeed);
        rb.MoveRotation(Quaternion.AngleAxis(smoothAngle, Vector3.right));
    }

    Quaternion rotateRigidBodyAroundPointPosOnlyBy(Rigidbody rb, Vector3 origin, Vector3 axis, float angle)
    {
        Quaternion q = Quaternion.AngleAxis(angle, axis);
        rb.MovePosition(q * (rb.transform.position - origin) + origin);
        return q;
    }

    private void OnConfirm()
    {
        panState = PanState.Flip;
    }

    private void OnGyro(Gyroscope gyro)
    {
        if (_gameSO.gameState != GameState.Playing) return;

        if (panState == PanState.Flip || panState == PanState.Flipping || panState == PanState.UnFlipping) return;

        // 静止状态下，Z=-1，在判断Z<-2时，颠勺？
        // if (gyro.userAcceleration.z > 1 && gyro.rotationRate.x > 1) // 手机横向翻转角度变化delta，右边向上>0
        // {
        //     panState = PanState.Flip;
        //     return;
        // }

        if ((Mathf.Abs(gyro.gravity.x) <= 0.1 && Mathf.Abs(gyro.gravity.y) <= 0.1))
        {
            if (panState != PanState.Circle && panState != PanState.Circling)
            {
                panState = PanState.Circle;
            }
        }
        else
        {
            Debug.Log("[Pan]OnGyro1:" + gyro.gravity.x + " ," + gyro.gravity.y + "," + gyro.gravity.z);
            panState = PanState.Tilting;
            if (!moveToTargetPos(originalPos))
            {
                Debug.Log("[Pan]Tilting & moving");
            }
        }

        if (gyro.gravity.magnitude == 0) return;

        // // 静止状态下 x,y=0 ，用x y来决定是否锅的倾斜，最大倾斜角
        // use gravity instead of acceleration to 克服抖动

        rb.MoveRotation(Quaternion.Euler(
            calcAngle(gyro.gravity.y, -gyro.gravity.z, 10),
            0,
            calcAngle(-gyro.gravity.x, 1)
        ));

    }
    float calcAngle(float x, float y, float cut = 15f)
    {
        return Mathf.Clamp(Mathf.Rad2Deg * Mathf.Atan2(x, y), -cut, cut);
    }

    // -------------public--------------------
    public bool IsPanCircling()
    {
        return panState == PanState.Circling;
    }

}

enum PanState
{
    Circle,
    Circling,
    Flip,
    Flipping,
    UnFlipping,
    Tilting,
}

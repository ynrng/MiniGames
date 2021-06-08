using System;
using UnityEngine;
using MiniGames.GameControl;
using Unity.Mathematics;

public class Pan : MonoBehaviour
{
    // -------------private--------------------
    [SerializeField] private float flipSpeed = 100f;
    [SerializeField] private float flipMaxAngle = 15f;
    [SerializeField] private float smooth = .1f;
    [SerializeField] private float circleMoveStep = .01f;
    [SerializeField] private float circlingAngle = 20f;
    [SerializeField] private float circlingOffset = .03f;
    [SerializeField] private PanState panState;
    private float flipVelocity = 0f;
    private bool beefOnSurface = false;

    // -------------reference--------------------
    [SerializeField] private GameSO _gameSO;
    private Rigidbody rb;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        // rb.centerOfMass = new Vector3(0, 0.7f, -5f); //手柄上，影响颠勺时的旋转中心，sadly不起作用，最后只能用blender改pivot
        resetPosition(withAnimation: false, useRest: true);
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
            resetPosition(withAnimation: true, useRest: _gameSO.gameState != GameState.Start);
            return;
        }
        switch (panState)
        {
            case PanState.Flip:
                if (resetPosition(withAnimation: true, useRest: false))
                {
                    panState = PanState.Flipping;
                }
                break;
            case PanState.Flipping:
                if (rb.rotation.eulerAngles.x <= (360 - flipMaxAngle + 0.5f) && rb.rotation.eulerAngles.x > 0.5f)
                {
                    flipVelocity = 0f;
                    panState = PanState.UnFlipping;
                    return;
                }
                pitchPan(360 - flipMaxAngle);
                break;
            case PanState.UnFlipping:
                if (rb.rotation.eulerAngles.x <= 0.5f || rb.rotation.eulerAngles.x >= (360f - 0.5f))
                {
                    flipVelocity = 0f;
                    rotateToTargetRot(Quaternion.identity);//todo  calc target rot
                    panState = beefOnSurface ? PanState.Circle : PanState.Tilting;
                    return;
                }
                pitchPan(0, true);
                break;
            case PanState.Circle:
                var targetPos = Constants.panPos + Vector3.back * circlingOffset;
                if (moveToTargetPos(targetPos))
                {
                    panState = PanState.Circling;
                }
                break;
            case PanState.Circling:
                rotateRigidBodyAroundPointPosOnlyBy(rb, Constants.panPos, Vector3.up, circlingAngle);
                break;
            default:
                if (!InputManager.GyroEnabled())
                {
                    doCircling();
                }
                break;
        }
    }

    bool resetPosition(bool withAnimation, bool useRest)
    {
        if (!withAnimation)
        {
            rb.MoveRotation(Quaternion.identity);
            rb.MovePosition(useRest ? Constants.panRestPos : Constants.panPos);
            return true;
        }
        return moveToTargetPos(useRest ? Constants.panRestPos : Constants.panPos) && rotateToTargetRot(Quaternion.identity);
    }

    bool rotateToTargetRot(Quaternion targetRot, float step = 5f)
    {
        if (Quaternion.Angle(targetRot, transform.rotation) < step)
        {
            rb.MoveRotation(targetRot);
            return true;
        }
        else
        {
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRot, step));
        }
        return false;
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
            beefOnSurface = true;
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
        Debug.Log("exit[pan]" + other.gameObject.tag + " " + panState);
        if (other.gameObject.CompareTag("BEEF"))
        {
            beefOnSurface = false;
            if (panState == PanState.Circle || panState == PanState.Circling)
            {
                // stop cirlcling the pan;
                panState = PanState.Tilting;
            }
        }
    }

    private void pitchPan(float angle, bool reverse = false)
    {
        float smoothAngle = Mathf.SmoothDampAngle(rb.rotation.eulerAngles.x, angle, ref flipVelocity, smooth, flipSpeed);
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

    void doCircling()
    {
        if (panState != PanState.Circle && panState != PanState.Circling && beefOnSurface)
        {
            panState = PanState.Circle;
        }
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
            doCircling();
        }
        else
        {
            Debug.Log("[Pan]OnGyro1:" + gyro.gravity.x + " ," + gyro.gravity.y + "," + gyro.gravity.z);
            panState = PanState.Tilting;
            if (!moveToTargetPos(Constants.panPos))
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
    public bool IsCircling()
    {
        return panState == PanState.Circling;
    }

    public Vector3 Center()
    {
        return transform.position + Constants.panPivotCenter;
    }

}

enum PanState
{
    Circle,
    Circling,
    Flip,
    Flipping,
    UnFlipping,
    Tilting
}

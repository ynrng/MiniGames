using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Pan : MonoBehaviour
{
    public float velocityMultiplier = 1f;
    public float flipSpeed = 0.001f;
    public float maxAngle = 5f;
    public float smooth = 1f;
    public float circleMoveStep = .5f;
    public float circlingAngle = 20f;
    public float circlingOffset = .03f;
    public GameObject messCenter;
    public Vector3 messCenterPos;
    // -------------private--------------------
    [SerializeField] private GameSO _gameSO;
    // [SerializeField] private InputManager _inputManagerSO;

    private Rigidbody rb;
    [SerializeField] private PanState panState;
    private Gyroscope flippingGyro;
    [SerializeField] private float yVelocity = 0f;
    private static Vector3 originalPos = Vector3.up;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        // rb.centerOfMass = new Vector3(0, 0.7f, -5f); //手柄上，影响颠勺时的旋转中心
        transform.position = originalPos;
        messCenterPos = messCenter.transform.position;
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

    void Start()
    {
        OnConfirm();//todo delete
    }

    private void Update()
    {
        if (panState == PanState.Flip)
        {
            // if ()
            // {
            resetPosition();
            panState = PanState.Flipping;
            // }
        }
        else if (panState == PanState.Flipping)
        {
            Debug.Log("Flipping" + rb.rotation.eulerAngles.x + " <= " + (360 - maxAngle) + ":" + yVelocity);
            if (rb.rotation.eulerAngles.x < 360 - maxAngle && rb.rotation.eulerAngles.x > 0)
            {
                yVelocity = 0f;
                panState = PanState.UnFlipping;
                return;
            }
            pitchPan(maxAngle);
        }
        else if (panState == PanState.UnFlipping)
        {
            Debug.Log("UnFlipping" + rb.rotation.eulerAngles.x + ":" + yVelocity);

            if (rb.rotation.eulerAngles.x <= 0 || rb.rotation.eulerAngles.x >= 360)
            {
                yVelocity = 0f;
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
        // if (Quaternion.Distance(targetPos, transform.position) < circleMoveStep)
        // {
        //     rb.MovePosition(targetPos);
        //     return true;
        // }
        // else
        // {
        //     rb.MovePosition(Vector3.MoveTowards(transform.position, targetPos, circleMoveStep));
        // }
        // return false;
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
            // to catch the beef;
            // if (panState == PanState.Flipping)
            // {
            //     panState = PanState.UnFlipping;
            // }
        }
    }

    private void pitchPan(float angle, bool reverse = false)
    {

        float smoothAngle = Mathf.SmoothDampAngle(rb.rotation.eulerAngles.x, 360 - angle, ref yVelocity, smooth, flipSpeed);
        Debug.Log("pitch:" + rb.rotation.eulerAngles.x + ":" + smoothAngle);
        if (yVelocity > 0)
        {
            if (smoothAngle > 180)
            {
                smoothAngle = Mathf.Clamp(smoothAngle, 360 - maxAngle, 360);
            }
            else
            {
                smoothAngle = Mathf.Clamp01(smoothAngle);
            }
        }
        rotateRigidBodyAroundPointBy(rb, messCenterPos, Vector3.right * (reverse ? -1 : 1), smoothAngle);
    }

    void rotateRigidBodyAroundPointBy(Rigidbody rb, Vector3 origin, Vector3 axis, float angle)
    {
        Quaternion q = rotateRigidBodyAroundPointPosOnlyBy(rb, origin, axis, angle);
        rb.MoveRotation(rb.transform.rotation * q);
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
        if (panState == PanState.Flip || panState == PanState.Flipping || panState == PanState.UnFlipping) return;

        // gameObject.transform.rotation = InputManager.GyroToUnity(gyro.attitude);
        // rb.MoveRotation(InputManager.GyroToUnity(gyro.attitude));
        // if (_gameSO.gameState == GameState.Playing)//todo uncomment
        // {
        // 静止状态下，Z=-1，在判断Z<-2时，颠勺？// todo 判断一下rotate的增量
        if (gyro.userAcceleration.z > 1 && gyro.rotationRate.x > 1) // 手机横向翻转角度变化delta，右边向上>0
        {
            // flippingGyro = gyro;
            // maxAngle = Mathf.Clamp(gyro.rotationRate.x, 1f, 5f);

            /// todo use gyro to decide the maxAngle
            /// and to make sure beef stay in air while filping pan
            panState = PanState.Flip;
            return;
        }

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
        Debug.Log("[Pan]OnGyro2:" + gyro.gravity.x + " ," + gyro.gravity.y + "," + gyro.gravity.z);

        // // 静止状态下 x,y=0 ，用x y来决定是否锅的倾斜，最大倾斜角
        // use gravity instead of acceleration to 克服抖动

        rb.MoveRotation(Quaternion.Euler(
            calcAngle(gyro.gravity.y, -gyro.gravity.z),
            0,// calcAngle(1, gyro.rotationRate.z, 10f),
            calcAngle(-gyro.gravity.x, 1)
        ));

        // var angleTilt = calcAngle(gyro.gravity.y, -gyro.gravity.z);
        // pitchPan(Mathf.Abs(angleTilt), angleTilt >= 0);
        // }

        // // }

    }
    float calcAngle(float x, float y, float varient = 20f)
    {
        return Mathf.Clamp(Mathf.Rad2Deg * Mathf.Atan2(x, y), -varient, varient);
    }

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

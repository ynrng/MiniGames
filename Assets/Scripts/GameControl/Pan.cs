using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour
{
    public float velocityMultiplier = 1f;
    public float flipSpeed = 0.001f;
    public float maxAngle = 5f;
    // -------------private--------------------
    [SerializeField] private GameSO _gameSO;
    // [SerializeField] private InputManager _inputManagerSO;

    private Rigidbody rb;
    [SerializeField] private PanState panState;
    private Gyroscope flippingGyro;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, 0.7f, -5f); //手柄上，影响颠勺时的旋转中心
    }

    /// <summary>
    /// 注册监听输入事件
    /// </summary>
    void OnEnable()
    {
        InputManager.confirmEvent += OnMove;
        InputManager.gyroEvent += OnGyro;

    }

    /// <summary>
    /// 取消注册监听
    /// </summary>
    void OnDisable()
    {
        InputManager.confirmEvent -= OnMove;
        InputManager.gyroEvent -= OnGyro;

    }

    void Start()
    {
        panState = PanState.Flipping;
    }

    public float smooth = 1f;
    public float yVelocity = 0f;
    private void Update()
    {
        if (panState == PanState.Flipping)
        {
            Debug.Log("Flipping" + rb.rotation.eulerAngles.x + " <= " + (360 - maxAngle) + ":" + yVelocity);
            if (rb.rotation.eulerAngles.x < 360 - maxAngle && rb.rotation.eulerAngles.x > 0)
            {
                yVelocity = 0f;
                panState = PanState.UnFlipping;
            }
            pitchPan(maxAngle);

        }
        else if (panState == PanState.UnFlipping)
        {
            Debug.Log("UnFlipping" + rb.rotation.eulerAngles.x + ":" + yVelocity);

            if (rb.rotation.eulerAngles.x <= 0 || rb.rotation.eulerAngles.x >= 360)
            {
                // panState = PanState.Circle;
                yVelocity = 0f;
                panState = PanState.Flipping;
            }
            pitchPan(0, true);
        }
        // else
        // {
        //     panState = PanState.Circle;
        // }
    }

    private void pitchPan(float angle, bool reverse = false)
    {

        float smoothAngle = Mathf.SmoothDampAngle(rb.rotation.eulerAngles.x, 360 - angle, ref yVelocity, smooth, flipSpeed);
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

        Utils.rotateRigidBodyAroundPointBy(rb, new Vector3(0, 0.7f, -5f), Vector3.right * (reverse ? -1 : 1), smoothAngle);
    }

    private void OnMove()
    {

        // Utils.rotateRigidBodyAroundPointBy(rb, new Vector3(0, 0.7f, -5f), Vector3.right, -15);
        // // // 静止状态下 x,y=0 ，用x y来决定是否锅的倾斜，最大倾斜角
        // // todo use lerp and curve 怎样克服抖动
        // rb.MoveRotation(Quaternion.Euler(
        //     calcAngle(-coord.y / coord.z, 30f),
        //     // calcAngle(coord.y, 30f)
        //     0, // todo
        //     calcAngle(coord.x / coord.z, 30f)
        //     // Mathf.Clamp(-coord.y * 180, -30f, 30f),
        //     //     Mathf.Clamp(coord.z * 180, -15f, 15f)
        //     ));

        // Debug.Log("[Pan]OnMove:" + coord + ";" + gameObject.transform.rotation);

        // // }

    }

    float calcAngle(float x, float y, float varient = 20f)
    {

        return Mathf.Clamp(Mathf.Rad2Deg * Mathf.Atan2(x, y), -varient, varient);

    }

    private void OnGyro(Gyroscope gyro)
    {
        if (panState == PanState.Flipping) return;

        // gameObject.transform.rotation = InputManager.GyroToUnity(gyro.attitude);
        // rb.MoveRotation(InputManager.GyroToUnity(gyro.attitude));
        // if (_gameSO.gameState == GameState.Playing)//todo uncomment
        // {
        // 静止状态下，Z=-1，在判断Z<-2时，颠勺？// todo 判断一下rotate的增量
        if (gyro.userAcceleration.z > 1 && gyro.rotationRate.x > 1) // 手机横向翻转角度变化delta，右边向上>0
        {
            flippingGyro = gyro;
            panState = PanState.Flipping;
            // myArrow.transform.forward =
            // Vector3.Slerp(myArrow.transform.forward, myArrow.rigidbody.velocity.normalized, Time.deltaTime);
            // rb.MoveRotation(Quaternion.Slerp(rb.rotation, gyro.rotationRate.x, Time.deltaTime));

            // rb.centerOfMass = new Vector3(0, -0.7f, 5f); //手柄上，影响颠勺时的旋转中心

        }

        // // 静止状态下 x,y=0 ，用x y来决定是否锅的倾斜，最大倾斜角
        // todo use lerp and curve 怎样克服抖动
        rb.MoveRotation(Quaternion.Euler(
            calcAngle(gyro.gravity.y, -gyro.gravity.z),
            0,// calcAngle(1, gyro.rotationRate.z, 10f),
            calcAngle(gyro.gravity.x, 1)
            ));
        // }

        // Debug.Log("[Pan]OnGyro:" + gyro.gravity);

        // // }

    }

}

enum PanState
{
    Circle,
    Flipping,
    UnFlipping,
}

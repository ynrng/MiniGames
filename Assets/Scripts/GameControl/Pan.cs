using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour
{
    public float velocityMultiplier = 1f;
    // -------------private--------------------
    [SerializeField] private GameSO _gameSO;
    // [SerializeField] private InputManager _inputManagerSO;

    private Rigidbody rb;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// 注册监听输入事件
    /// </summary>
    void OnEnable()
    {
        InputManager.moveEvent += OnMove;
        InputManager.gyroEvent += OnGyro;

    }

    /// <summary>
    /// 取消注册监听
    /// </summary>
    void OnDisable()
    {
        InputManager.moveEvent -= OnMove;
        InputManager.gyroEvent -= OnGyro;

    }

    private void OnRotate(Vector3 coord)
    {
        // 其实是旋转的矢量
    }

    private void OnMove(Vector3 coord)
    {

        // // 静止状态下 x,y=0 ，用x y来决定是否锅的倾斜，最大倾斜角
        // todo use lerp and curve 怎样克服抖动
        rb.MoveRotation(Quaternion.Euler(
            calcAngle(-coord.y / coord.z, 30f),
            // calcAngle(coord.y, 30f)
            0, // todo
            calcAngle(coord.x / coord.z, 30f)
            // Mathf.Clamp(-coord.y * 180, -30f, 30f),
            //     Mathf.Clamp(coord.z * 180, -15f, 15f)
            ));

        Debug.Log("[Pan]OnMove:" + coord + ";" + gameObject.transform.rotation);

        // }

    }

    float calcAngle(float x, float varient)
    {

        return Mathf.Clamp(Mathf.Rad2Deg * Mathf.Atan(x), -20, 20);

    }

    private void OnGyro(Gyroscope gyro)
    {

                //     // gameObject.transform.rotation = InputManager.GyroToUnity(gyro.attitude);
                //     // rb.MoveRotation(InputManager.GyroToUnity(gyro.attitude));
                //     if (_gameSO.gameState == GameState.Playing)
                //     {
                //         // 静止状态下，Z=-1，在判断Z<-2时，颠勺？// todo 判断一下rotate的增量
                //         if (gyro.userAcceleration.x > 1 && beef) // 手机横向翻转角度变化delta，右边向上>0
                //         {
                //             beef.addVelociy(Mathf.Clamp(gyro.userAcceleration.x, 1, 3) * velocityMultiplier);
                //         }
                //         // 静止状态下 x,y=0 ，用x y来决定是否锅的倾斜，最大倾斜角
                //         // gameObject.transform.rotation = InputManager.GyroToUnity(gyro.attitude);

                //         Debug.Log("[Pan]OnGyro:");

                //     }

    }

}

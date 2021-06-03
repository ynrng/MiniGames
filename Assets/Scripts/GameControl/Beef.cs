using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Beef : MonoBehaviour
{
    // -------------public--------------------

    [Header("red")]
    public Vector3 rightCoord;

    [Header("green")]
    public Vector3 upCoord;

    [Header("blue")]
    public Vector3 forwardCoord;

    // -------------private--------------------
    [SerializeField] private GameSO _gameSO;
    // [SerializeField] private InputManagerSO _inputManagerSO;
    private Rigidbody rb;
    private bool hasLanded = false;
    // private Vector3 panUp = Vector3.up;
    private int faceUp = -1; //-1表示未接触或处在翻滚中
    private const float faceUpThreshold = 0.9f;
    private const float moveTipThreshold = 5;//s
    private DateTime noFaceUpFromTime;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (_gameSO.gameState == GameState.Playing)
        {
            // if (!rb.useGravity)
            // {
            rb.useGravity = true;
            // }

            if (_gameSO.timeSpent >= 0)
            {
                // 检查是否所有面都熟了
                // 检查是否超过时间限制
                if (!_gameSO.CheckIsFaceAllDone() && !_gameSO.CheckIsTimeOver())
                {
                    // 检查当前是哪一面朝上，相对锅
                    if (faceUp > -1)
                    {
                        // todo  移除提示"Move your pan!"
                        noFaceUpFromTime = default;
                        // 判断当前面是否已经熟了（上下位置对换，无关紧要
                        if (_gameSO.CheckIsFaceRaw(faceUp))
                        {
                            // 如果没有熟，开始当前面的倒计时
                            _gameSO.faceTimeLeft[faceUp] -= Time.deltaTime;
                            // 判断当前面倒计时是否>0，如果倒计时结束，那么显示提示/更改当前面颜色
                            if (_gameSO.faceTimeLeft[faceUp] <= 0)
                            {
                                // todo ui & vibrate
                                Debug.Log("[TODO]Face " + faceUp + " done!");
                                _gameSO.faceTimeLeft[faceUp] = 0;
                                _gameSO.faceUndone.Remove(faceUp);
                            }
                        }

                    }
                    else
                    {
                        // 如果没有绝对朝下面，说明在锅边缘，提示油温过低，移动锅让牛排居中
                        if (noFaceUpFromTime == default)
                        {
                            noFaceUpFromTime = DateTime.Now;
                        }
                        else if (DateTime.Now.Subtract(noFaceUpFromTime).TotalSeconds >= moveTipThreshold)
                        {
                            // todo ui
                            // 计时 超过时长 提示"Move your pan!"
                            Debug.Log("[TODO]Move your pan!");
                        }
                    }
                }

            }
        }

    }

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun
    /// touching another rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    private void OnCollisionEnter(Collision other)
    {

        if (_gameSO.gameState == GameState.Playing && other.gameObject.CompareTag("PAN"))
        {
            // 开始煎牛排的总计时
            if (_gameSO.timeSpent == -1)
            {
                _gameSO.timeStart = DateTime.Now;
                _gameSO.timeSpent = 0;
            }

        }

        hasLanded = true;
    }

    /// <summary>
    /// OnCollisionStay is called once per frame for every collider/rigidbody
    /// that is touching rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    private void OnCollisionStay(Collision other)
    {

        Debug.Log("OnCollisionStay");
        upCoord = gameObject.transform.up;
        rightCoord = gameObject.transform.right;
        forwardCoord = gameObject.transform.forward;

        if (other.gameObject.CompareTag("PAN"))
        {
            // 判断向上的面，因为只有在这里才会移动位置
            faceUp = GetCurrentFaceUp(other.gameObject.transform.up);
        }

    }

    private void OnCollisionExit(Collision other)
    {
        Debug.Log("OnCollisionExit");
        hasLanded = false;
        faceUp = -1;
    }

    private int GetCurrentFaceUp(Vector3 up)
    {
        if (Vector3.Dot(up, upCoord) > faceUpThreshold)
        {
            return 0;
        }
        if (Vector3.Dot(up, upCoord) < -faceUpThreshold)
        {
            return 5;
        }
        if (Vector3.Dot(up, rightCoord) > faceUpThreshold)
        {
            return 1;
        }
        if (Vector3.Dot(up, rightCoord) < -faceUpThreshold)
        {
            return 4;
        }
        if (Vector3.Dot(up, forwardCoord) > faceUpThreshold)
        {
            return 2;
        }
        if (Vector3.Dot(up, forwardCoord) < -faceUpThreshold)
        {
            return 3;
        }
        return -1;

    }

    // -------------public--------------------
    public void addVelociy(float force)
    {
        // gameObject.transform.rotation=Quaternion.Euler()
        rb.AddForce(upCoord * force, ForceMode.Impulse);
    }

}
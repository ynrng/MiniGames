using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MiniGames.GameControl;
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

    // private Vector3 panUp = Vector3.up;
    private int faceUp = -1; //-1表示未接触或处在翻滚中
    private const float faceUpThreshold = 0.9f;
    private const float moveTipThreshold = 5;//s
    private DateTime noFaceUpFromTime;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        _gameSO.onGameStateChange += onGameChange;
    }
    private void OnDestroy()
    {
        _gameSO.onGameStateChange -= onGameChange;

    }

    // Update is called once per frame
    private void Update()
    {
        if (_gameSO.gameState == GameState.Playing)
        {
            // if (!rb.useGravity)
            // {
            // rb.useGravity = true;
            // }

            GameScore();
            CheckPosY();
            CheckVelocity();
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
        faceUp = -1;
    }

    private void onGameChange(GameState previous, GameState next)
    {
        switch (next)
        {
            case GameState.Start:
                rb.useGravity = false;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                gameObject.transform.position = Constants.PosBeef;
                gameObject.transform.rotation = Quaternion.identity;
                meshRenderer.enabled = true;
                break;
            case GameState.Playing:
                rb.useGravity = true;
                break;
            case GameState.Win:
            // meshRenderer.enabled = false;
            // rb.useGravity = false;
            // rb.velocity = Vector3.zero;
            // break;
            case GameState.Lose:
                meshRenderer.enabled = false;
                rb.useGravity = false;
                rb.velocity = Vector3.zero;
                break;

            default:
                break;
        }
    }

    private void GameScore()
    {
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

    private void CheckPosY()
    {
        if (gameObject.transform.position.y < 0)
        {
            _gameSO.UpdateGameState(GameState.Lose);
        }
    }

    private void CheckVelocity()
    {
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, 10f);
    }

    private int GetCurrentFaceUp(Vector3 up)
    {
        if (Vector3.Dot(up, upCoord) > faceUpThreshold)
        {
            _gameSO.faceCurrent = 0;
            return 0;
        }
        if (Vector3.Dot(up, upCoord) < -faceUpThreshold)
        {
            _gameSO.faceCurrent = 5;
            return 5;
        }
        if (Vector3.Dot(up, rightCoord) > faceUpThreshold)
        {
            _gameSO.faceCurrent = 1;
            return 1;
        }
        if (Vector3.Dot(up, rightCoord) < -faceUpThreshold)
        {
            _gameSO.faceCurrent = 4;
            return 4;
        }
        if (Vector3.Dot(up, forwardCoord) > faceUpThreshold)
        {
            _gameSO.faceCurrent = 2;
            return 2;
        }
        if (Vector3.Dot(up, forwardCoord) < -faceUpThreshold)
        {
            _gameSO.faceCurrent = 3;
            return 3;
        }
        _gameSO.faceCurrent = -1;
        return -1;

    }

    // -------------public--------------------
    public void addVelociy(float force)
    {
        // gameObject.transform.rotation=Quaternion.Euler()
        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
        // rb.AddTorque()

    }

}
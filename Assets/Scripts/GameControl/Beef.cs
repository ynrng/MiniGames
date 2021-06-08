using System;
using UnityEngine;
using MiniGames.GameControl;

public class Beef : MonoBehaviour
{
    // -------------public--------------------

    [Header("red")]
    public Vector3 rightCoord;

    [Header("green")]
    public Vector3 upCoord;

    [Header("blue")]
    public Vector3 forwardCoord;
    public Pan _pan;

    // -------------private--------------------
    [SerializeField] private GameSO _gameSO;

    private Rigidbody rb;
    private MeshRenderer meshRenderer;
    private DrawCube cube;
    // private Vector3 panUp = Vector3.up;
    private int faceUp = -1; //-1表示未接触或处在翻滚中
    private const float faceUpThreshold = 0.9f;
    private const float moveTipThreshold = 5;//s
    private DateTime noFaceUpFromTime;

    public float smoothPollStep = .5f;
    public float rotateSelfRate = 10f;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        cube = gameObject.GetComponent<DrawCube>();
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
            CheckPosY(); // if on pan
            CheckVelocity(); // ?
            GameScore(); // if win or lose
            RotateWithPan();
        }

    }
    void RotateWithPan()
    {
        if (_pan && _pan.IsPanCircling())
        {
            // circle beef with pan && // todo pull to center
            rb.MovePosition(Vector3.MoveTowards(transform.position, _pan.transform.position, smoothPollStep));
            rb.MoveRotation(Quaternion.AngleAxis(rotateSelfRate, Vector3.up));
        }
    }

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

    private void OnCollisionStay(Collision other)
    {
        upCoord = gameObject.transform.up;
        rightCoord = gameObject.transform.right;
        forwardCoord = gameObject.transform.forward;

        if (other.gameObject.CompareTag("PAN"))
        {
            // 判断向上的面，因为只有在这里才会移动位置
            faceUp = GetCurrentFaceUp(other.gameObject.transform.up);
            _gameSO.faceCurrent = faceUp; // Debug Only
        }

    }

    private void OnCollisionExit(Collision other)
    {
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
                cube.ResetFace();
                meshRenderer.enabled = true;
                break;
            case GameState.Playing:
                rb.useGravity = true;
                break;
            case GameState.Win:
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
                        // 判断当前面倒计时是否>0，如果倒计时结束，那么显示提示/更改当前面颜色
                        if (_gameSO.SetFaceTimeLeft(faceUp, -Time.deltaTime) <= 0)
                        {
                            // todo ui & vibrate
                            Debug.Log("[TODO]Face " + faceUp + " done!");
                            cube.UpdateFace(faceUp, true);
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
        // order drew in Assets/Scripts/GameControl/DrawCylinder.cs
        if (Vector3.Dot(up, upCoord) > faceUpThreshold)
        {
            return 4;
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
            return 3;
        }
        if (Vector3.Dot(up, forwardCoord) > faceUpThreshold)
        {
            return 2;
        }
        if (Vector3.Dot(up, forwardCoord) < -faceUpThreshold)
        {
            return 0;
        }
        return -1;

    }

    // -------------public--------------------
    public void addVelociy(float force)
    {
        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
    }

}
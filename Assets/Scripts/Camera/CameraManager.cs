
using UnityEngine;
using UnityEngine.InputSystem;
using Gyroscope = UnityEngine.InputSystem.Gyroscope;

public class CameraManager : MonoBehaviour
{

    [SerializeField] private GameSO _gameSO;// = default;
    [SerializeField] private InputManagerSO _inputManagerSO;

    Vector3 acceleration1;
    float acceleration2;
    Vector3 acceleration3;
    Quaternion acceleration4;
    Quaternion acceleration5;

    // Start is called before the first frame update

    private void OnGUI()
    {

        GUIStyle myButtonStyle = new GUIStyle(GUI.skin.label);
        myButtonStyle.fontSize = 50;

        // Set color for selected and unselected buttons
        myButtonStyle.normal.textColor = Color.red;
        myButtonStyle.alignment = TextAnchor.UpperRight;

        string txt2 = "rotate :" + (Gyroscope.current != null) + " enable:" + (Gyroscope.current != null && Gyroscope.current.enabled) + ":" + acceleration1.ToString();
        GUI.Label(new Rect(0, 10, Screen.width, 60), txt2, myButtonStyle);

        string txt3 = "move :" + (Accelerometer.current != null) + " enable:" + (Accelerometer.current != null && Accelerometer.current.enabled) + ":" + acceleration3.ToString();
        GUI.Label(new Rect(0, 60, Screen.width, 110), txt3, myButtonStyle);

        string txt4 = "atti :" + (AttitudeSensor.current != null) + " enable:" + (AttitudeSensor.current != null && AttitudeSensor.current.enabled) + ":" + acceleration4.ToString();
        GUI.Label(new Rect(0, 110, Screen.width, 160), txt4, myButtonStyle);

        string txt5 = "atti :" + (Gyroscope.current != null) + " enable:" + (Gyroscope.current != null && Gyroscope.current.enabled) + ":" + acceleration5.ToString();
        GUI.Label(new Rect(0, 160, Screen.width, 210), txt5, myButtonStyle);

    }

    private void Awake()
    {
        // _gameSO.ResetGameState();
    }

    /// <summary>
    /// 注册监听输入事件
    /// </summary>
    void OnEnable()
    {

        // All sensors start out disabled so they have to manually be enabled first.
        if (Accelerometer.current != null)
            InputSystem.EnableDevice(Accelerometer.current);
        if (AttitudeSensor.current != null)
            InputSystem.EnableDevice(AttitudeSensor.current);
        if (Gyroscope.current != null)
            InputSystem.EnableDevice(Gyroscope.current);

        _inputManagerSO.confirmEvent += OnConfirm;
        _inputManagerSO.rotateEvent += OnRotate;
        _inputManagerSO.moveEvent += OnMove;
        _inputManagerSO.faceEvent += OnFace;
    }

    /// <summary>
    /// 取消注册监听
    /// </summary>
    void OnDisable()
    {
        _inputManagerSO.confirmEvent -= OnConfirm;
        _inputManagerSO.rotateEvent -= OnRotate;
        _inputManagerSO.moveEvent -= OnMove;
        _inputManagerSO.faceEvent -= OnFace;

        if (Accelerometer.current != null)
            InputSystem.DisableDevice(Accelerometer.current);
        if (AttitudeSensor.current != null)
            InputSystem.DisableDevice(AttitudeSensor.current);
        if (Gyroscope.current != null)
            InputSystem.DisableDevice(Gyroscope.current);
    }

    protected void Update()
    {
        // same as move
        // if (Accelerometer.current != null)
        //     acceleration = Accelerometer.current.acceleration.ReadValue();

        acceleration5 = Input.gyro.attitude;

    }

    private void OnConfirm()
    {
        Debug.Log("OnConfirm:");

        switch (_gameSO.gameState)
        {
            case GameState.Start:
                _gameSO.UpdateGameState(GameState.Playing);
                break;
            case GameState.Win:
            case GameState.Lose:
                _gameSO.ResetGameState(GameState.Playing);
                break;
            default:
                break;
        }

    }
    private void OnRotate(Vector3 coord)
    {

        acceleration1 = coord;
    }

    private void OnMove(Vector3 coord)
    {
        acceleration3 = coord;
    }
    private void OnFace(Quaternion coord)
    {
        acceleration4 = coord;
    }

}

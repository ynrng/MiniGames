using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{

    // Assign delegate{} to events to initialise them with an empty delegate
    // so we can skip the null check when we use them

    #region events declare
    // ICookBeefActions
    public static event UnityAction<Vector3> rotateEvent = delegate { };
    public static event UnityAction<Quaternion> faceEvent = delegate { };
    public static event UnityAction<Gyroscope> gyroEvent = delegate { };

    /// <summary></summary>
    public static event UnityAction<Vector3> moveEvent = delegate { };

    // IMenusActions
    /// <summary></summary>
    public static event UnityAction confirmEvent = delegate { };
    #endregion

    Gyroscope gyro;
    bool gyroEnabled = false;
    GameObject container;
    Quaternion rot;

    bool accelerometeSupported = false;

    // private InputControls controls;

    private void Start()
    {
        // gyro setup
        gyroEnabled = EnableGyro();
        // container = new GameObject("container");
        // container.transform.position = transform.position;
        // transform.parent = container.transform;

        accelerometeSupported = EnableAccelerometer();

    }

    //This is a legacy function, check out the UI section for other ways to create your UI
    void OnGUI()
    {
        int fontSize = Screen.width / 40;
        GUIStyle myButtonStyle = new GUIStyle(GUI.skin.label);
        myButtonStyle.fontSize = fontSize;

        // Set color for selected and unselected buttons
        myButtonStyle.normal.textColor = Color.red;
        myButtonStyle.alignment = TextAnchor.UpperRight;
        //Output the rotation rate, attitude and the enabled state of the gyroscope as a Label
        if (gyroEnabled)
        {
            GUI.Label(new Rect(0, fontSize * 0, Screen.width, fontSize), "Gyro enabled : " + gyro.enabled, myButtonStyle);
            GUI.Label(new Rect(0, fontSize * 1, Screen.width, fontSize), "Gyro rotation rate " + gyro.rotationRate, myButtonStyle);
            GUI.Label(new Rect(0, fontSize * 2, Screen.width, fontSize), "Gyro acce" + gyro.userAcceleration, myButtonStyle);
            GUI.Label(new Rect(0, fontSize * 3, Screen.width, fontSize), "Gyro attitude" + gyro.attitude, myButtonStyle);
            GUI.Label(new Rect(0, fontSize * 4, Screen.width, fontSize), "Gyro gravity" + gyro.gravity, myButtonStyle);
            GUI.Label(new Rect(0, fontSize * 6, Screen.width, fontSize), "accelerate: " + Input.acceleration, myButtonStyle);
        }
        else
        {
            GUI.Label(new Rect(500, 300, 200, fontSize), "Gyro no surpport");
        }
    }

    public void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            confirmEvent.Invoke();
        }
        if (gyroEnabled)
        {
            // transform.localRotation = gyro.attitude * rot;

            faceEvent.Invoke(GyroToUnity(gyro.attitude));
            rotateEvent.Invoke(gyro.userAcceleration);
            gyroEvent.Invoke(gyro);
        }
        if (accelerometeSupported)
        {
            moveEvent.Invoke(Input.acceleration);
        }
    }

    /********************************************/

    // The Gyroscope is right-handed.  Unity is left handed.
    // Make the necessary change to the camera.
    void GyroModifyCamera()
    {
        transform.rotation = GyroToUnity(Input.gyro.attitude);
    }

    static public bool GyroEnabled()
    {
        return SystemInfo.supportsGyroscope;
    }

    private bool EnableGyro()
    {
        if (GyroEnabled())
        {
            gyro = Input.gyro;
            gyro.enabled = true;

            // container.transform.rotation = Quaternion.Euler(90f, 90f, 90f);
            // rot = new Quaternion(0, 0, 1, 0);

            return true;
        }
        return false;
    }

    private bool EnableAccelerometer()
    {
        return SystemInfo.supportsAccelerometer;
    }
    // -------------public--------------------
    public static Quaternion GyroToUnity(Quaternion q)
    {

        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
}
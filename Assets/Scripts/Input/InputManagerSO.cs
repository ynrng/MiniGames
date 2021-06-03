using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Gyroscope = UnityEngine.InputSystem.Gyroscope;

[CreateAssetMenu(fileName = "InputManager", menuName = "SO/InputManager", order = 0)]
public class InputManagerSO : ScriptableObject, InputControls.ICookBeefActions, InputControls.IMenusActions
{

    // Assign delegate{} to events to initialise them with an empty delegate
    // so we can skip the null check when we use them

    #region events declare
    // ICookBeefActions
    public event UnityAction<Vector3> rotateEvent = delegate { };

    public event UnityAction<Quaternion> faceEvent = delegate { };
    public event UnityAction<Vector3> moveEvent = delegate { };

    // IMenusActions
    public event UnityAction confirmEvent = delegate { };
    #endregion

    private InputControls controls;

    public void OnEnable()
    {
        // All sensors start out disabled so they have to manually be enabled first.
        if (Accelerometer.current != null)
            InputSystem.EnableDevice(Accelerometer.current);
        if (AttitudeSensor.current != null)
            InputSystem.EnableDevice(AttitudeSensor.current);
        if (Gyroscope.current != null)
            InputSystem.EnableDevice(Gyroscope.current);

        // 如果在 Awake 里做初始化，那么极大可能会得到null reference。
        if (controls == null)
        {
            controls = new InputControls();
            controls.CookBeef.SetCallbacks(this);
            controls.Menus.SetCallbacks(this);
        }

        Debug.Log("[InputManagerSO]Enabling gameplay controls!");
        controls.CookBeef.Enable();
        controls.Menus.Enable();

    }

    public void OnDisable()
    {
        Debug.Log("[InputManagerSO]Disabling gameplay controls!");
        controls.CookBeef.Disable();
        controls.Menus.Disable();

        // Enable the gyroscope, if present.
        if (Accelerometer.current != null)
            InputSystem.DisableDevice(Accelerometer.current);
        if (AttitudeSensor.current != null)
            InputSystem.DisableDevice(AttitudeSensor.current);
        if (Gyroscope.current != null)
            InputSystem.DisableDevice(Gyroscope.current);
    }

    #region ICookBeefActions
    public void OnRotate(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Vector3 coord = context.ReadValue<Vector3>();
            rotateEvent.Invoke(coord);
            Debug.Log("[InputManagerSO]OnRotate:" + coord.x + ":" + coord.y + ":" + coord.z);

        }
    }

    public void OnAttitude(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Quaternion coord = context.ReadValue<Quaternion>();
            faceEvent.Invoke(coord);
            Debug.Log("[InputManagerSO]OnAttitude:" + coord);
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Vector3 coord = context.ReadValue<Vector3>();
            moveEvent.Invoke(coord);
            Debug.Log("[InputManagerSO]OnMove:" + coord.x + ":" + coord.y + ":" + coord.z);
        }
    }
    #endregion

    #region IMenusActions
    public void OnConfirm(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            confirmEvent.Invoke();
            Debug.Log("[InputManagerSO]OnConfirm");
        }

    }
    #endregion

}
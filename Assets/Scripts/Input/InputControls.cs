// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Input/InputControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputControls"",
    ""maps"": [
        {
            ""name"": ""CookBeef"",
            ""id"": ""0630e107-8c92-4ffd-b39c-ad962f423ed4"",
            ""actions"": [
                {
                    ""name"": ""Rotate"",
                    ""type"": ""Value"",
                    ""id"": ""fa4fd8fc-243a-47a9-aeb5-537a471dc366"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attitude"",
                    ""type"": ""Value"",
                    ""id"": ""668335ba-6a82-4509-94ec-85affba920ef"",
                    ""expectedControlType"": ""Quaternion"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""16a69f79-89dc-4598-beb5-b207b04bd90a"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""1c9e1637-072c-4da7-9daa-f5fb44b210e1"",
                    ""path"": ""<Gyroscope>/angularVelocity"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""95ae14de-72da-464d-bd85-20c055b19b6a"",
                    ""path"": ""<AttitudeSensor>/attitude"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attitude"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4396d0d7-cf00-43e5-9614-d35882429fb8"",
                    ""path"": ""<Accelerometer>/acceleration"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Menus"",
            ""id"": ""c0944ecc-9035-462e-8bbf-74c05dfda536"",
            ""actions"": [
                {
                    ""name"": ""Confirm"",
                    ""type"": ""Button"",
                    ""id"": ""203b569c-8b1d-4708-938a-f4f5e18cd148"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""d61e7f74-ab08-440d-8703-563a5a2014f0"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Confirm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a9a7c406-72c6-445d-a5f3-36c10d9db23f"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Confirm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1eb91848-c1ce-4324-898e-10fd07934710"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Confirm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""28dc9e6e-9f26-4e4f-b372-ed955d62ecb5"",
                    ""path"": ""<Pointer>/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Confirm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // CookBeef
        m_CookBeef = asset.FindActionMap("CookBeef", throwIfNotFound: true);
        m_CookBeef_Rotate = m_CookBeef.FindAction("Rotate", throwIfNotFound: true);
        m_CookBeef_Attitude = m_CookBeef.FindAction("Attitude", throwIfNotFound: true);
        m_CookBeef_Move = m_CookBeef.FindAction("Move", throwIfNotFound: true);
        // Menus
        m_Menus = asset.FindActionMap("Menus", throwIfNotFound: true);
        m_Menus_Confirm = m_Menus.FindAction("Confirm", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // CookBeef
    private readonly InputActionMap m_CookBeef;
    private ICookBeefActions m_CookBeefActionsCallbackInterface;
    private readonly InputAction m_CookBeef_Rotate;
    private readonly InputAction m_CookBeef_Attitude;
    private readonly InputAction m_CookBeef_Move;
    public struct CookBeefActions
    {
        private @InputControls m_Wrapper;
        public CookBeefActions(@InputControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Rotate => m_Wrapper.m_CookBeef_Rotate;
        public InputAction @Attitude => m_Wrapper.m_CookBeef_Attitude;
        public InputAction @Move => m_Wrapper.m_CookBeef_Move;
        public InputActionMap Get() { return m_Wrapper.m_CookBeef; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CookBeefActions set) { return set.Get(); }
        public void SetCallbacks(ICookBeefActions instance)
        {
            if (m_Wrapper.m_CookBeefActionsCallbackInterface != null)
            {
                @Rotate.started -= m_Wrapper.m_CookBeefActionsCallbackInterface.OnRotate;
                @Rotate.performed -= m_Wrapper.m_CookBeefActionsCallbackInterface.OnRotate;
                @Rotate.canceled -= m_Wrapper.m_CookBeefActionsCallbackInterface.OnRotate;
                @Attitude.started -= m_Wrapper.m_CookBeefActionsCallbackInterface.OnAttitude;
                @Attitude.performed -= m_Wrapper.m_CookBeefActionsCallbackInterface.OnAttitude;
                @Attitude.canceled -= m_Wrapper.m_CookBeefActionsCallbackInterface.OnAttitude;
                @Move.started -= m_Wrapper.m_CookBeefActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_CookBeefActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_CookBeefActionsCallbackInterface.OnMove;
            }
            m_Wrapper.m_CookBeefActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Rotate.started += instance.OnRotate;
                @Rotate.performed += instance.OnRotate;
                @Rotate.canceled += instance.OnRotate;
                @Attitude.started += instance.OnAttitude;
                @Attitude.performed += instance.OnAttitude;
                @Attitude.canceled += instance.OnAttitude;
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
            }
        }
    }
    public CookBeefActions @CookBeef => new CookBeefActions(this);

    // Menus
    private readonly InputActionMap m_Menus;
    private IMenusActions m_MenusActionsCallbackInterface;
    private readonly InputAction m_Menus_Confirm;
    public struct MenusActions
    {
        private @InputControls m_Wrapper;
        public MenusActions(@InputControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Confirm => m_Wrapper.m_Menus_Confirm;
        public InputActionMap Get() { return m_Wrapper.m_Menus; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenusActions set) { return set.Get(); }
        public void SetCallbacks(IMenusActions instance)
        {
            if (m_Wrapper.m_MenusActionsCallbackInterface != null)
            {
                @Confirm.started -= m_Wrapper.m_MenusActionsCallbackInterface.OnConfirm;
                @Confirm.performed -= m_Wrapper.m_MenusActionsCallbackInterface.OnConfirm;
                @Confirm.canceled -= m_Wrapper.m_MenusActionsCallbackInterface.OnConfirm;
            }
            m_Wrapper.m_MenusActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Confirm.started += instance.OnConfirm;
                @Confirm.performed += instance.OnConfirm;
                @Confirm.canceled += instance.OnConfirm;
            }
        }
    }
    public MenusActions @Menus => new MenusActions(this);
    public interface ICookBeefActions
    {
        void OnRotate(InputAction.CallbackContext context);
        void OnAttitude(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
    }
    public interface IMenusActions
    {
        void OnConfirm(InputAction.CallbackContext context);
    }
}

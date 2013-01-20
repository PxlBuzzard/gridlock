using UnityEngine;
using System.Collections.Generic;

public static class OuyaInputManager
{
#if UNITY_ANDROID
    static OuyaInputManager()
    {
        OuyaInputManager.initKeyStates();
    }
#endif

    public class OuyaKeyState
    {
        public OuyaSDK.OuyaPlayer player;
        public float m_axisLeftStickX = 0f;
        public float m_axisLeftStickY = 0f;
        public float m_axisRightStickX = 0f;
        public float m_axisRightStickY = 0f;
        public float m_axisLeftTrigger = 0f;
        public float m_axisRightTrigger = 0f;

        public bool m_buttonDPadCenter = false;
        public bool m_buttonDPadDown = false;
        public bool m_buttonDPadLeft = false;
        public bool m_buttonDPadRight = false;
        public bool m_buttonDPadUp = false;
        public bool m_buttonSystem = false;
        public bool m_buttonO = false;
        public bool m_buttonU = false;
        public bool m_buttonY = false;
        public bool m_buttonA = false;
        public bool m_buttonLB = false;
        public bool m_buttonLT = false;
        public bool m_buttonRB = false;
        public bool m_buttonRT = false;
    }

    /// <summary>
    /// Button Event Handler
    /// </summary>
    public static class OuyaButtonEvent
    {
        //Delegate for the button event
        public delegate void ButtonEventHandler(OuyaSDK.OuyaPlayer player, OuyaSDK.KeyEnum button, OuyaSDK.InputAction buttonState);
        //acutal button event, this is where you subscribte to the event using the += / -=
        public static event ButtonEventHandler ButtonsEvent;

        //Call this event ( trigger )
        public static void buttonPressEvent(OuyaSDK.OuyaPlayer player, OuyaSDK.KeyEnum button, OuyaSDK.InputAction buttonState)
        {
            if (ButtonsEvent != null)
            {
                ButtonsEvent(player, button, buttonState);
            }
        }

        //Subscribte to the event 
        public static void addButtonEventListener(ButtonEventHandler handler)
        {
            OuyaButtonEvent.ButtonsEvent += handler;
        }

        //UnSubscribte to the event
        public static void removeButtonEventListener(ButtonEventHandler handler)
        {
            OuyaButtonEvent.ButtonsEvent -= handler;
        }
    }



#if UNITY_ANDROID
    private static List<OuyaInputManager.OuyaKeyState> keyStates;
#endif

    /// <summary>
    /// Get Devices
    /// </summary>
    /// <returns>List<Device></returns>
    public static List<OuyaGameObject.Device> getDevices()
    {
        //get OuyaGameObject;
        OuyaGameObject ouyagameobject = GameObject.Find("OuyaGameObject").GetComponent<OuyaGameObject>();
        //Return a list of devices.
        return ouyagameobject.devices;
    }

    public static void HandleAxisEvent(OuyaSDK.InputAxisEvent inputEvent)
    {
#if UNITY_ANDROID
        switch (inputEvent.getAxisCode())
        {
            case OuyaSDK.AxisEnum.AXIS_LSTICK_X:
                OuyaInputManager.getPlayerKeyState(inputEvent.getPlayer()).m_axisLeftStickX = inputEvent.getAxis();
                break;
            case OuyaSDK.AxisEnum.AXIS_LSTICK_Y:
                OuyaInputManager.getPlayerKeyState(inputEvent.getPlayer()).m_axisLeftStickY = inputEvent.getAxis();
                break;
            case OuyaSDK.AxisEnum.AXIS_RSTICK_X:
                OuyaInputManager.getPlayerKeyState(inputEvent.getPlayer()).m_axisRightStickX = inputEvent.getAxis();
                break;
            case OuyaSDK.AxisEnum.AXIS_RSTICK_Y:
                OuyaInputManager.getPlayerKeyState(inputEvent.getPlayer()).m_axisRightStickY = inputEvent.getAxis();
                break;
            case OuyaSDK.AxisEnum.AXIS_LTRIGGER:
                OuyaInputManager.getPlayerKeyState(inputEvent.getPlayer()).m_axisLeftTrigger = inputEvent.getAxis();
                break;
            case OuyaSDK.AxisEnum.AXIS_RTRIGGER:
                OuyaInputManager.getPlayerKeyState(inputEvent.getPlayer()).m_axisRightTrigger = inputEvent.getAxis();
                break;
        }
#endif
    }

    public static void HandleButtonEvent(OuyaSDK.InputButtonEvent inputEvent)
    {
#if UNITY_ANDROID
		OuyaInputManager.OuyaKeyState keyState = OuyaInputManager.getPlayerKeyState(inputEvent.getPlayer());
		if (null == keyState)
		{
			return;
		}
		
		switch (inputEvent.getKeyAction())
        {
            case OuyaSDK.InputAction.KeyDown:
			case OuyaSDK.InputAction.KeyUp:
				switch (inputEvent.getKeyCode())
				{
					case OuyaSDK.KeyEnum.BUTTON_O:
                		keyState.m_buttonO = inputEvent.getKeyAction() == OuyaSDK.InputAction.KeyDown;
						break;
					case OuyaSDK.KeyEnum.BUTTON_U:
                		keyState.m_buttonU = inputEvent.getKeyAction() == OuyaSDK.InputAction.KeyDown;
						break;
					case OuyaSDK.KeyEnum.BUTTON_Y:
                		keyState.m_buttonY = inputEvent.getKeyAction() == OuyaSDK.InputAction.KeyDown;
						break;
					case OuyaSDK.KeyEnum.BUTTON_A:
                		keyState.m_buttonA = inputEvent.getKeyAction() == OuyaSDK.InputAction.KeyDown;
						break;
					case OuyaSDK.KeyEnum.BUTTON_LB:
                		keyState.m_buttonLB = inputEvent.getKeyAction() == OuyaSDK.InputAction.KeyDown;
						break;
					case OuyaSDK.KeyEnum.BUTTON_LT:
                		keyState.m_buttonLT = inputEvent.getKeyAction() == OuyaSDK.InputAction.KeyDown;
						break;
					case OuyaSDK.KeyEnum.BUTTON_RB:
                		keyState.m_buttonRB = inputEvent.getKeyAction() == OuyaSDK.InputAction.KeyDown;
						break;
					case OuyaSDK.KeyEnum.BUTTON_RT:
                		keyState.m_buttonRT = inputEvent.getKeyAction() == OuyaSDK.InputAction.KeyDown;
						break;
					case OuyaSDK.KeyEnum.BUTTON_SYSTEM:
                		keyState.m_buttonSystem = inputEvent.getKeyAction() == OuyaSDK.InputAction.KeyDown;
						break;
					case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                		keyState.m_buttonDPadDown = inputEvent.getKeyAction() == OuyaSDK.InputAction.KeyDown;
						break;
					case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                		keyState.m_buttonDPadLeft = inputEvent.getKeyAction() == OuyaSDK.InputAction.KeyDown;
						break;
					case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                		keyState.m_buttonDPadRight = inputEvent.getKeyAction() == OuyaSDK.InputAction.KeyDown;
						break;
					case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                		keyState.m_buttonDPadUp = inputEvent.getKeyAction() == OuyaSDK.InputAction.KeyDown;
						break;
					default:
						return;
				}
                OuyaButtonEvent.buttonPressEvent(inputEvent.getPlayer(), inputEvent.getKeyCode(), inputEvent.getKeyAction());
                break;
        }
#endif
    }

    /// <summary>
    /// Wrap Unity's method
    /// </summary>
    /// <param name="inputName"></param>
    /// <returns></returns>
    public static float GetAxis(string inputName, OuyaSDK.OuyaPlayer player)
    {
#if UNITY_ANDROID
        switch (inputName)
        {
            case "LT":
                return OuyaInputManager.getPlayerKeyState(player).m_axisLeftTrigger;
            case "RT":
                return OuyaInputManager.getPlayerKeyState(player).m_axisRightTrigger;
            case "RX":
                return OuyaInputManager.getPlayerKeyState(player).m_axisRightStickX;
            case "RY":
                return -OuyaInputManager.getPlayerKeyState(player).m_axisRightStickY;
            case "LX":
                return OuyaInputManager.getPlayerKeyState(player).m_axisLeftStickX;
            case "LY":
                return OuyaInputManager.getPlayerKeyState(player).m_axisLeftStickY;
        }
        return 0f;
#else
        return Input.GetAxis(inputName);
#endif
    }

    /// <summary>
    /// Wrap Unity's method
    /// </summary>
    /// <param name="inputName"></param>
    /// <returns></returns>
    public static bool GetButton(string inputName, OuyaSDK.OuyaPlayer player)
    {
#if UNITY_ANDROID
        switch (inputName)
        {
            case "SYS": //arbitrary name and mapping
                return OuyaInputManager.getPlayerKeyState(player).m_buttonSystem;
            case "DPC": //arbitrary name and mapping
                return OuyaInputManager.getPlayerKeyState(player).m_buttonDPadCenter;
            case "DPD": //arbitrary name and mapping
                return OuyaInputManager.getPlayerKeyState(player).m_buttonDPadDown;
            case "DPL": //arbitrary name and mapping
                return OuyaInputManager.getPlayerKeyState(player).m_buttonDPadLeft;
            case "DPR": //arbitrary name and mapping
                return OuyaInputManager.getPlayerKeyState(player).m_buttonDPadRight;
            case "DPU": //arbitrary name and mapping
                return OuyaInputManager.getPlayerKeyState(player).m_buttonDPadUp;
            case "O": //arbitrary name and mapping
                return OuyaInputManager.getPlayerKeyState(player).m_buttonO;
            case "U": //arbitrary name and mapping
                return OuyaInputManager.getPlayerKeyState(player).m_buttonU;
            case "Y": //arbitrary name and mapping
                return OuyaInputManager.getPlayerKeyState(player).m_buttonY;
            case "A": //arbitrary name and mapping
                return OuyaInputManager.getPlayerKeyState(player).m_buttonA;
            case "LT": //arbitrary name and mapping
                return OuyaInputManager.getPlayerKeyState(player).m_buttonLT;
            case "RT": //arbitrary name and mapping
                return OuyaInputManager.getPlayerKeyState(player).m_buttonRT;
            case "LB": //arbitrary name and mapping
                return OuyaInputManager.getPlayerKeyState(player).m_buttonLB;
            case "RB": //arbitrary name and mapping
                return OuyaInputManager.getPlayerKeyState(player).m_buttonRB;
        }
        return false;
#else
        return Input.GetButton(inputName);
#endif
    }

    /// <summary>
    /// Wrap Unity's method
    /// </summary>
    /// <param name="inputName"></param>
    /// <returns></returns>
    public static bool GetButtonDown(string inputName, OuyaSDK.OuyaPlayer player)
    {
#if UNITY_ANDROID
        // these will map to the Unity game's existing button names
        // the text cases are placeholders
        return OuyaInputManager.GetButton(inputName, player);
#else
        return Input.GetButtonDown(inputName);
#endif
    }

    /// <summary>
    /// Wrap Unity's method
    /// </summary>
    /// <param name="inputName"></param>
    /// <returns></returns>
    public static bool GetButtonUp(string inputName, OuyaSDK.OuyaPlayer player)
    {
#if UNITY_ANDROID
        // these will map to the Unity game's existing button names
        // the text cases are placeholders
        return !OuyaInputManager.GetButton(inputName, player);
#else
        return Input.GetButtonUp(inputName);
#endif
    }
	
#if UNITY_ANDROID
    public static void initKeyStates()
    {
        OuyaInputManager.OuyaKeyState keyState; 
        keyStates = new List<OuyaInputManager.OuyaKeyState>();

        keyState= new OuyaInputManager.OuyaKeyState();
        keyState.player = OuyaSDK.OuyaPlayer.player1;
        keyStates.Add(keyState);

        keyState = new OuyaInputManager.OuyaKeyState();
        keyState.player = OuyaSDK.OuyaPlayer.player2;
        keyStates.Add(keyState);

        keyState = new OuyaInputManager.OuyaKeyState();
        keyState.player = OuyaSDK.OuyaPlayer.player3;
        keyStates.Add(keyState);

        keyState = new OuyaInputManager.OuyaKeyState();
        keyState.player = OuyaSDK.OuyaPlayer.player4;
        keyStates.Add(keyState);
    }

    private static OuyaInputManager.OuyaKeyState getPlayerKeyState(OuyaSDK.OuyaPlayer player)
    {
		OuyaInputManager.OuyaKeyState keyState = keyStates.Find(delegate(OuyaInputManager.OuyaKeyState key) { return key.player.Equals(player); });
        return keyState;
    }
#endif
}
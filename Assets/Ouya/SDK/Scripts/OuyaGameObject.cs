using LitJson;
using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class OuyaGameObject : MonoBehaviour
{
    public bool debugOff = false;
    public List<Device> devices;
    private string m_inputData = string.Empty;

    #region Data Interfaces

    public void DebugLog(string message)
    {
        Debug.Log(message);
    }

    public void DebugLogError(string message)
    {
        Debug.LogError(message);
    }

    public void RequestDeveloperId()
    {
        if (string.IsNullOrEmpty(OuyaSDK.getDeveloperId()))
        {
            Debug.LogError("SDK is not initialized");
            return;
        }

        Debug.LogError("OUYA developer id has not been set");
    }

    public void ProductListListener(string jsonData)
    {
        if (string.IsNullOrEmpty(jsonData))
        {
            Debug.LogError("OuyaSDK.ProductListListener: received empty jsondata");
            return;
        }

        if (string.IsNullOrEmpty(OuyaSDK.getDeveloperId()))
        {
            Debug.LogError("SDK is not initialized");
            return;
        }

        if (null == OuyaSDK.getProductListListener())
        {
            Debug.LogError("OuyaSDK.ProductListListener: Product listener is not set");
            return;
        }

        if (null == OuyaSDK.getProductListListener().onSuccess)
        {
            Debug.LogError("OuyaSDK.ProductListListener: onSuccess is not set");
            return;
        }

        //Debug.Log(string.Format("OuyaSDK.ProductListListener: jsonData={0}", jsonData));
        OuyaSDK.Product product = JsonMapper.ToObject<OuyaSDK.Product>(jsonData);
        List<OuyaSDK.Product> products = new List<OuyaSDK.Product>();
        products.Add(product);
        OuyaSDK.getProductListListener().onSuccess(products);
    }

    #region Input Events
    public void onKeyDown(string jsonData)
    {
        InputListener(OuyaSDK.InputAction.KeyDown, jsonData);
    }

    public void onKeyUp(string jsonData)
    {
        InputListener(OuyaSDK.InputAction.KeyUp, jsonData);
    }

    public void onGenericMotionEvent(string jsonData)
    {
        InputListener(OuyaSDK.InputAction.GenericMotionEvent, jsonData);
    }

    public void onTouchEvent(string jsonData)
    {
        InputListener(OuyaSDK.InputAction.TouchEvent, jsonData);
    }

    public void onTrackballEvent(string jsonData)
    {
        InputListener(OuyaSDK.InputAction.TrackballEvent, jsonData);
    }

    public void onSetDevices(string jsonData)
    {
		//Debug.Log(string.Format("Devices jsonData={0}", jsonData));
		
        List<Device> deviceList = new List<Device>();
        deviceList = JsonMapper.ToObject<List<Device>>(jsonData);
        foreach(Device d in deviceList){
            //Debug.Log("DeviceID:" + d.id + " DevicePlayer:" + d.player + " DeviceName:" + d.name);
            devices.Add(d);
        }
        //Debug.Log("DeviceCount:" + devices.Count);
    }
    #endregion

    private bool m_detectLTriggerDown = false;
    private bool m_detectRTriggerDown = false;

    private void InputListener(OuyaSDK.InputAction inputAction, string jsonData)
    {
        #region Error Handling
        if (string.IsNullOrEmpty(jsonData))
        {
            Debug.LogError("OuyaSDK.InputListener: received invalid jsondata");
            return;
        }
        m_inputData = jsonData;

        if (string.IsNullOrEmpty(OuyaSDK.getDeveloperId()))
        {
            Debug.LogError("SDK is not initialized");
            return;
        }

        //Debug.Log(string.Format("OuyaSDK.InputListener: inputAction={0} jsonData={1}", inputAction, jsonData));

        if (null == OuyaSDK.getInputAxisListener())
        {
            Debug.LogError("OuyaSDK.InputListener: Input axis listener is not set");
            return;
        }

        if (null == OuyaSDK.getInputAxisListener().onSuccess)
        {
            Debug.LogError("OuyaSDK.InputListener: Input axis listener onSuccess is not set");
            return;
        }

        if (null == OuyaSDK.getInputButtonListener())
        {
            Debug.LogError("OuyaSDK.InputListener: Input button listener is not set");
            return;
        }

        if (null == OuyaSDK.getInputButtonListener().onSuccess)
        {
            Debug.LogError("OuyaSDK.InputListener: Input button listener onSuccess is not set");
            return;
        }
        #endregion

        InputContainer container = JsonMapper.ToObject<InputContainer>(jsonData);
        OuyaSDK.InputAxisEvent inputAxis;
        OuyaSDK.InputButtonEvent inputButton;
        
        Device device = devices.Find(delegate(Device d) { return (null == d || null == container) ? false : (d.id == container.DeviceId); });
        //Debug.Log("Device:" + device.id + " Player" + device.player);

        switch (container.DeviceName.ToUpper())
        {
            #region OUYA Game Controller
            case "OUYA GAME CONTROLLER":
                switch (inputAction)
                {
                    #region KeyDown
                    case OuyaSDK.InputAction.KeyDown:
                        if (container.KeyEvent.mRepeatCount == 0 || container.KeyEvent.mRepeatCount > 5)
                        {
                            switch (container.KeyEvent.mKeyCode)
                            {
                                case 97:
                                    inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_A, device.player);
                                    OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                    break;
                                case 96:
                                    inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_O, device.player);
                                    OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                    break;
                                case 99:
                                    inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_U, device.player);
                                    OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                    break;
                                case 100:
                                    inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_Y, device.player);
                                    OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                    break;

                                case 102:
                                    inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_LB, device.player);
                                    OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                    break;
                                case 104:
                                    inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_LT, device.player);
                                    OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                    break;
                                case 106:
                                    inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_L3, device.player);
                                    OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                    break;

                                case 103:
                                    inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_RB, device.player);
                                    OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                    break;
                                case 105:
                                    inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_RT, device.player);
                                    OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                    break;
                                case 107:
                                    inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_R3, device.player);
                                    OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                    break;

                                case 19:
                                    inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_UP, device.player);
                                    OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                    break;
                                case 20:
                                    inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN, device.player);
                                    OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                    break;
                                case 21:
                                    inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT, device.player);
                                    OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                    break;
                                case 22:
                                    inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT, device.player);
                                    OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                    break;

                                default:
                                    Debug.Log("Unhandled " + inputAction + ": " + container.KeyEvent.mKeyCode);
                                    break;
                            }
                        }
                        break;
                    #endregion

                    #region KeyUp
                    case OuyaSDK.InputAction.KeyUp:
                        switch (container.KeyEvent.mKeyCode)
                        {
                            case 97:
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_A, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                break;
                            case 96:
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_O, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                break;
                            case 99:
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_U, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                break;
                            case 100:
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_Y, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                break;

                            case 102:
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_LB, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                break;
                            case 104:
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_LT, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                break;
                            case 106:
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_L3, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                break;

                            case 103:
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_RB, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                break;
                            case 105:
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_RT, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                break;
                            case 107:
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_R3, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                break;

                            case 19:
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_UP, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                break;
                            case 20:
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                break;
                            case 21:
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                break;
                            case 22:
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                                break;

                            default:
                                Debug.Log("Unhandled " + inputAction + ": " + container.KeyEvent.mKeyCode);
                                break;
                        }
                        break;
                    #endregion

                    #region GenericMotionEvent
                    case OuyaSDK.InputAction.GenericMotionEvent:
                        
                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_LSTICK_X, container.AxisX, device.player);
                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_LSTICK_Y, container.AxisY, device.player);
                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_RSTICK_X, container.AxisZ, device.player);
                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_RSTICK_Y, container.AxisRZ, device.player);
                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        //inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_LTRIGGER,container.AxisLTrigger, device.player);
                        //OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        //inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_RTRIGGER,container.AxisRTrigger, device.player);
                        //OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        if (container.AxisLTrigger > 0.13f)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(OuyaSDK.InputAction.KeyDown, OuyaSDK.KeyEnum.BUTTON_LT, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            m_detectLTriggerDown = true;
                        }
                        else //if (m_detectLTriggerDown)
                        {
                            m_detectLTriggerDown = false;
                            inputButton = new OuyaSDK.InputButtonEvent(OuyaSDK.InputAction.KeyUp, OuyaSDK.KeyEnum.BUTTON_LT, device.player);
                            container.AxisLTrigger = 0; //override for deadzone
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }

                        if (container.AxisRTrigger > 0.13f)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(OuyaSDK.InputAction.KeyDown, OuyaSDK.KeyEnum.BUTTON_RT, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            m_detectRTriggerDown = true;
                        }
                        else //if (m_detectRTriggerDown)
                        {
                            m_detectRTriggerDown = false;
                            inputButton = new OuyaSDK.InputButtonEvent(OuyaSDK.InputAction.KeyUp, OuyaSDK.KeyEnum.BUTTON_RT, device.player);
                            container.AxisRTrigger = 0; //override for deadzone
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }

                        break;
                    #endregion
                }
                break;
            #endregion

            #region Microsoft X-Box 360 pad
            case "MICROSOFT X-BOX 360 PAD":
                switch (inputAction)
                {
                    #region KeyDown
                    case OuyaSDK.InputAction.KeyDown:
                        if (container.KeyEvent.mRepeatCount == 0 || container.KeyEvent.mRepeatCount > 5)
                        {
                            if (container.KeyEvent.mKeyCode == 97)
                            {
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_A, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            }
                            if (container.KeyEvent.mKeyCode == 96)
                            {
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_O, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            }
                            if (container.KeyEvent.mKeyCode == 99)
                            {
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_U, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            }
                            if (container.KeyEvent.mKeyCode == 100)
                            {
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_Y, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            }
                            if (container.KeyEvent.mKeyCode == 102)
                            {
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_LB, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            }
                            if (container.KeyEvent.mKeyCode == 103)
                            {
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_RB, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            }
                        }
                        break;
                    #endregion
                    
                    #region KeyUp
                    case OuyaSDK.InputAction.KeyUp:
                        if (container.KeyEvent.mKeyCode == 97)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_A, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }
                        if (container.KeyEvent.mKeyCode == 96)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_O, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }
                        if (container.KeyEvent.mKeyCode == 99)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_U, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }
                        if (container.KeyEvent.mKeyCode == 100)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_Y, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }
                        if (container.KeyEvent.mKeyCode == 102)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_LB, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }
                        if (container.KeyEvent.mKeyCode == 103)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_RB, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }
                        break;
                    #endregion

                    #region GenericMotionEvent
                    case OuyaSDK.InputAction.GenericMotionEvent:
                        if (container.AxisHatY == -1)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_UP, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }

                        if (container.AxisHatY == 1)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }

                        if (container.AxisHatX == -1)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }

                        if (container.AxisHatX == 1)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }

                        if (container.AxisHatX == 0 &&
                            container.AxisHatY == 0)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_CENTER, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }

                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_LSTICK_X, container.AxisX, device.player);
                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_LSTICK_Y, container.AxisY, device.player);
                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_RSTICK_X, container.AxisZ, device.player);
                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_RSTICK_Y, container.AxisRZ, device.player);
                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_LTRIGGER, container.AxisLTrigger, device.player);
                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_RTRIGGER, container.AxisRTrigger, device.player);
                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        if (container.AxisLTrigger > 0.1f)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(OuyaSDK.InputAction.KeyDown, OuyaSDK.KeyEnum.BUTTON_LT, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            m_detectLTriggerDown = true;
                        }
                        else if (m_detectLTriggerDown)
                        {
                            m_detectLTriggerDown = false;
                            inputButton = new OuyaSDK.InputButtonEvent(OuyaSDK.InputAction.KeyUp, OuyaSDK.KeyEnum.BUTTON_LT, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }

                        if (container.AxisRTrigger > 0.1f)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(OuyaSDK.InputAction.KeyDown, OuyaSDK.KeyEnum.BUTTON_RT, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            m_detectRTriggerDown = true;
                        }
                        else if (m_detectRTriggerDown)
                        {
                            m_detectRTriggerDown = false;
                            inputButton = new OuyaSDK.InputButtonEvent(OuyaSDK.InputAction.KeyUp, OuyaSDK.KeyEnum.BUTTON_RT, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }

                        break;
                    #endregion
                }
                break;
            #endregion

            #region idroid:con
            case "IDROID:CON":
                switch (inputAction)
                {
                    #region KeyDown
                    case OuyaSDK.InputAction.KeyDown:
                        if (container.KeyEvent.mRepeatCount == 0 || container.KeyEvent.mRepeatCount > 5)
                        {
                            if (container.KeyEvent.mKeyCode == 189)
                            {
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_A, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            }
                            if (container.KeyEvent.mKeyCode == 188)
                            {
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_O, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            }
                            if (container.KeyEvent.mKeyCode == 190)
                            {
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_U, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            }
                            if (container.KeyEvent.mKeyCode == 191)
                            {
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_Y, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            }
                            if (container.KeyEvent.mKeyCode == 192)
                            {
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_LB, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            }
                            if (container.KeyEvent.mKeyCode == 193)
                            {
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_RB, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            }
                        }
                        break;
                    #endregion

                    #region KeyUp
                    case OuyaSDK.InputAction.KeyUp:
                        if (container.KeyEvent.mKeyCode == 189)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_A, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }
                        if (container.KeyEvent.mKeyCode == 188)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_O, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }
                        if (container.KeyEvent.mKeyCode == 190)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_U, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }
                        if (container.KeyEvent.mKeyCode == 191)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_Y, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }
                        if (container.KeyEvent.mKeyCode == 192)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_LB, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }
                        if (container.KeyEvent.mKeyCode == 193)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_RB, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }
                        break;
                    #endregion

                    #region GenericMotionEvent
                    case OuyaSDK.InputAction.GenericMotionEvent:
                        if (container.AxisHatY == -1)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_UP, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }

                        if (container.AxisHatY == 1)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }

                        if (container.AxisHatX == -1)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }

                        if (container.AxisHatX == 1)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }

                        if (container.AxisHatX == 0 &&
                            container.AxisHatY == 0)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_CENTER, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }

                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_LSTICK_X, container.AxisX, device.player);
                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_LSTICK_Y, container.AxisY, device.player);
                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_RSTICK_X, container.AxisRX, device.player);
                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_RSTICK_Y, container.AxisRY, device.player);
                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_LTRIGGER, container.AxisZ, device.player);
                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_RTRIGGER, container.AxisZ, device.player);
                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        if (container.AxisZ > 0.1f)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(OuyaSDK.InputAction.KeyDown, OuyaSDK.KeyEnum.BUTTON_LT, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            m_detectLTriggerDown = true;
                        }
                        else if (m_detectLTriggerDown)
                        {
                            m_detectLTriggerDown = false;
                            inputButton = new OuyaSDK.InputButtonEvent(OuyaSDK.InputAction.KeyUp, OuyaSDK.KeyEnum.BUTTON_LT, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }

                        if (container.AxisZ < -0.1f)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(OuyaSDK.InputAction.KeyDown, OuyaSDK.KeyEnum.BUTTON_RT, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            m_detectRTriggerDown = true;
                        }
                        else if (m_detectRTriggerDown)
                        {
                            m_detectRTriggerDown = false;
                            inputButton = new OuyaSDK.InputButtonEvent(OuyaSDK.InputAction.KeyUp, OuyaSDK.KeyEnum.BUTTON_RT, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }

                        break;
                    #endregion
                }
                break;
            #endregion

            #region USB Controller
            case "USB CONTROLLER":
            default:
                switch (inputAction)
                {
                    #region KeyDown
                    case OuyaSDK.InputAction.KeyDown:
                        if (container.KeyEvent.mRepeatCount == 0 || container.KeyEvent.mRepeatCount > 5)
                        {
                            if (container.KeyEvent.mKeyCode == 97)
                            {
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_A, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            }
                            if (container.KeyEvent.mKeyCode == 98)
                            {
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_O, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            }
                            if (container.KeyEvent.mKeyCode == 99)
                            {
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_U, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            }
                            if (container.KeyEvent.mKeyCode == 96)
                            {
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_Y, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            }
                            if (container.KeyEvent.mKeyCode == 100)
                            {
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_LB, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            }
                            if (container.KeyEvent.mKeyCode == 101)
                            {
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_RB, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                            }
                            if (container.KeyEvent.mKeyCode == 102)
                            {
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_LT, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);

                                switch (inputAction)
                                {
                                    case OuyaSDK.InputAction.KeyDown:
                                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_LTRIGGER, 1, device.player);
                                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);
                                        break;
                                    case OuyaSDK.InputAction.KeyUp:
                                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_LTRIGGER, 0, device.player);
                                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);
                                        break;
                                }
                            }
                            if (container.KeyEvent.mKeyCode == 103)
                            {
                                inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_RT, device.player);
                                OuyaSDK.getInputButtonListener().onSuccess(inputButton);

                                switch (inputAction)
                                {
                                    case OuyaSDK.InputAction.KeyDown:
                                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_RTRIGGER, -1, device.player);
                                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);
                                        break;
                                    case OuyaSDK.InputAction.KeyUp:
                                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_RTRIGGER, 0, device.player);
                                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);
                                        break;
                                }
                            }
                        }
                        break;
                    #endregion

                    #region KeyUp
                    case OuyaSDK.InputAction.KeyUp:
                        if (container.KeyEvent.mKeyCode == 97)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_A, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }
                        if (container.KeyEvent.mKeyCode == 98)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_O, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }
                        if (container.KeyEvent.mKeyCode == 99)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_U, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }
                        if (container.KeyEvent.mKeyCode == 96)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_Y, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }
                        if (container.KeyEvent.mKeyCode == 100)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_LB, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }
                        if (container.KeyEvent.mKeyCode == 101)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_RB, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }
                        if (container.KeyEvent.mKeyCode == 102)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_LT, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);

                            switch (inputAction)
                            {
                                case OuyaSDK.InputAction.KeyDown:
                                    inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_LTRIGGER, 1, device.player);
                                    OuyaSDK.getInputAxisListener().onSuccess(inputAxis);
                                    break;
                                case OuyaSDK.InputAction.KeyUp:
                                    inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_LTRIGGER, 0, device.player);
                                    OuyaSDK.getInputAxisListener().onSuccess(inputAxis);
                                    break;
                            }
                        }
                        if (container.KeyEvent.mKeyCode == 103)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_RT, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);

                            switch (inputAction)
                            {
                                case OuyaSDK.InputAction.KeyDown:
                                    inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_RTRIGGER, -1, device.player);
                                    OuyaSDK.getInputAxisListener().onSuccess(inputAxis);
                                    break;
                                case OuyaSDK.InputAction.KeyUp:
                                    inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_RTRIGGER, 0, device.player);
                                    OuyaSDK.getInputAxisListener().onSuccess(inputAxis);
                                    break;
                            }
                        }
                        break;
                    #endregion

                    #region GenericMotionEvent
                    case OuyaSDK.InputAction.GenericMotionEvent:
                        if (container.AxisHatY == -1)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_UP, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }

                        if (container.AxisHatY == 1)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }

                        if (container.AxisHatX == -1)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }

                        if (container.AxisHatX == 1)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }

                        if (container.AxisHatX == 0 &&
                            container.AxisHatY == 0)
                        {
                            inputButton = new OuyaSDK.InputButtonEvent(inputAction, OuyaSDK.KeyEnum.BUTTON_DPAD_CENTER, device.player);
                            OuyaSDK.getInputButtonListener().onSuccess(inputButton);
                        }

                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_LSTICK_X, container.AxisX, device.player);
                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_LSTICK_Y, container.AxisY, device.player);
                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_RSTICK_X, container.AxisZ, device.player);
                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_RSTICK_Y, container.AxisRZ, device.player);
                        OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        //inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_LTRIGGER, container.AxisZ);
                        //OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        //inputAxis = new OuyaSDK.InputAxisEvent(inputAction, OuyaSDK.AxisEnum.AXIS_RTRIGGER, container.AxisZ);
                        //OuyaSDK.getInputAxisListener().onSuccess(inputAxis);

                        break;
                    #endregion
                }
                break;
            #endregion
        }
    }

    public class InputContainer
    {
        public string Method = string.Empty;
        public int KeyCode = 0;
        public KeyEvent KeyEvent = null;
        public MotionEvent MotionEvent = null;

        public int Action = 0;
        public int ActionCode = 0;
        public int ActionIndex = 0;
        public int ActionMasked = 0;
        public int ButtonState = 0;

        public float AxisBrake = 0;
        public float AxisDistance = 0;
        public float AxisGas = 0;
        public float AxisGeneric1 = 0;
        public float AxisGeneric2 = 0;
        public float AxisGeneric3 = 0;
        public float AxisGeneric4 = 0;
        public float AxisGeneric5 = 0;
        public float AxisGeneric6 = 0;
        public float AxisGeneric7 = 0;
        public float AxisGeneric8 = 0;
        public float AxisGeneric9 = 0;
        public float AxisGeneric10 = 0;
        public float AxisGeneric11 = 0;
        public float AxisGeneric12 = 0;
        public float AxisGeneric13 = 0;
        public float AxisGeneric14 = 0;
        public float AxisGeneric15 = 0;
        public float AxisGeneric16 = 0;
        public float AxisHatX = 0;
        public float AxisHatY = 0;
        public float AxisHScroll = 0;
        public float AxisLTrigger = 0;
        public float AxisOrientation = 0;
        public float AxisPressire = 0;
        public float AxisRTrigger = 0;
        public float AxisRudder = 0;
        public float AxisRX = 0;
        public float AxisRY = 0;
        public float AxisRZ = 0;
        public float AxisSize = 0;
        public float AxisThrottle = 0;
        public float AxisTilt = 0;
        public float AxisToolMajor = 0;
        public float AxisToolMinor = 0;
        public float AxisVScroll = 0;
        public float AxisWheel = 0;
        public float AxisX = 0;
        public float AxisY = 0;
        public float AxisZ = 0;
        public int DeviceId = 0;
        public string DeviceName = string.Empty;
        public int EdgeFlags = 0;
        public int Flags = 0;
        public int MetaState = 0;
        public int PointerCount = 0;
        public float Pressure = 0;
        public float X = 0;
        public float Y = 0;
    }

    public class KeyEvent
    {
        public int mEventTime;
        public int mDownTime;
        public int mDeviceId;
        public int mFlags;
        public int mKeyCode;
        public int mMetaState;
        public int mAction;
        public bool mRecycled;
        public int mRepeatCount;
        public int mScanCode;
        public int mSource;
        public int mSeq;
    }

    public class MotionEvent
    {
        public bool mRecycled;
        public int mNativePtr;
        public int mSeq;
    }

    public class Device
    {
        public int id = 0;
        public OuyaSDK.OuyaPlayer player = OuyaSDK.OuyaPlayer.none;
        public string name = string.Empty;
    }

    #endregion

    void Awake(){
        devices = new List<Device>();
    }

    private void OnStart()
    {
        Input.ResetInputAxes();

    }

    private void OnGUI()
    {

        GameObject goNGUI = GameObject.Find("_NGUIHandler");
        if (goNGUI != null)
        {
//            OuyaNGUIHandler nguiHandler = goNGUI.GetComponent<OuyaNGUIHandler>();
  //          nguiHandler.rawOut.text = m_inputData;
        }
        else
        {
            if (!debugOff)
            {
                GUILayout.Label(m_inputData);

                foreach (string joystick in Input.GetJoystickNames())
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Joystick:");
                    GUILayout.Label(joystick);
                    GUILayout.EndHorizontal();
                }
            }

        }

    }
}
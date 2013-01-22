using System;
using System.Collections.Generic;
using UnityEngine;

public class OuyaShowController : MonoBehaviour
{
    /// <summary>
    /// This is your assigned developer id
    /// </summary>
    private const string DEVELOPER_ID = "310a8f51-4d6e-4ae5-bda0-b93878e5f5d0";

    #region Transition fields

    /// <summary>
    /// Meta reference for camera positions
    /// </summary>
    public Transform[] CameraPositions = null;

    /// <summary>
    /// The camera to transition to
    /// </summary>
    private int m_transitionId = 0;

    /// <summary>
    /// The previous transition id
    /// </summary>
    private int m_oldTransitionId = 0;

    /// <summary>
    /// A timer for transitions
    /// </summary>
    private DateTime m_transitionTimer = DateTime.MinValue;

    #endregion

    #region Model reference fields

    public Material ControllerMaterial;

    public MeshRenderer RendererAxisLeft;
    public MeshRenderer RendererAxisRight;
    public MeshRenderer RendererButtonA;
    public MeshRenderer RendererButtonO;
    public MeshRenderer RendererButtonU;
    public MeshRenderer RendererButtonY;
    public MeshRenderer RendererDpadDown;
    public MeshRenderer RendererDpadLeft;
    public MeshRenderer RendererDpadRight;
    public MeshRenderer RendererDpadUp;
    public MeshRenderer RendererLB;
    public MeshRenderer RendererLT;
    public MeshRenderer RendererRB;
    public MeshRenderer RendererRT;

    #endregion

    private OuyaSDK.InputAction m_inputAction = OuyaSDK.InputAction.None;
    private OuyaSDK.KeyEnum m_keyEnum = OuyaSDK.KeyEnum.NONE;
    private float m_axisLeftStickX = 0f;
    private float m_axisLeftStickY = 0f;
    private float m_axisRightStickX = 0f;
    private float m_axisRightStickY = 0f;
    private float m_axisLeftTrigger = 0f;
    private float m_axisRightTrigger = 0f;

    void Awake()
    {
        #region Initialize OUYA

        try
        {
            OuyaSDK.initialize(DEVELOPER_ID);

            OuyaSDK.registerInputButtonListener(new OuyaSDK.InputButtonListener<OuyaSDK.InputButtonEvent>()
            {
                onSuccess = (OuyaSDK.InputButtonEvent inputEvent) =>
                {
                    Debug.Log(string.Format("inputButtonEvent onSuccess keyAction={0} keyCode={1}", inputEvent.getKeyAction(), inputEvent.getKeyCode()));
                    m_inputAction = inputEvent.getKeyAction();
                    m_keyEnum = inputEvent.getKeyCode();
                },

                onFailure = (int errorCode, string errorMessage) =>
                {
                    // Your app probably wants to do something more sophisticated than popping a Toast. This is
                    // here to tell you that your app needs to handle this case: if your app doesn't display
                    // something, the user won't know of the failure.
                    Debug.Log(string.Format("Could not fetch input (error {0}: {1})", errorCode, errorMessage));
                }
            });

            OuyaSDK.registerInputAxisListener(new OuyaSDK.InputAxisListener<OuyaSDK.InputAxisEvent>()
            {
                onSuccess = (OuyaSDK.InputAxisEvent inputEvent) =>
                {
                    Debug.Log(string.Format("inputAxisEvent onSuccess {0}", inputEvent));
                    Debug.Log(string.Format("inputAxisEvent onSuccess axisCode={0} axis={1}", inputEvent.getAxisCode(), inputEvent.getAxis()));
                    m_inputAction = inputEvent.getInputAction();
                    switch (inputEvent.getAxisCode())
                    {
                        case OuyaSDK.AxisEnum.AXIS_LSTICK_X:
                            m_axisLeftStickX = inputEvent.getAxis();
                            break;
                        case OuyaSDK.AxisEnum.AXIS_LSTICK_Y:
                            m_axisLeftStickY = inputEvent.getAxis();
                            break;
                        case OuyaSDK.AxisEnum.AXIS_RSTICK_X:
                            m_axisRightStickX = inputEvent.getAxis();
                            break;
                        case OuyaSDK.AxisEnum.AXIS_RSTICK_Y:
                            m_axisRightStickY = inputEvent.getAxis();
                            break;
                        case OuyaSDK.AxisEnum.AXIS_LTRIGGER:
                            m_axisLeftTrigger = inputEvent.getAxis();
                            break;
                        case OuyaSDK.AxisEnum.AXIS_RTRIGGER:
                            m_axisRightTrigger = inputEvent.getAxis();
                            break;
                    }
                },

                onFailure = (int errorCode, string errorMessage) =>
                {
                    // Your app probably wants to do something more sophisticated than popping a Toast. This is
                    // here to tell you that your app needs to handle this case: if your app doesn't display
                    // something, the user won't know of the failure.
                    Debug.Log(string.Format("Could not fetch input (error {0}: {1})", errorCode, errorMessage));
                }
            });

            #endregion

            #region Extra

            // Create material instances for independent highlights
            
            RendererAxisLeft.material = (Material)UnityEngine.Object.Instantiate(ControllerMaterial);
            RendererAxisRight.material = (Material)UnityEngine.Object.Instantiate(ControllerMaterial);
            RendererButtonA.material = (Material)UnityEngine.Object.Instantiate(ControllerMaterial);
            RendererButtonO.material = (Material)UnityEngine.Object.Instantiate(ControllerMaterial);
            RendererButtonU.material = (Material)UnityEngine.Object.Instantiate(ControllerMaterial);
            RendererButtonY.material = (Material)UnityEngine.Object.Instantiate(ControllerMaterial);
            RendererDpadDown.material = (Material)UnityEngine.Object.Instantiate(ControllerMaterial);
            RendererDpadLeft.material = (Material)UnityEngine.Object.Instantiate(ControllerMaterial);
            RendererDpadRight.material = (Material)UnityEngine.Object.Instantiate(ControllerMaterial);
            RendererDpadUp.material = (Material)UnityEngine.Object.Instantiate(ControllerMaterial);
            RendererLB.material = (Material)UnityEngine.Object.Instantiate(ControllerMaterial);
            RendererLT.material = (Material)UnityEngine.Object.Instantiate(ControllerMaterial);
            RendererRB.material = (Material)UnityEngine.Object.Instantiate(ControllerMaterial);
            RendererRT.material = (Material)UnityEngine.Object.Instantiate(ControllerMaterial);

            #endregion
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("Awake exception={0}", ex));
        }
    }

    #region Presentation

    private void OnGUI()
    {
        try
        {
           // UpdateCamera();

            UpdateController();

            OuyaNGUIHandler nguiHandler = GameObject.Find("_NGUIHandler").GetComponent<OuyaNGUIHandler>();
            if (nguiHandler != null)
            {
                string[] joystickNames = Input.GetJoystickNames();
                //foreach (string joystick in Input.GetJoystickNames())
                for (int i = 0; i < joystickNames.Length; i++)
                {
                    if (i == 1) { nguiHandler.controller1.text = joystickNames[i]; }
                    if (i == 2) { nguiHandler.controller1.text = joystickNames[i]; }
                    if (i == 3) { nguiHandler.controller1.text = joystickNames[i]; }
                    if (i == 4) { nguiHandler.controller1.text = joystickNames[i]; }
                }

                nguiHandler.genericEvent.text = m_inputAction + " : " + m_keyEnum;
                nguiHandler.joystickLeft.text = string.Format("{0},{1}", m_axisLeftStickX, m_axisLeftStickY);
                nguiHandler.joystickRight.text = string.Format("{0},{1}", m_axisRightStickX, m_axisRightStickY);
                nguiHandler.trigger.text = string.Format("{0},{1}", m_axisLeftTrigger, m_axisRightTrigger);
            }
        }
        catch (System.Exception)
        {
        }
    }

    #endregion

    #region Extra

    private const float TRANSITION_RATE = 4.0f;

    private void UpdateCamera()
    {
        //move the camera around
        float t = 0f;
        if (m_transitionTimer < DateTime.Now)
        {
            m_transitionTimer = DateTime.Now + TimeSpan.FromSeconds(TRANSITION_RATE);
            m_oldTransitionId = m_transitionId;
            m_transitionId = UnityEngine.Random.Range(0, CameraPositions.Length);
        }
        else
        {
            t = 1f - (float) ((m_transitionTimer - DateTime.Now).TotalSeconds)/TRANSITION_RATE;
        }

        Camera.main.transform.position = Vector3.Lerp(CameraPositions[m_oldTransitionId].position,
                                                      CameraPositions[m_transitionId].position, t);
        Camera.main.transform.rotation = Quaternion.Lerp(CameraPositions[m_oldTransitionId].rotation,
                                                         CameraPositions[m_transitionId].rotation, t);
    }

    private void UpdateHighlight(MeshRenderer mr, bool highlight)
    {
        if (highlight)
        {
            Color c = new Color(0, 10, 0, 1);
            mr.material.color = Color.Lerp(mr.material.color, c, Time.deltaTime * 10);
        }
        else
        {
            mr.material.color = Color.Lerp(mr.material.color, Color.white, Time.deltaTime);
        }
    }

    private void UpdateController()
    {
        UpdateHighlight(RendererAxisLeft, Mathf.Abs(m_axisLeftStickX) > 0.25f ||
            Mathf.Abs(m_axisLeftStickY) > 0.25f);

        RendererAxisLeft.transform.localRotation = Quaternion.Euler(m_axisLeftStickY * 15, 0, m_axisLeftStickX * 15);

        UpdateHighlight(RendererAxisRight, Mathf.Abs(m_axisRightStickX) > 0.25f ||
            Mathf.Abs(m_axisRightStickY) > 0.25f);

        RendererAxisRight.transform.localRotation = Quaternion.Euler(m_axisRightStickY * 15, 0, m_axisRightStickX * 15);

        UpdateHighlight(RendererLT, Mathf.Abs(m_axisLeftTrigger) > 0.25f);

        RendererLT.transform.localRotation = Quaternion.Euler(m_axisLeftTrigger * -15, 0, 0);

        UpdateHighlight(RendererRT, Mathf.Abs(m_axisRightTrigger) > 0.25f);

        RendererRT.transform.localRotation = Quaternion.Euler(m_axisRightTrigger * -15, 0, 0);

        UpdateHighlight(RendererButtonA, m_inputAction == OuyaSDK.InputAction.KeyDown && m_keyEnum == OuyaSDK.KeyEnum.BUTTON_A);
        UpdateHighlight(RendererButtonO, m_inputAction == OuyaSDK.InputAction.KeyDown && m_keyEnum == OuyaSDK.KeyEnum.BUTTON_O);
        UpdateHighlight(RendererButtonU, m_inputAction == OuyaSDK.InputAction.KeyDown && m_keyEnum == OuyaSDK.KeyEnum.BUTTON_U);
        UpdateHighlight(RendererButtonY, m_inputAction == OuyaSDK.InputAction.KeyDown && m_keyEnum == OuyaSDK.KeyEnum.BUTTON_Y);
        UpdateHighlight(RendererDpadDown, m_keyEnum == OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN);
        UpdateHighlight(RendererDpadLeft, m_keyEnum == OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT);
        UpdateHighlight(RendererDpadRight, m_keyEnum == OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT);
        UpdateHighlight(RendererDpadUp, m_keyEnum == OuyaSDK.KeyEnum.BUTTON_DPAD_UP);
        UpdateHighlight(RendererLB, m_inputAction == OuyaSDK.InputAction.KeyDown && m_keyEnum == OuyaSDK.KeyEnum.BUTTON_LB);
        UpdateHighlight(RendererLT, m_inputAction == OuyaSDK.InputAction.KeyDown && m_keyEnum == OuyaSDK.KeyEnum.BUTTON_LT);
        UpdateHighlight(RendererRB, m_inputAction == OuyaSDK.InputAction.KeyDown && m_keyEnum == OuyaSDK.KeyEnum.BUTTON_RB);
        UpdateHighlight(RendererRT, m_inputAction == OuyaSDK.InputAction.KeyDown && m_keyEnum == OuyaSDK.KeyEnum.BUTTON_RT);
    }

    #endregion
}
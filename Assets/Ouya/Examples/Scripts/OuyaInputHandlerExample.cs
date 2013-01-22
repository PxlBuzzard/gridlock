using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OuyaInputHandlerExample : MonoBehaviour
{
    [SerializeField]
    public OuyaSDK.OuyaPlayer player;
    public float speed = 6.0F;
    public float gravity = 20.0F;
    public float turnSpeed = 60f;
    private Vector3 moveDirection = Vector3.zero;

    private const string DEVELOPER_ID = "310a8f51-4d6e-4ae5-bda0-b93878e5f5d0";

    private CharacterController controller;
    private bool isBattleStance = false; 

    void Awake()
    {
        OuyaInputManager.OuyaButtonEvent.addButtonEventListener(HandleButtonEvent);
        
        //Get our character controller;
        controller = GetComponent<CharacterController>();

        /*
         * Note:  Only 1 Script has to do this in your entire scene.  This is here for example only, and you should probably put the 
         * following intialization code in another game object in your scene.
         */
        try
        {
            //First Initalize Ouya SDK;
            #region Initialize OUYA

            //Initialize OuyaSDK with your developer ID
            //Get your developer_id from the ouya developer portal @ http://developer.ouya.tv
            OuyaSDK.initialize(DEVELOPER_ID);

            //Register the for Button Input handling through the static class 
            OuyaSDK.registerInputButtonListener(new OuyaSDK.InputButtonListener<OuyaSDK.InputButtonEvent>()
            {
                onSuccess = (OuyaSDK.InputButtonEvent inputEvent) =>
                {
                    //Assign our handler in the static class.
                    OuyaInputManager.HandleButtonEvent(inputEvent);
                },

                onFailure = (int errorCode, string errorMessage) =>
                {
                    // Your app probably wants to do something more sophisticated than popping a Toast. This is
                    // here to tell you that your app needs to handle this case: if your app doesn't display
                    // something, the user won't know of the failure.
                    //Debug.Log(string.Format("Could not fetch input (error {0}: {1})", errorCode, errorMessage));
                }
            });
            
            //Register the for Axis Input handling through the static class
            OuyaSDK.registerInputAxisListener(new OuyaSDK.InputAxisListener<OuyaSDK.InputAxisEvent>()
            {
                onSuccess = (OuyaSDK.InputAxisEvent inputEvent) =>
                {
                    //Assign our handler in the static class.
                    OuyaInputManager.HandleAxisEvent(inputEvent);
                },

                onFailure = (int errorCode, string errorMessage) =>
                {
                    // Your app probably wants to do something more sophisticated than popping a Toast. This is
                    // here to tell you that your app needs to handle this case: if your app doesn't display
                    // something, the user won't know of the failure.
                    //Debug.Log(string.Format("Could not fetch input (error {0}: {1})", errorCode, errorMessage));
                }
            });

            #endregion
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("Awake exception={0}", ex));
        }
        //END: Note.
        
    }

    void Update()
    {

        //Get Joystick points and calculate deadzone.
        Vector2 point = convertRawJoystickCoordinates(OuyaInputManager.GetAxis("LX", player), OuyaInputManager.GetAxis("LY", player), .25f);

        //Rotate character to where it is looking.
        transform.Rotate(0f, point.x * turnSpeed * Time.deltaTime, 0f);

        //only move if we are grounded.
        if (controller.isGrounded)
        {
            //get the move direction from the controller axis;
            moveDirection = transform.forward * -point.y;

            //Set the speed
            moveDirection *= speed;
        }

        //Apply any gravity factors
        moveDirection.y -= gravity * Time.deltaTime;

        //Move the character;
        controller.Move(moveDirection * Time.deltaTime);

        //Handle animation if we are moving.
        if (point.normalized.x != 0 && point.normalized.y != 0)
        {
            //If axis is not 0 then play run animation.
            this.animation.Play("run");
        }

        //if no animation is playing, then play the idle animation.
        if (!this.animation.isPlaying)
        {
            string anim = "idle";
            if (isBattleStance)
            {
                anim = "waitingforbattle";
            }
            this.animation.Play(anim);
        }
    }
    
    void HandleButtonEvent(OuyaSDK.OuyaPlayer p, OuyaSDK.KeyEnum b, OuyaSDK.InputAction bs)
    {
        if (!player.Equals(p)) { return; }

        if (b.Equals(OuyaSDK.KeyEnum.BUTTON_O) && bs.Equals(OuyaSDK.InputAction.KeyDown))
        {
            this.animation.Play("attack");
            Debug.Log("Button O Down Event was triggered on Player" + player);
        }

        if (b.Equals(OuyaSDK.KeyEnum.BUTTON_O) && bs.Equals(OuyaSDK.InputAction.KeyUp))
        {
            Debug.Log("Button O Up Event was triggered on Player" + player);
        }

        //BATTLE STANCE:
        if (b.Equals(OuyaSDK.KeyEnum.BUTTON_Y) && bs.Equals(OuyaSDK.InputAction.KeyDown))
        {
            if (!isBattleStance)
            {
                //Set Battle stance.
                this.animation.Play("waitingforbattle");
                isBattleStance = true;
            }
            else
            {
                if (this.animation.IsPlaying("waitingforbattle"))
                {
                    this.animation.Play("idle");
                    isBattleStance = false;
                }
            }
        }

        //DANCE:
        if (b.Equals(OuyaSDK.KeyEnum.BUTTON_U) && bs.Equals(OuyaSDK.InputAction.KeyDown))
        {
            this.animation.Play("dance");
        }


        //FAKE Death:
        if (b.Equals(OuyaSDK.KeyEnum.BUTTON_A) && bs.Equals(OuyaSDK.InputAction.KeyDown))
        {
            this.animation.Play("die");
        }
    }

    private Vector2 convertRawJoystickCoordinates(float x, float y, float deadzoneRadius)
    {

        Vector2 result = new Vector2(x, y); // a class with just two members, int x and int y
        bool isInDeadzone = testIfRawCoordinatesAreInDeadzone(x, y, deadzoneRadius);
        if (isInDeadzone)
        {
            result.x = 0f;
            result.y = 0f;
        }
        return result;
    }

    private bool testIfRawCoordinatesAreInDeadzone(float x, float y, float radius)
    {
        float distance = Mathf.Sqrt((x * x) + (y * y));
        return distance < radius;
    }

}

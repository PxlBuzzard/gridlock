using UnityEngine;
using System.Collections;

public class InputInitializer: MonoBehaviour {
	
	private string developerID = "37c5a4ae-caa6-4e13-8f7b-bb0e7b532f6c";
	
	void Awake() {
		try
		{
			OuyaSDK.initialize(developerID);
			
			OuyaSDK.registerInputButtonListener( new OuyaSDK.InputButtonListener<OuyaSDK.InputButtonEvent>()
			{
				onSuccess = (OuyaSDK.InputButtonEvent inputEvent) =>
				{
					OuyaInputManager.HandleButtonEvent(inputEvent);
					print("INITIALIZED BUTTON EVENTS");
				},
				
				onFailure = (int ErrorCode, string errorMessage) =>
				{
					// failure code
					print("FAILED BUTTON INITIALIZE");
				}
			});
			
			OuyaSDK.registerInputAxisListener( new OuyaSDK.InputAxisListener<OuyaSDK.InputAxisEvent>()
			{
				onSuccess = (OuyaSDK.InputAxisEvent inputEvent) =>
				{
					OuyaInputManager.HandleAxisEvent(inputEvent);
					print("AXIS INITIALIZE SUCCEDED");
				},
				
				onFailure = (int ErrorCode, string errorMessage) =>
				{
					// failure code
					print("FAILURE ON BUTTON INITIALIZE");
				}
			});
		} catch (System.Exception ex) {
			// failure...
			print("Failure..............");
		}
	}	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

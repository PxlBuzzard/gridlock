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
				},
				
				onFailure = (int ErrorCode, string errorMessage) =>
				{
					// failure code
				}
			});
			
			OuyaSDK.registerInputAxisListener( new OuyaSDK.InputAxisListener<OuyaSDK.InputAxisEvent>()
			{
				onSuccess = (OuyaSDK.InputAxisEvent inputEvent) =>
				{
					OuyaInputManager.HandleAxisEvent(inputEvent);
				},
				
				onFailure = (int ErrorCode, string errorMessage) =>
				{
					// failure code
				}
			});
		} catch (System.Exception ex) {
			// failure...
			print("Failure.............." + ex.Message);
		}
	}	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

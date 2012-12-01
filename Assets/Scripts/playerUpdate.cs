using UnityEngine;
using System.Collections;

public class playerUpdate : MonoBehaviour {
	
	
	public const int MOVE_SPEED = 5;
	public string lastDirection;
	public OTAnimatingSprite player;
	
	// Use this for initialization
	void Start () 
	{	
    	player = OT.ObjectByName("player") as OTAnimatingSprite;
		lastDirection = "Down";
	}
	
	// Update is called once per frame
	void Update () 
	{
	   	string currentDirection = "";
		
		if (Input.GetKey ("left"))
		{
	        transform.Translate(-MOVE_SPEED * Time.deltaTime,0,0);	
			currentDirection += "Left";
		}
		else if (Input.GetKey ("right"))
		{
	        transform.Translate(MOVE_SPEED * Time.deltaTime,0,0);
			currentDirection += "Right";
		}
			
		if (Input.GetKey ("down"))
		{
	        transform.Translate(0,-MOVE_SPEED * Time.deltaTime,0);	
			currentDirection += "Down";
		}
		else if (Input.GetKey ("up"))
		{
	        transform.Translate(0,MOVE_SPEED * Time.deltaTime,0);
			currentDirection += "Up";
		}
		
		if(currentDirection == "")
		{
			player.PlayLoop(lastDirection + "Static");
		}
		else
		{
			player.PlayLoop(currentDirection);
			lastDirection = currentDirection;
		}
			
		
		print ("Direction: " + lastDirection);
	}
}

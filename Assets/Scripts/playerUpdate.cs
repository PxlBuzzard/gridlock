using UnityEngine;
using System.Collections;

public class playerUpdate : Photon.MonoBehaviour {
	
	//public const int NUM_BULLETS = 20;
	private const float ANIMATE_THRESHOLD = 0.075f;
	public const int MOVE_SPEED = 5;
	public string lastDirection;
	public OTAnimatingSprite player;
	public bulletManager theBulletManager;
	
	Vector3 correctPos = Vector3.zero;
	
	private bool isFiring;
	
	// Use this for initialization
	void Start () 
	{	
		lastDirection = "Down";
		player.spriteContainer = OT.ContainerByName("PlayerSheet");
		
		player.animation = OT.AnimationByName("PlayerAnim");
		
		for (int i = 0; i < player.animation.framesets.Length; i++)
			player.animation.framesets[i].container = player.spriteContainer;
		
		//print("made character");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (photonView.isMine)
		{
		   	string currentDirection = "";
			string currentAimingDirection = "";
			//move player
			if (Input.GetButton("Move Left"))
			{
		        transform.Translate(-MOVE_SPEED * Time.deltaTime,0,0);	
				currentDirection += "Left";
			}
			else if (Input.GetButton("Move Right"))
			{
		        transform.Translate(MOVE_SPEED * Time.deltaTime,0,0);
				currentDirection += "Right";
			}
				
			if (Input.GetButton("Move Down"))
			{
		        transform.Translate(0,-MOVE_SPEED * Time.deltaTime,0);	
				currentDirection += "Down";
			}
			else if (Input.GetButton("Move Up"))
			{
		        transform.Translate(0,MOVE_SPEED * Time.deltaTime,0);
				currentDirection += "Up";
			}
			
			// Player aiming
			if (Input.GetButton("Aim Left"))
			{	
				currentAimingDirection += "Left";
			}
			else if (Input.GetButton("Aim Right"))
			{
				currentAimingDirection += "Right";
			}
				
			if (Input.GetButton("Aim Down"))
			{
				currentAimingDirection += "Down";
			}
			else if (Input.GetButton("Aim Up"))
			{
				currentAimingDirection += "Up";
			}
			
			if(currentDirection == "")
			{
				if (currentAimingDirection == "")
				{
					player.PlayLoop(lastDirection + "Static");
				}
				else
				{
					player.PlayLoop(currentAimingDirection  + "Static");
				}
			}
			else
			{
				if (currentAimingDirection == "")
				{
					player.PlayLoop(currentDirection);
					lastDirection = currentDirection;
				}
				else
				{
					player.PlayLoop(currentAimingDirection);
					lastDirection = currentAimingDirection;
				}
			}
			
			//fire bullets
			if(Input.GetKey ("space"))
			{
				isFiring = true;
			}
			else
			{
				isFiring = false;
			}
			
			//update bullets
			theBulletManager.Update();
			
			OT.view.position = new Vector2(player.position.x, player.position.y);
		}
		else
		{
			transform.position = Vector3.Lerp(transform.position, correctPos, Time.deltaTime * 5);
			
			//make the player animate till they come to a stop
			if (Mathf.Abs(transform.position.x - correctPos.x) <= ANIMATE_THRESHOLD && Mathf.Abs(transform.position.y - correctPos.y) <= ANIMATE_THRESHOLD)
			{
				player.PlayLoop(lastDirection + "Static");
			}
			else
			{
				player.PlayLoop(lastDirection);
			}
			//print (lastDirection);
			
			
		}
		
		if (isFiring)
		{
			theBulletManager.Fire(player);
		}
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			if (photonView.isMine)
			{
				stream.SendNext(transform.position);
				stream.SendNext(lastDirection);
				stream.SendNext(isFiring);
			}
		}
		else
		{
			if (!photonView.isMine)
			{
				correctPos = (Vector3)stream.ReceiveNext();
				lastDirection = (string)stream.ReceiveNext();
				isFiring = (bool)stream.ReceiveNext();
			}
		}
	}
}

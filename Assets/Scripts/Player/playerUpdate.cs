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
	
	// Use this for initialization
	void Start () 
	{	
		lastDirection = "Down";
		player.spriteContainer = OT.ContainerByName("PlayerSheet");
		
		player.animation = OT.AnimationByName("PlayerAnim");
		
		for (int i = 0; i < player.animation.framesets.Length; i++)
			player.animation.framesets[i].container = player.spriteContainer;
		
		print("made character");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (photonView.isMine)
		{
		   	string currentDirection = "";
			
			//move player
			if (Input.GetKey ("left") || Input.GetKey ("a"))
			{
		        transform.Translate(-MOVE_SPEED * Time.deltaTime,0,0);	
				currentDirection += "Left";
			}
			else if (Input.GetKey ("right") || Input.GetKey ("d"))
			{
		        transform.Translate(MOVE_SPEED * Time.deltaTime,0,0);
				currentDirection += "Right";
			}
				
			if (Input.GetKey ("down") || Input.GetKey ("s"))
			{
		        transform.Translate(0,-MOVE_SPEED * Time.deltaTime,0);	
				currentDirection += "Down";
			}
			else if (Input.GetKey ("up") || Input.GetKey ("w"))
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
			
			//fire bullets
			if(Input.GetKey ("space"))
			{
				theBulletManager.Fire(player);
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
			print (lastDirection);
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
			}
		}
		else
		{
			if (!photonView.isMine)
			{
				correctPos = (Vector3)stream.ReceiveNext();
				lastDirection = (string)stream.ReceiveNext();
			}
		}
	}
}

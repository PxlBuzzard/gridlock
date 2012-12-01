using UnityEngine;
using System.Collections;

public class playerUpdate : Photon.MonoBehaviour {
	
	//public const int NUM_BULLETS = 20;
	public const int MOVE_SPEED = 5;
	public string lastDirection;
	public OTAnimatingSprite player;
	public bulletManager theBulletManager;
	public OTAnimation PlayerAnimPrefab;
	public OTSpriteSheet PlayerSheetPrefab;
	
	Vector3 correctPos = Vector3.zero;
	
	// Use this for initialization
	void Start () 
	{	
		lastDirection = "Down";
		player.spriteContainer = (OTSpriteSheet)Instantiate(PlayerSheetPrefab);
		
		player.animation = (OTAnimation)Instantiate(PlayerAnimPrefab);
		
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
			

			
			if(Input.GetKey ("space"))
			{
				theBulletManager.Fire(player);
			}
		}
		else
		{
			transform.position = Vector3.Lerp(transform.position, correctPos, Time.deltaTime * 5);
			player.PlayLoop(lastDirection);
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

using UnityEngine;
using System.Collections;

public class playerUpdate : Photon.MonoBehaviour {
	
	//public const int NUM_BULLETS = 20;
	private const float ANIMATE_THRESHOLD = 0.075f;
	public const int MOVE_SPEED = 5;
	public string lastDirection;
	public OTAnimatingSprite player;
	public bulletManager theBulletManager;
	public healthBar theHealthBar;
	
	public int maxHealth;
	public int currentHealth;
	
	Vector3 correctPos = Vector3.zero;
	
	private bool isFiring;
	
	private string playerNum;
	
	// Use this for initialization
	void Start () 
	{	
		lastDirection = "Down";
		player.spriteContainer = OT.ContainerByName("PlayerSheet");
		
		player.animation = OT.AnimationByName("PlayerAnim");
		
		for (int i = 0; i < player.animation.framesets.Length; i++)
			player.animation.framesets[i].container = player.spriteContainer;
		
		playerNum = "P1";
		
		maxHealth = 75;
		currentHealth = maxHealth;
		theHealthBar.maxHealth = maxHealth;
		theHealthBar.currentHealth = maxHealth;
		theBulletManager.player = player;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (photonView.isMine)
		{
		   	string currentDirection = "";
			string currentAimingDirection = "";
			
			//move player
			if (Input.GetAxis(playerNum + "Horizontal") < -0.3)
			{
		        transform.Translate(Input.GetAxis(playerNum + "Horizontal") * MOVE_SPEED * Time.deltaTime,0,0);	
				currentDirection += "Left";
			}
			else if (Input.GetAxis(playerNum + "Horizontal") > 0.3)
			{
		        transform.Translate(Input.GetAxis(playerNum + "Horizontal") * MOVE_SPEED * Time.deltaTime,0,0);
				currentDirection += "Right";
			}
				
			if (Input.GetAxis(playerNum + "Vertical") < -0.3)
			{
		        transform.Translate(0,Input.GetAxis(playerNum + "Vertical") * MOVE_SPEED * Time.deltaTime,0);	
				currentDirection += "Down";
			}
			else if (Input.GetAxis(playerNum + "Vertical") > 0.3)
			{
		        transform.Translate(0,Input.GetAxis(playerNum + "Vertical") * MOVE_SPEED * Time.deltaTime,0);
				currentDirection += "Up";
			}
			
			// Player aiming
			if (Input.GetAxis(playerNum + "Horizontal Aiming") < -0.5)
			{	
				currentAimingDirection += "Left";
			}
			else if (Input.GetAxis(playerNum + "Horizontal Aiming") > 0.5)
			{
				currentAimingDirection += "Right";
			}
				
			if (Input.GetAxis(playerNum + "Vertical Aiming") < -0.5)
			{
				currentAimingDirection += "Down";
			}
			else if (Input.GetAxis(playerNum + "Vertical Aiming") > 0.5)
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
					lastDirection = currentAimingDirection;
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
			if(Input.GetAxis(playerNum + "Shoot") < -0.04 || Input.GetButton(playerNum + "Shoot"))
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
			
			theHealthBar.AdjustCurrentHealth(currentHealth);
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
				stream.SendNext(currentHealth);
			}
		}
		else
		{
			if (!photonView.isMine)
			{
				correctPos = (Vector3)stream.ReceiveNext();
				lastDirection = (string)stream.ReceiveNext();
				isFiring = (bool)stream.ReceiveNext();
				currentHealth = (int)stream.ReceiveNext();
			}
		}
	}
	
	public void DeductHealth(int damage)
	{
		if (photonView.isMine)
		{
			currentHealth -= damage;
			
			if(currentHealth <= 0)
			{
				currentHealth = maxHealth;
				player.position = Vector2.zero;
			}
			
			theHealthBar.AdjustCurrentHealth(currentHealth);
		}
	}
}

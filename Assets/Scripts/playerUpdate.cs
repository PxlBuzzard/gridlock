using UnityEngine;
using System.Collections;

/// <summary>
/// Main player class.
/// </summary>
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
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () 
	{	
		lastDirection = "Down";
		player.spriteContainer = OT.ContainerByName("PlayerSheet");
		
		player.animation = OT.AnimationByName("PlayerAnim");
		
		for (int i = 0; i < player.animation.framesets.Length; i++)
			player.animation.framesets[i].container = player.spriteContainer;
		
		//set player number for controller purposes
		playerNum = "P1";
		
		maxHealth = 75;
		currentHealth = maxHealth;
		theHealthBar.maxHealth = maxHealth;
		theHealthBar.currentHealth = maxHealth;
		theBulletManager.player = player;
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
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
	
	/// <summary>
	/// Compacts data to send it to other players.
	/// </summary>
	/// <param name='stream'>
	/// Stream.
	/// </param>
	/// <param name='info'>
	/// Info.
	/// </param>
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
	
	/// <summary>
	/// Deducts the health.
	/// </summary>
	/// <param name='damage'>
	/// Amount to damage the player by. (should be positive)
	/// </param>
	public void DeductHealth(int damage)
	{
		if (photonView.isMine)
		{
			currentHealth -= damage;
			
			//kill the player if he drops below 0 HP
			if(currentHealth <= 0)
				KillPlayer();
			
			theHealthBar.AdjustCurrentHealth(currentHealth);
		}
	}
	
	/// <summary>
	/// Kills the player, respawns, and coin explosion.
	/// </summary>
	public void KillPlayer()
	{
		currentHealth = maxHealth;
		player.position = Vector2.zero;
		CoinExplosion(10);
	}
	
	/// <summary>
	/// ITS A COIN ASPOLSION UP IN HURR
	/// </summary>
	/// <param name='numCoins'>
	/// Number of coins to make explode.
	/// </param>
	public void CoinExplosion(uint numCoins)
	{
		for (int i = 0; i < numCoins; i++)
		{
			//instantiate a coin prefab	
			//prefab.Explode();
		}
	}
}
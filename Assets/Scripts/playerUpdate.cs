using UnityEngine;
using System.Collections;

/// <summary>
/// Main player class.
/// </summary>
public class playerUpdate : Photon.MonoBehaviour {
	
	private const float ANIMATE_THRESHOLD = 0.075f;
	public const int MOVE_SPEED = 5;
	public string lastDirection;
	public OTAnimatingSprite player;
	public bulletManager theBulletManager;
	public healthBar theHealthBar;
	
	public int numCoins;
	public coinManager theCoinManager;
	
	public int maxHealth;
	public int currentHealth;
	
	Vector3 correctPos = Vector3.zero;
	
	private bool isFiring;
	private bool isDead;
	
	public string playerNum;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start() 
	{	
		//this should be read from a save file
		numCoins = 0;
		
		lastDirection = "Down";
		player.spriteContainer = OT.ContainerByName("PlayerSheet");
		
		player.animation = OT.AnimationByName("PlayerAnim");
		
		for (int i = 0; i < player.animation.framesets.Length; i++)
			player.animation.framesets[i].container = player.spriteContainer;
		
		maxHealth = 300;
		isDead = false;
		currentHealth = maxHealth;
		theHealthBar.maxHealth = maxHealth;
		theHealthBar.currentHealth = maxHealth;
		theBulletManager.player = player;
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update() 
	{
		if (photonView.isMine && !isDead)
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
			else if (!isDead)
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
			isFiring = (Input.GetAxis(playerNum + "Shoot") < -0.04 || Input.GetButton(playerNum + "Shoot"));
			
			//update bullets
			theBulletManager.Update();
			
		//	OT.view.position = new Vector2(player.position.x, player.position.y);
		}
		else if (!photonView.isMine)
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
		
		if (isDead)
		{
			player.alpha -= .005f;
			theHealthBar.barOpacity(.005f, true);
		}
		
		DeductHealth(1);
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
		if (photonView.isMine && !isDead)
		{
			currentHealth -= damage;
			
			//kill the player if he drops below 0 HP
			if(currentHealth <= 0)
				StartCoroutine(KillPlayer());
			
			theHealthBar.AdjustCurrentHealth(currentHealth);
		}
		
		if (!isDead)
		{
			StopCoroutine("HealthFlash");
			StartCoroutine(HealthFlash());
		}
	}
	
	/// <summary>
	/// Flashes the player red when shot.
	/// </summary>
	IEnumerator HealthFlash()
	{
		player.tintColor = Color.red;
		
		yield return new WaitForSeconds(.2f);
		
		player.tintColor = Color.white;
	}
	
	/// <summary>
	/// Kills the player, respawns, and coin explosion.
	/// </summary>
	IEnumerator KillPlayer()
	{
		CoinExplosion(10);
		player.alpha = .5f;
		theHealthBar.barOpacity(.5f, true);
		isDead = true;
		isFiring = false;
		player.collidable = false;
		player.PlayLoop(lastDirection + "Static");
			
		yield return new WaitForSeconds(3f);
		
		Respawn();
	}
	
	/// <summary>
	/// Respawn the player.
	/// </summary>
	void Respawn()
	{
		player.alpha = 1;
		isDead = false;
		theHealthBar.AdjustCurrentHealth(maxHealth);
		theHealthBar.barOpacity(1f, false);
		player.collidable = true;
		
		//make player invincible for 3 seconds after spawn
		currentHealth = maxHealth;
		
		//switch to respawn location
		player.position = Vector2.zero;
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
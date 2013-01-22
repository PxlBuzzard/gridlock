using UnityEngine;
using System.Collections;

/// <summary>
/// Main player class.
/// </summary>
public class playerUpdate : Photon.MonoBehaviour {
	
	private const float ANIMATE_THRESHOLD = 0.075f;
	public const int MOVE_SPEED = 5;
	public const int VERT_MOVE_SPEED = 3;
	public const int DASH_SPEED = 20;
	public string lastDirection;
	public string dashDirection;
	public OTAnimatingSprite player;
	public OTAnimatingSprite gun;
	public OTTileMap map;
	public bulletManager theBulletManager;
	public healthBar theHealthBar;
	
	private OTSound gunShot;
	
	public int numCoins;
	public coinManager theCoinManager;
	
	public int maxHealth;
	public int currentHealth;
	
	Vector3 correctPos = Vector3.zero;
	
	private bool isFiring;
	private bool isDead;
	private Timer dashTimer = new Timer();
	private Timer dashCooldownTimer = new Timer();
	
	public string playerNum;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start() 
	{	
		//this should be read from a save file
		numCoins = 0;
		
		lastDirection = "Down";
		
		
		// DO NOT USE Instantiate(prefab, startPosition, QUATERNION.IDENTITY);  (Resets transform of prefabs to the initial prefab state)
		// Working instantiation off of photon network
		gun = (Instantiate(Resources.Load("GunPrefab")) as GameObject).GetComponent<OTAnimatingSprite>();
		// Working instantiation on photon network
		//GameObject gun_test = PhotonNetwork.Instantiate("GunPrefab", Vector3.zero, Quaternion.identity, 0);
		//gun = gun_test.GetComponent<OTAnimatingSprite>();
		
		player.spriteContainer = OT.ContainerByName("PlayerSheet");
		gun.spriteContainer = OT.ContainerByName("GunSheet");
		
		player.animation = OT.AnimationByName("PlayerAnim");
		gun.animation = OT.AnimationByName("GunAnim");
		
		for (int i = 0; i < player.animation.framesets.Length; i++)
			player.animation.framesets[i].container = player.spriteContainer;
		for (int i = 0; i < gun.animation.framesets.Length; i++)
			gun.animation.framesets[i].container = gun.spriteContainer;
		
		gun.transform.localScale = new Vector3(1.185185f, 1.185185f, 1);
		gun.depth = -10;
		
		maxHealth = 300;
		isDead = false;
		currentHealth = maxHealth;
		theHealthBar.maxHealth = maxHealth;
		theHealthBar.currentHealth = maxHealth;
		theBulletManager.player = player;
		
		gunShot = new OTSound("gunShot");
		gunShot.Volume(.2f);
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update() 
	{	
		//gun.transform.position = new Vector3(player.position.x + .1f, player.position.y - .1f, gun.transform.position.z);
		
		if (photonView.isMine && !isDead)
		{
		   	string currentDirection = "";
			string currentAimingDirection = "";
			
			if(!dashCooldownTimer.isRunning && !dashTimer.isRunning && Input.GetKey(KeyCode.E))
			{
				dashDirection = lastDirection;
				dashTimer.Countdown(.2f);
			}
			dashTimer.Update();
			if(dashTimer.isFinished)
			{
				dashTimer.Reset();
				dashCooldownTimer.Countdown(3f);	
			}
			dashCooldownTimer.Update();
			if(dashCooldownTimer.isFinished)
			{
				dashCooldownTimer.Reset();	
			}
			
			// Player moving
			if(dashTimer.isRunning)
			{
				dashPlayer();
			}
			else
			{
				currentDirection = movePlayer(currentDirection);
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
			
			if(currentDirection == "" && !dashTimer.isRunning)
			{
				if (currentAimingDirection == "")
				{
					player.PlayLoop(lastDirection + "Static");
					gun.PlayLoop(lastDirection + "Static");
				}
				else
				{
					player.PlayLoop(currentAimingDirection  + "Static");
					gun.PlayLoop(currentAimingDirection  + "Static");
					lastDirection = currentAimingDirection;
				}
			}
			else if (!isDead)
			{
				if (currentAimingDirection == "" && !dashTimer.isRunning)
				{
					player.PlayLoop(currentDirection);
					gun.PlayLoop(currentDirection);
					lastDirection = currentDirection;
				}
				else if(!(currentAimingDirection == ""))
				{
					player.PlayLoop(currentAimingDirection);
					gun.PlayLoop(currentAimingDirection);
					lastDirection = currentAimingDirection;
				}
			}
			
			//fire bullets
			isFiring = (Input.GetAxis(playerNum + "Shoot") < -0.04 || Input.GetButton(playerNum + "Shoot"));
			if(!dashTimer.isRunning)
			{
				isFiring = (Input.GetAxis(playerNum + "Shoot") < -0.04 || Input.GetButton(playerNum + "Shoot"));
			}
			else
			{
				isFiring = false;	
			}
		}
		else if (!photonView.isMine)
		{			
			player.transform.position = Vector3.Lerp(player.transform.position, correctPos, Time.deltaTime * 5);
			
			//make the player animate till they come to a stop
			if (Mathf.Abs(player.transform.position.x - correctPos.x) <= ANIMATE_THRESHOLD && Mathf.Abs(player.transform.position.y - correctPos.y) <= ANIMATE_THRESHOLD)
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
			gunShot.Play(true);
			theBulletManager.Fire();
		}
		
		if (isDead)
		{
			player.alpha -= .005f;
			theHealthBar.barOpacity(.005f, true);
		}
	}
	
	string movePlayer(string currentDirection)
	{
		//move player
		if (Input.GetAxis(playerNum + "Horizontal") < -0.3)
		{
	        player.transform.Translate(Input.GetAxis(playerNum + "Horizontal") * MOVE_SPEED * Time.deltaTime,0,0);
			gun.transform.Translate(Input.GetAxis(playerNum + "Horizontal") * MOVE_SPEED * Time.deltaTime,0,0);
			currentDirection += "Left";
		}
		else if (Input.GetAxis(playerNum + "Horizontal") > 0.3)
		{
	        player.transform.Translate(Input.GetAxis(playerNum + "Horizontal") * MOVE_SPEED * Time.deltaTime,0,0);
			gun.transform.Translate(Input.GetAxis(playerNum + "Horizontal") * MOVE_SPEED * Time.deltaTime,0,0);
			currentDirection += "Right";
		}
			
		if (Input.GetAxis(playerNum + "Vertical") < -0.3)
		{
	        player.transform.Translate(0,Input.GetAxis(playerNum + "Vertical") * VERT_MOVE_SPEED * Time.deltaTime,0);
			gun.transform.Translate(0,Input.GetAxis(playerNum + "Vertical") * VERT_MOVE_SPEED * Time.deltaTime,0);
			currentDirection += "Down";
		}
		else if (Input.GetAxis(playerNum + "Vertical") > 0.3)
		{
	        player.transform.Translate(0,Input.GetAxis(playerNum + "Vertical") * VERT_MOVE_SPEED * Time.deltaTime,0);
			gun.transform.Translate(0,Input.GetAxis(playerNum + "Vertical") * VERT_MOVE_SPEED * Time.deltaTime,0);
			currentDirection += "Up";
		}
		
		return currentDirection;
	}
	
	void dashPlayer()
	{
		if(dashDirection.Contains("Left"))
		{
			player.transform.Translate(DASH_SPEED * Time.deltaTime * -1,0,0);	
		}
		
		if(dashDirection.Contains("Right"))
		{
			player.transform.Translate(DASH_SPEED * Time.deltaTime,0,0);	
		}
		
		if(dashDirection.Contains("Up"))
		{
			player.transform.Translate(0,DASH_SPEED * Time.deltaTime,0);	
		}
		
		if(dashDirection.Contains("Down"))
		{
			player.transform.Translate(0,DASH_SPEED * Time.deltaTime * -1,0);	
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
				stream.SendNext(player.transform.position);
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
			StartCoroutine("HealthFlash");
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
		int randomSpawnPoint = (int)(Random.value * (map.GetComponent<map>().spawnPoints.Count - 1));
		player.position = new Vector3(map.GetComponent<map>().spawnPoints[0].x * map.GetComponent<map>().conversionScale.x, map.GetComponent<map>().spawnPoints[0].y * map.GetComponent<map>().conversionScale.y, player.depth);
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
using UnityEngine;
using System.Collections;

public class bullet : Photon.MonoBehaviour
{
	
	private const float HORZ_BULLET_SPEED = 1.0f;
	private const float VERT_BULLET_SPEED = 0.59375f;
	private const float DELETION_TIME = .5f;
	private Vector2 speed;
	private float speedMod = 30f;
	public OTSprite thisBullet;
	public OTSprite playerOwner;
	private Timer timeToDelete;
	public bool isDead;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	public void Start() 
	{
		timeToDelete = new Timer();
		isDead = true;
		thisBullet.onCollision = OnCollision;
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	public void Update()
	{
		if(!isDead)
		{
			thisBullet.position += speed * Time.deltaTime;
			
			//kill the bullet if it has been alive long enough
			if (timeToDelete.Update())
			{
				isDead = true;
			}
		}
	}
	
	/// <summary>
	/// Fires when bullet collides with another object.
	/// </summary>
	/// <param name='owner'>
	/// The bullet.
	/// </param>
	public void OnCollision(OTObject bullet)
	{
		if ((bullet.collisionObject.name == "PlayerOnePrefab(Clone)" || 
			bullet.collisionObject.name == "PlayerTwoPrefab(Clone)" || 
			bullet.collisionObject.name == "player-1" || 
			bullet.collisionObject.name == "player-2") && 
			thisBullet.collidable == true)
		{	
			if (playerOwner != bullet.collisionObject)
			{
				isDead = true;
				
				//switch to damage on gun
				bullet.collisionObject.GetComponent<playerUpdate>().DeductHealth(3);
			}
		}
	}
	
	/// <summary>
	/// Fires a bullet.
	/// </summary>
	public void Fire()  
	{
		speed = Vector2.zero;
		Vector2 pos = thisBullet.position;
		timeToDelete.Countdown(DELETION_TIME);
		isDead = false;
		thisBullet.visible = true;
		thisBullet.collidable = true;

        //Set Position and rotation based on player variables
        switch (playerOwner.GetComponent<playerUpdate>().lastDirection)
        {
            case "Down":
			case "DownStatic":
                {
				    thisBullet.rotation = 270;
                    speed = new Vector2(0.0f, -1 * VERT_BULLET_SPEED);
                    pos.x = playerOwner.position.x;
                    pos.y = playerOwner.position.y;
                    break;
                }
            case "LeftDown":
			case "LeftDownStatic":
                {
			        thisBullet.rotation = 210.6f;
                    speed = new Vector2(-1 * HORZ_BULLET_SPEED, -1 * VERT_BULLET_SPEED);
                    pos.x = playerOwner.position.x;
                    pos.y = playerOwner.position.y;
                    break;
                }
            case "Left":
			case "LeftStatic":
                {
                    thisBullet.rotation = 180;
                    speed = new Vector2(-1 * HORZ_BULLET_SPEED, 0.0f);
                    pos.x = playerOwner.position.x;
                    pos.y = playerOwner.position.y;
                    break;
                }
            case "LeftUp":
			case "LeftUpStatic":
                {
					thisBullet.rotation = 149.4f;
                    speed = new Vector2(-1 * HORZ_BULLET_SPEED, VERT_BULLET_SPEED);
                    pos.x = playerOwner.position.x;
                    pos.y = playerOwner.position.y;

                    break;
                }
            case "Up":
			case "UpStatic":
                {
			        thisBullet.rotation = 90;
                    speed = new Vector2(0.0f, VERT_BULLET_SPEED);
                    pos.x = playerOwner.position.x;
                    pos.y = playerOwner.position.y;
                    break;
                }
            case "RightUp":
			case "RightUpStatic":
                {

			        thisBullet.rotation = 30.6f;
                    speed = new Vector2(HORZ_BULLET_SPEED, VERT_BULLET_SPEED);
                    pos.x = playerOwner.position.x;
                    pos.y = playerOwner.position.y;
                    break;
                }
            case "Right":
			case "RightStatic":
                {
                    thisBullet.rotation = 0;
                    speed = new Vector2(HORZ_BULLET_SPEED, 0.0f);
                    pos.x = playerOwner.position.x;
                    pos.y = playerOwner.position.y;
                    break;
                }
            case "RightDown":
			case "RightDownStatic":
                {
                    thisBullet.rotation = 329.4f;
                    speed = new Vector2(HORZ_BULLET_SPEED, -1 * VERT_BULLET_SPEED);
                    pos.x = playerOwner.position.x;
                    pos.y = playerOwner.position.y;
                    break;
                }
        }
		
		thisBullet.position = pos;
		
        speed.Normalize();
        speed = speed * speedMod;
	}
}
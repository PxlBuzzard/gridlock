using UnityEngine;
using System.Collections;

public class bullet : MonoBehaviour
{
	private const float DELETION_TIME = .5f;
	private Vector2 speed;
	private float speedMod = 0.5f;
	public OTSprite thisBullet;
	private Timer timeToDelete;
	public bool isDead;
	
	// Use this for initialization
	public void Start () 
	{
		timeToDelete = new Timer();
		isDead = true;
		thisBullet.onCollision = OnCollision;
	}
	
	public void OnCollision(OTObject owner)
	{
		thisBullet.visible = false;
	}
	
	// Update is called once per frame
	public void Update()
	{
		if(!isDead)
		{
			thisBullet.position += speed;
			
			//kill the bullet if it's been alive long enough
			if (timeToDelete.Update())
			{
				isDead = true;
				thisBullet.visible = false;
			}
		}
	}
	
	public void Fire (OTAnimatingSprite player)  
	{
		speed = Vector2.zero;
		Vector2 pos = thisBullet.position;
		timeToDelete.Countdown(DELETION_TIME);
		isDead = false;
		thisBullet.visible = true;

        //Set Position and rotation based on player variables
        switch (player.GetComponent<playerUpdate>().lastDirection)
        {
            case "Down":
			case "DownStatic":
                {
				    thisBullet.rotation = Mathf.PI * 3 / 2;
                    speed = new Vector2(0.0f, -1.0f);
                    pos.x = player.position.x;
                    pos.y = player.position.y;
                    break;
                }
            case "LeftDown":
			case "LeftDownStatic":
                {
			        thisBullet.rotation = Mathf.PI * 5 / 4;
                    speed = new Vector2(-1.0f, -1.0f);
                    pos.x = player.position.x;
                    pos.y = player.position.y;
                    break;
                }
            case "Left":
			case "LeftStatic":
                {
                    thisBullet.rotation = Mathf.PI;
                    speed = new Vector2(-1.0f, 0.0f);
                    pos.x = player.position.x;
                    pos.y = player.position.y;
                    break;
                }
            case "LeftUp":
			case "LeftUpStatic":
                {
					thisBullet.rotation = Mathf.PI * 3 / 4;
                    speed = new Vector2(-1.0f, 1.0f);
                    pos.x = player.position.x;
                    pos.y = player.position.y;
                    break;
                }
            case "Up":
			case "UpStatic":
                {
			        thisBullet.rotation = Mathf.PI / 2;
                    speed = new Vector2(0.0f, 1.0f);
                    pos.x = player.position.x;
                    pos.y = player.position.y;
                    break;
                }
            case "RightUp":
			case "RightUpStatic":
                {

			        thisBullet.rotation = Mathf.PI / 4;
                    speed = new Vector2(1f, 1f);
                    pos.x = player.position.x;
                    pos.y = player.position.y;
                    break;
                }
            case "Right":
			case "RightStatic":
                {
                    thisBullet.rotation = 0;
                    speed = new Vector2(1.0f, 0.0f);
                    pos.x = player.position.x;
                    pos.y = player.position.y;
                    break;
                }
            case "RightDown":
			case "RightDownStatic":
                {
                    thisBullet.rotation = Mathf.PI * 7 / 4;
                    speed = new Vector2(1.0f, -1.0f);
                    pos.x = player.position.x;
                    pos.y = player.position.y;
                    break;
                }
        }
		
		thisBullet.position = pos;
		
        speed.Normalize();
        speed = speed * speedMod;
		
		//speed = new Vector2(5, 5);
	}
}
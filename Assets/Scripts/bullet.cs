using UnityEngine;
using System.Collections;

public class bullet : MonoBehaviour
{
	private const float DELETION_TIME = 5f;
	private Vector2 speed;
	private float speedMod = 0.3f;
	public OTSprite thisBullet;
	private Timer timeToDelete;
	public bool isDead;
	
	// Use this for initialization
	void Start () 
	{
		timeToDelete = new Timer();
		isDead = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		thisBullet.position += speed;
		timeToDelete.Update();
		
		//kill the bullet if it's been alive long enough
		if (timeToDelete.isFinished)
		{
			isDead = true;
		}
	}
	
	public void Fire (OTAnimatingSprite player)  
	{
		speed = Vector2.zero;
		Vector2 pos = thisBullet.position;
		timeToDelete.Countdown(DELETION_TIME);
		isDead = false;

        //Set Position and rotation based on player variables
        switch (player.GetComponent<playerUpdate>().lastDirection)
        {
            case "Down":
                {
				    thisBullet.rotation = Mathf.PI * 3 / 2;
                    speed = new Vector2(0.0f, -1.0f);
                    pos.x = player.position.x;
                    pos.y = player.position.y;
                    break;
                }
            case "LeftDown":
                {
			        thisBullet.rotation = Mathf.PI * 5 / 4;
                    speed = new Vector2(-1.0f, -1.0f);
                    pos.x = player.position.x;
                    pos.y = player.position.y;
                    break;
                }
            case "Left":
                {
                    thisBullet.rotation = Mathf.PI;
                    speed = new Vector2(-1.0f, 0.0f);
                    pos.x = player.position.x;
                    pos.y = player.position.y;
                    break;
                }
            case "LeftUp":
                {
					thisBullet.rotation = Mathf.PI * 3 / 4;
                    speed = new Vector2(-1.0f, 1.0f);
                    pos.x = player.position.x;
                    pos.y = player.position.y;
                    break;
                }
            case "Up":
                {
			        thisBullet.rotation = Mathf.PI / 2;
                    speed = new Vector2(0.0f, 1.0f);
                    pos.x = player.position.x;
                    pos.y = player.position.y;
                    break;
                }
            case "RightUp":
                {

			        thisBullet.rotation = Mathf.PI / 4;
                    speed = new Vector2(1f, 1f);
                    pos.x = player.position.x;
                    pos.y = player.position.y;
                    break;
                }
            case "Right":
                {
                    thisBullet.rotation = 0;
                    speed = new Vector2(1.0f, 0.0f);
                    pos.x = player.position.x;
                    pos.y = player.position.y;
                    break;
                }
            case "RightDown":
                {
                    thisBullet.rotation = Mathf.PI * 7 / 4;
                    speed = new Vector2(1.0f, -1.0f);
                    pos.x = player.position.x;
                    pos.y = player.position.y;
                    break;
                }
        }
		
		print ("Posx: " + pos.x + ", Posy: " + pos.y);
		
		thisBullet.position = pos;
		
        speed.Normalize();
        speed = speed * speedMod;
		
		//speed = new Vector2(5, 5);
	}
}
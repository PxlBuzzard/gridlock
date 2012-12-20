using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the logic for coins.
/// </summary>
public class coin : MonoBehaviour {
	
	/// <summary>
	/// The maximum distance from the player to move towards them.
	/// </summary>
	private const float DISTANCE_TO_ATTRACT = 50.0f;
	private int coinValue;
	private Vector2 direction;
	private float speed;
	private bool goToPlayer;
	public OTAnimatingSprite player;
	public OTAnimatingSprite thisCoin;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start() 
	{
		goToPlayer = false;
		coinValue = 10;
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update() 
	{
		if (goToPlayer)
		{
			//do a proximity check
			Vector2 proximity = player.position - thisCoin.position;
			if (proximity.sqrMagnitude < DISTANCE_TO_ATTRACT * DISTANCE_TO_ATTRACT)
			{
				direction = proximity / proximity.magnitude;
				
				//speed up the coin as it goes to the player
				speed *= 1.03f;
			}
			else 
			{
				//slow coin down if it isn't moving towards the player
				speed *= 0.97f;
			}
		}
		
		//update coin's position
		thisCoin.position = new Vector2(direction.x * speed * Time.deltaTime, direction.y * speed * Time.deltaTime);
	}
	
	/// <summary>
	/// Give the coin a random direction and velocity.
	/// </summary>
	public void Explode()
	{
		//random direction
		float rads = Random.value * Mathf.PI * 2;
    	direction = new Vector2(Mathf.Cos(rads), Mathf.Sin(rads));
		
		//random velocity between 1 and 5
		speed = 1.0f * (Random.value * 5);
		
		//move towards player if in proximity
		goToPlayer = true;
	}
	
	/// <summary>
	/// Happens on initial collision with another object.
	/// </summary>
	/// <param name='owner'>
	/// The thing being collided with.
	/// </param>
	public void OnCollision(OTObject owner)
	{
		if((owner.collisionObject == player) && thisCoin.collidable)
		{	
			//isDead = true;
			//owner.collisionObject.GetComponent<playerUpdate>().Coins(coinValue);
		}
	}
}

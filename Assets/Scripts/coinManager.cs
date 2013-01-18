using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class coinManager : Photon.MonoBehaviour 
{
	private const int NUM_COINS = 30;
	private Queue<OTSprite> inactiveCoins;
	private List<OTSprite> activeCoins;
	public OTSprite player;
	public OTSprite CoinPrefab;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start() 
	{
		inactiveCoins = new Queue<OTSprite>();
		activeCoins = new List<OTSprite>();
		
		for (int i = 0; i < NUM_COINS; i++) 
		{
			OTSprite aCoin = (OTSprite)Instantiate(CoinPrefab);
			(aCoin as OTSprite).position = new Vector3(-25, -25, 0);
			//aCoin.collidable = false;
			aCoin.GetComponent<bullet>().playerOwner = player;
			inactiveCoins.Enqueue(aCoin);
		}
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	public void Update() 
	{	
		for (int i = 0; i < activeCoins.Count; i++)
		{
			if (activeCoins[i].GetComponent<coin>().isDead)
			{
				activeCoins[i].collidable = false;
				activeCoins[i].position = new Vector3(0, 0, 0);
				activeCoins[i].visible = false;
				inactiveCoins.Enqueue(activeCoins[i]);
				activeCoins.Remove(activeCoins[i]);
				i--;
			}
		}
	}
	
	/// <summary>
	/// Explode some coins out of a player.
	/// </summary>
	/// <param name='player'>
	/// Player to explode out of.
	/// </param>
	public void Explode(OTAnimatingSprite player)
	{
		if(inactiveCoins.Count > 0)
		{
			OTSprite aCoin = inactiveCoins.Dequeue();
			activeCoins.Add(aCoin);
			aCoin.GetComponent<coin>().Explode(player);
		}
	}
}
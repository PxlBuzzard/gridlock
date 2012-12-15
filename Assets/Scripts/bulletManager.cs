using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bulletManager : Photon.MonoBehaviour 
{
	private const int NUM_BULLETS = 30;
	private Queue<OTSprite> inactiveBullets;
	private List<OTSprite> activeBullets;
	public OTSprite player;
	public OTSprite BulletPrefab;

	// Use this for initialization
	void Start () 
	{
		inactiveBullets = new Queue<OTSprite>();
		activeBullets = new List<OTSprite>();
		
		for (int i = 0; i < NUM_BULLETS; i++) 
		{
			OTSprite aBullet = (OTSprite)Instantiate(BulletPrefab);
			(aBullet as OTSprite).position = new Vector3(-25, -25, 0);
			//aBullet.collidable = false;
			aBullet.GetComponent<bullet>().playerOwner = player;
			inactiveBullets.Enqueue(aBullet);
		}
	}
	
	// Update is called once per frame
	public void Update () 
	{	
		for (int i = 0; i < activeBullets.Count; i++)
		{
			if (activeBullets[i].GetComponent<bullet>().isDead)
			{
				activeBullets[i].collidable = false;
				activeBullets[i].position = new Vector3(0, 0, 0);
				activeBullets[i].visible = false;
				inactiveBullets.Enqueue(activeBullets[i]);
				activeBullets.Remove(activeBullets[i]);
				i--;
			}
		}
	}
	
	public void Fire (OTAnimatingSprite player)
	{
		if(inactiveBullets.Count > 0)
		{
			OTSprite aBullet = inactiveBullets.Dequeue();
			activeBullets.Add(aBullet);
			
			aBullet.GetComponent<bullet>().Fire(player);
		}
	}
}
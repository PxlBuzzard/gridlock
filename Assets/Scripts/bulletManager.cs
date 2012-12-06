using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bulletManager : MonoBehaviour 
{
	private const int NUM_BULLETS = 100;
	private Queue<OTSprite> inactiveBullets;
	public OTSprite BulletPrefab;
	public int test;
	
	// Use this for initialization
	void Start () 
	{
		inactiveBullets = new Queue<OTSprite>();
		
		for (int i = 0; i < NUM_BULLETS; i++) 
		{
			OTSprite aBullet = (OTSprite)Instantiate(BulletPrefab);
			(aBullet as OTSprite).position = new Vector3(-25, -25, 0);
			inactiveBullets.Enqueue(aBullet);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	public void Fire (OTAnimatingSprite player)
	{
		if(inactiveBullets.Count > 0)
		{
			OTSprite aBullet = inactiveBullets.Dequeue();
			
			aBullet.GetComponent<bullet>().Fire(player);
			
			test++;
			print ("Bullet Fired: " + test);
		}
	}
}
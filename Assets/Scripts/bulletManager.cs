using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bulletManager : MonoBehaviour 
{
	private const int NUM_BULLETS = 100;
	private Queue<bullet> inactiveBullets;
	private List<bullet> activeBullets;
	public OTSprite BulletPrefab;
	public int test;
	
	// Use this for initialization
	void Start () 
	{
		inactiveBullets = new Queue<bullet>();
		activeBullets = new List<bullet>();
		
		for (int i = 0; i < NUM_BULLETS; i++) 
		{
			bullet aBullet = (bullet)Instantiate(BulletPrefab);
			aBullet.thisBullet.position = new Vector3(-25, -25, 0);
			inactiveBullets.Enqueue(aBullet);
		}
	}
	
	// Update is called once per frame
	public void Update () 
	{
		foreach (bullet bullet in activeBullets)
		{
			bullet.UpdateBullet();
			if (bullet.isDead)
			{
				activeBullets.Remove(bullet);
				inactiveBullets.Enqueue(bullet);
			}
		}
	}
	
	public void Fire (OTAnimatingSprite player)
	{
		if(inactiveBullets.Count > 0)
		{
			bullet aBullet = inactiveBullets.Dequeue();
			
			aBullet.GetComponent<bullet>().Fire(player);
			
			test++;
			print ("Bullet Fired: " + test);
		}
	}
}
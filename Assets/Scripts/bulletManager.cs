using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bulletManager : MonoBehaviour 
{
	private const int NUM_BULLETS = 100;
	private Queue<OTSprite> inactiveBullets;
	private List<OTSprite> activeBullets;
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
			inactiveBullets.Enqueue(aBullet);
		}
	}
	
	// Update is called once per frame
	public void Update () 
	{
		for (int i = 0; i < activeBullets.Count; i++)
		{
			activeBullets[i].GetComponent<bullet>().Update();
			
			if (activeBullets[i].GetComponent<bullet>().isDead)
			{
				activeBullets[i].position = new Vector3(-25, -25, 0);
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
			
			print ("Bullet Fired: " + activeBullets.Count);
		}
	}
}
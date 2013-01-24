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

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () 
	{
		inactiveBullets = new Queue<OTSprite>();
		activeBullets = new List<OTSprite>();
		
		for (int i = 0; i < NUM_BULLETS; i++) 
		{
			OTSprite aBullet = (OTSprite)Instantiate(BulletPrefab);
			aBullet.GetComponent<bullet>().playerOwner = player;
			aBullet.visible = false;
			inactiveBullets.Enqueue(aBullet);
			aBullet.otCollider = new BoxCollider();
			aBullet.position = new Vector2(9999, 9999);
			
		}
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update () 
	{	
		for (int i = 0; i < activeBullets.Count; i++)
		{
			if (activeBullets[i].GetComponent<bullet>().isDead)
			{
				activeBullets[i].collidable = false;
				activeBullets[i].visible = false;
				activeBullets[i].position = Vector3.zero;
				inactiveBullets.Enqueue(activeBullets[i]);
				activeBullets.Remove(activeBullets[i]);
				i--;
			}
		}
	}
	
	/// <summary>
	/// Called when this class is being destroyed.
	/// </summary>
	void OnDestroy ()
	{
		print ("Delete bullet manager");
		for (int i = 0; i < activeBullets.Count; i++)
		{
			Destroy(activeBullets[i].gameObject);
		}
		
		for (int i = 0; i < inactiveBullets.Count; i++)
		{
			Destroy(inactiveBullets.Dequeue().gameObject);
			i--;
		}
	}
	
	/// <summary>
	/// Fire a bullet.
	/// </summary>
	public void Fire ()
	{
		if (inactiveBullets.Count > 0)
		{
			OTSprite aBullet = inactiveBullets.Dequeue();
			activeBullets.Add(aBullet);
			
			aBullet.GetComponent<bullet>().Fire();
		}
	}
}
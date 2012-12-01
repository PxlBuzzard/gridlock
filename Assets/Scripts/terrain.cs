using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class terrain : MonoBehaviour 
{
	public OTSprite grassPrefab;
	private List<OTSprite> grassTiles;
	
	// Use this for initialization
	void Start () 
	{
		grassTiles = new List<OTSprite>();
		
		for (int i = 0; i < 42; i++) 
		{
			for (int ii = 0; ii < 20; ii++) 
			{
				OTSprite aGrassTile = (OTSprite)Instantiate(grassPrefab);
				(aGrassTile as OTSprite).position = new Vector3(i - 21, ii - 9.5f, 0);
				grassTiles.Add(aGrassTile);
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}

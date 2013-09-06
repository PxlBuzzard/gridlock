using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OTBatching : MonoBehaviour {
	
	public OTSpriteBatch batch = null;	
	public bool batchOnStart = true;
	OTSprite sprite = null;
	
	public static void Batch(OTSprite sprite, OTSpriteBatch batch)
	{
		if (batch!=null)
		{
			OTBatching b = sprite.GetComponent<OTBatching>();
			if (b==null)
				b = sprite.gameObject.AddComponent<OTBatching>();
				
			b.batch = batch;
			b.Batch();				
		}
	}
	
	public static void UnBatch(OTSprite sprite)
	{
		OTBatching b = sprite.GetComponent<OTBatching>();
		if (b!=null)
			Destroy(b);
	}	
	
	OTSprite[] sprites = new OTSprite[] {};
	public void Batch()
	{
		if (sprite==null)
			sprite = GetComponent<OTSprite>();
		
	
		if (batch!=null)
		{
			if (sprite!=null)
				batch.AddSprite(sprite);				
				
			sprites = gameObject.GetComponentsInChildren<OTSprite>();
			for (int i=0; i<sprites.Length; i++)
			{
				if (sprites[i].isInvalid)
					sprites[i].ForceUpdate();
				batch.AddSprite(sprites[i]);				
			}
		}
	}
		
	public void UnBatch()
	{
		if (sprite==null)
			sprite = GetComponent<OTSprite>();
		if (batch!=null)
		{
			if (sprite!=null)
				batch.RemoveSprite(sprite);		
			for (int i=0; i<sprites.Length; i++)
				batch.RemoveSprite(sprites[i]);				
		}
	}
	
	void Start()
	{
		if (batchOnStart)
		  Batch();
	}
	
}

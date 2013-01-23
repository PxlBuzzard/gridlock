using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class map : MonoBehaviour 
{
	public OTTileMap currentMap;
	public Vector3 mapScale;
	public List<Vector2> spawnPoints;
	public Vector2 conversionScale;
	public bool mapLoaded = true;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () 
	{
		LoadMap ("Map_roads");
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update () 
	{
		// When method LoadMap is called it automatically reloads the map, resetting scale, depth, etc. once start is over
		// This will reset them whenever a new map is loaded
		if(mapLoaded)
		{			
			mapLoaded = false;
			currentMap.transform.localScale = mapScale;
			Vector3 tempStartScale = currentMap.otTransform.localScale;
			
			conversionScale = new Vector2(mapScale.x / tempStartScale.x, mapScale.y / tempStartScale.y);
			currentMap.depth = 1;
		}
		
	}
	
	void LoadMap (string mapToLoad)
	{
		currentMap.tileMapXML = (TextAsset)Resources.Load("Maps/" + mapToLoad);
		UpdateMapProperties();
		
		mapLoaded = true;
	}
	
	void UpdateMapProperties()
	{
		XmlDocument mapLoading = new XmlDocument();
        mapLoading.LoadXml(currentMap.tileMapXML.text);
		XmlNodeList properties = mapLoading.SelectSingleNode("map").SelectSingleNode("objectgroup").SelectNodes("object");
		
		foreach(XmlNode property in properties)
		{
			spawnPoints.Add(new Vector2((float)XmlConvert.ToDouble(property.Attributes["x"].Value), (float)XmlConvert.ToDouble(property.Attributes["y"].Value)));
		}
	}
}

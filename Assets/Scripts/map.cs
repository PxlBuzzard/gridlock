using UnityEngine;
using System.Collections;
using System.Xml;

public class map : MonoBehaviour 
{
	public OTTileMap currentMap;
	public Vector3 mapScale = new Vector3(128, 76, 1);
	//public Queue<Vector3> spawnPoints;
	
	// Use this for initialization
	void Start () 
	{
		LoadMap ("respawnMap_1");
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void LoadMap (string mapToLoad)
	{
		currentMap.tileMapXML = (TextAsset)Resources.Load("Maps/" + mapToLoad);
		currentMap.Reload();
		currentMap.otTransform.localScale = mapScale;
		currentMap.depth = 1;	
		
		UpdateMapProperties();
	}
	
	void UpdateMapProperties()
	{
		System.IO.StreamReader mapLoading = new System.IO.StreamReader("Assets/Resources/Maps/respawnMap_1.xml");
		XmlTextReader reader = new XmlTextReader(mapLoading);
		
		
		
		
		//print (currentNode.);
	}
}

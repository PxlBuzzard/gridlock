using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class GameManager : Photon.MonoBehaviour {
	
	private const int MAX_PLAYERS_IN_ROOM = 20;
	private string roomName;
	private Vector3 spawnPoint = new Vector3(0,0,0);
	private Vector2 cameraPosition = Vector2.zero;
	private float cameraZoom = 0f;
	private float xDist;
	private float yDist;
	private bool p2exists = false;
	
	void Start() 
	{
		//needed for Photon
		Application.runInBackground = true;
		
		//Setup Photon
		if (!PhotonNetwork.connected)
		{
			PhotonNetwork.ConnectUsingSettings("1.0");
		}
		
		//create a random room name
		roomName = "Gridlock Test Room #" + (int)(Random.value * 1000);
	}
	
	void OnGUI() 
	{
		if (PhotonNetwork.room == null)
		{
			GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));
			if (GUILayout.Button("Click here to fill out the alpha feedback form :)"))
				Application.OpenURL("https://docs.google.com/spreadsheet/viewform?formkey=dEVJY2NzYll4dC1OSWhFMklRcHY3TGc6MA");
			GUILayout.Label("Main Menu");
			GUILayout.Label("Create room");
			GUILayout.BeginHorizontal();
			roomName = GUILayout.TextField(roomName);
			if (GUILayout.Button("Create"))
				PhotonNetwork.CreateRoom(roomName, true, true, MAX_PLAYERS_IN_ROOM);
			GUILayout.EndHorizontal();
			GUILayout.Space(30);
			GUILayout.Label("Room Listing");
			
			//Show all rooms
			foreach (RoomInfo room in PhotonNetwork.GetRoomList())
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button(room.name))
					PhotonNetwork.JoinRoom(room.name);
				GUILayout.Label(room.playerCount + " / " + room.maxPlayers);
				GUILayout.EndHorizontal();
			}
		
			GUILayout.EndArea();
		}
		else
		{
			if (GUILayout.Button("Leave room"))
				PhotonNetwork.LeaveRoom();
		}
	}
	
	// Photon Callback
	void OnJoinedRoom()
	{
		PhotonNetwork.Instantiate("PlayerOnePrefab", spawnPoint, Quaternion.identity, 0);
		PhotonNetwork.Instantiate("MapPrefab", new Vector3(0, 0, 0), Quaternion.identity, 0);
	}
	
	//TODO: Remove stuff if you leave a room
	void OnLeftLobby()
	{
		//delete all the stuff
	}
	
	void Update()
	{
		if (Input.GetButtonDown("P2join") && !p2exists)
		{
			PhotonNetwork.Instantiate("PlayerTwoPrefab", spawnPoint, Quaternion.identity, 0);
			p2exists = true;
		}
		
		
		if (GameObject.Find("PlayerTwoPrefab(Clone)") != null)
		{
			cameraPosition = new Vector2((GameObject.Find("PlayerOnePrefab(Clone)").transform.position.x + 
										GameObject.Find("PlayerTwoPrefab(Clone)").transform.position.x) / 2, 
										(GameObject.Find("PlayerOnePrefab(Clone)").transform.position.y + 
										GameObject.Find("PlayerTwoPrefab(Clone)").transform.position.y) / 2);
			
			xDist = Mathf.Abs(GameObject.Find("PlayerOnePrefab(Clone)").transform.position.x -
							GameObject.Find("PlayerTwoPrefab(Clone)").transform.position.x);
			yDist = Mathf.Abs(GameObject.Find("PlayerOnePrefab(Clone)").transform.position.y -
							GameObject.Find("PlayerTwoPrefab(Clone)").transform.position.y);
			
			if (xDist > yDist)
			{
				cameraZoom =  (-.03f * (float)xDist);
			} else {
				cameraZoom =  (-.03f * (float)yDist);
			}
			
		} else if (GameObject.Find("PlayerOnePrefab(Clone)") != null){
			cameraPosition = new Vector2(GameObject.Find("PlayerOnePrefab(Clone)").transform.position.x, GameObject.Find("PlayerOnePrefab(Clone)").transform.position.y);
		}
		OT.view.position = cameraPosition;
		OT.view.zoom = cameraZoom;
	}
}
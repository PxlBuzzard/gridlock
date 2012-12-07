using UnityEngine;
using System.Collections;
//using ExitGames.Client.Photon.LoadBalancing;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	
	private const int MAX_PLAYERS_IN_ROOM = 20;
	private string roomName = "Gridlock test";
	//public LoadBalancingClient GameInstance;
	private Vector3 spawnPoint = new Vector3(0,0,0);
	
	// Use this for initialization
	void Start() {
		Application.runInBackground = true;
		
		//Setup photon
		if (!PhotonNetwork.connected)
		{
			PhotonNetwork.ConnectUsingSettings("1.0");
		}
	}
	
	// Update is called once per frame
	void OnGUI() {
	
		if (PhotonNetwork.room == null)
		{
			GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));
			GUILayout.Label("Main Menu");
			
			GUILayout.Label("Create room");
			GUILayout.BeginHorizontal();
			roomName = GUILayout.TextField(roomName);
			if (GUILayout.Button("Create"))
				PhotonNetwork.CreateRoom(roomName, true, true, MAX_PLAYERS_IN_ROOM);
			GUILayout.EndHorizontal();
			
			GUILayout.Space(30);
			GUILayout.Label("Room Listing");
		
			foreach (RoomInfo room in PhotonNetwork.GetRoomList())
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button(room.name))
					PhotonNetwork.JoinRoom(room.name);
				GUILayout.Label(room.playerCount + "/" + room.maxPlayers);
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
		PhotonNetwork.Instantiate("PlayerPrefab", spawnPoint, Quaternion.identity, 0);
	}
}

using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	
	private const int MAX_PLAYERS_IN_ROOM = 20;
	private string roomName;
	private Vector3 spawnPoint = new Vector3(0,0,0);
	
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
		PhotonNetwork.Instantiate("PlayerPrefab", spawnPoint, Quaternion.identity, 0);
		//Instantiate("CoinPrefab", spawnPoint, Quaternion.identity, 0);
	}
	
	//TODO: Remove stuff if you leave a room
	void OnLeftLobby()
	{
		//delete all the stuff
	}
}
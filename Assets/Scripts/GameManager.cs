using UnityEngine;
using System.Collections;
//using ExitGames.Client.Photon.LoadBalancing;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	
	private string roomName = "Gridlock test";
	//public LoadBalancingClient GameInstance;
	private Vector3 spawnPoint = new Vector3(0,0,0);
	
	// Use this for initialization
	void Start() {
		Application.runInBackground = true;
		// CustomTypes.Register();
		//this.GameInstance = new LoadBalancingClient();
		//this.GameInstance.MasterServerAddress = "app.exitgamescloud.com:5055";
        //this.GameInstance.ConnectToMaster("d787f7c3-5e04-46f2-96c3-5ced102c4817", "1.0", "unityPlayer");
		
		//Setup photon
		if (!PhotonNetwork.connected)
		{
			//this.ConnectToMaster("d787f7c3-5e04-46f2-96c3-5ced102c4817", "1.0", "thePlayerName");
			//PhotonNetwork.Connect("app.exitgamescloud.com", 5055, "Gridlock");
			PhotonNetwork.ConnectUsingSettings("1.0");
		}
	}
	
	// Update is called once per frame
	void OnGUI() {
	
		if (PhotonNetwork.room == null)
		{
			GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));
			GUILayout.Label("Main Menu");
			
			GUILayout.Label("Join room");
			GUILayout.BeginHorizontal();
			roomName = GUILayout.TextField(roomName);
			if (GUILayout.Button("Join"))
				PhotonNetwork.CreateRoom(roomName);
			GUILayout.EndHorizontal();
			
			GUILayout.Space(30);
			GUILayout.Label("Room Listing");
		
			foreach (Room room in PhotonNetwork.GetRoomList())
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

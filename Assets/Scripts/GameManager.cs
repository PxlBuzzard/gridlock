using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class GameManager : Photon.MonoBehaviour {
	
	#region Variables
	private const int MAX_PLAYERS_IN_ROOM = 20;
	private string roomName;
	private Vector3 spawnPoint = new Vector3(0,0,0);
	private Vector2 cameraPosition = Vector2.zero;
	private float cameraZoom = 0f;
	private float xDist;
	private float yDist;
	private bool p2exists = false;
	private string mute = "Mute";
	private Timer timer = new Timer();
	public Texture2D logoText;
	public Texture2D logoPlanet;
	public Texture2D pressStart;
	public Texture2D controllerHelp;
	
	private enum GameState { Startup, MainMenu, Loading, Paused, InGame };
	
	private GameState gameState = GameState.Startup;

	#endregion
	
	/// <summary>
	/// Start this instance.
	/// </summary>
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
		roomName = "Gridlock Prototype Server"; //"Gridlock Test Room #" + (int)(Random.value * 1000);
		
		// black background
		camera.clearFlags = CameraClearFlags.SolidColor;
		camera.backgroundColor = Color.black;
	}
	
	void Awake()
	{
		
	}
	
	/// <summary>
	/// Draws the GUI.
	/// </summary>
	void OnGUI() 
	{
		switch (gameState)
		{
			case GameState.Startup:
				{
					// start playing music here
			
					if (timer.isFinished)
					{
						// (Fade in)Super Company presents...(fade out)
						GUIStyle companyStyle = new GUIStyle();
						companyStyle.normal.background = new Texture2D(0, 0);
						Color companyColor = Color.white;
						companyColor.a = Mathf.Sin((Time.fixedTime - .95f) * 1f);
						companyStyle.normal.textColor = companyColor;
						companyStyle.fontSize = 32;
			
						if (companyStyle.normal.textColor.a > -0.45f)
						{
							GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 200) / 2, 400, 300));
								GUILayout.Label("Super Company presents", companyStyle);
							GUILayout.EndArea();
						}
						else
						{
							timer.Reset();
							gameState = GameState.MainMenu;
						}
					}
					else if (!timer.isRunning)
					{
						timer.Countdown(2f);
					}
					else
					{
						timer.Update();
					}
					break;
				}

			case GameState.MainMenu:
				{
					//planet
					GUILayout.BeginArea(new Rect((Screen.width - 1000) / 2, Mathf.Sin(.5f * (Time.time / 2) * (Screen.height - 900) / 2) * 2, 1000, 1000));
						GUILayout.Label(logoPlanet);
					GUILayout.EndArea();
					
					//game logo
					GUILayout.BeginArea(new Rect((Screen.width - 1000) / 2, (Screen.height - 500) / 2, 1000, 500));
						GUILayout.Label(logoText);
					GUILayout.EndArea();
			
					//Press Start to begin text
					Color startColor = Color.white;
					startColor.a = Mathf.Cos(Time.fixedTime * 4f);
					GUI.color = startColor;
					GUI.DrawTexture(new Rect((Screen.width - 843) / 2, (Screen.height + 500) / 2, 843, 121), pressStart);
					
					// TAKE THIS MUTHA OUT
					//gameState = GameState.Loading;
					
					//check for input to move to loading screen
					if (Event.current.type == EventType.KeyDown)
					{
						gameState = GameState.Loading;
					}
			
					if (OuyaInputManager.GetButtonDown("O", OuyaSDK.OuyaPlayer.player1))
					{
						gameState = GameState.Loading;
					}
			
					break;
				}
			case GameState.Loading:
				{
					if (timer.isFinished)
					{
						PhotonNetwork.JoinRoom(roomName);
						if (PhotonNetwork.room == null)
						{
							PhotonNetwork.CreateRoom(roomName, true, true, MAX_PLAYERS_IN_ROOM);	
						}
						timer.Reset();
						gameState = GameState.InGame;
					}
					else if (!timer.isRunning)
					{
						timer.Countdown(10f);
					}
					else 
					{
						//controller help
						GUILayout.BeginArea(new Rect((Screen.width - 926) / 2, (Screen.height - 800) / 2, 926, 612));
							GUILayout.Label(controllerHelp);
						GUILayout.EndArea();
				
						//player count
						GUIStyle companyStyle = new GUIStyle();
						companyStyle.alignment = TextAnchor.MiddleCenter;
						companyStyle.normal.textColor = Color.white;
						companyStyle.fontSize = 20;
						GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 100) / 2, 400, 100));
							GUILayout.Label("Player count: " + PhotonNetwork.countOfPlayersInRooms, companyStyle);
						GUILayout.EndArea();
				
						//loading text
						companyStyle.fontSize = 40;
						GUILayout.BeginArea(new Rect((Screen.width - 600) / 2, (Screen.height + 500) / 2, 600, 200));
							GUILayout.Label("Connecting to server...", companyStyle);
						GUILayout.EndArea();
						timer.Update();
					}
					break;
				}
			
			case GameState.InGame:
				{
					
					break;
				}
			
			case GameState.Paused:
				{
					
					break;
				}
		}
		// picture of controls
		// loading into server... (load map, don't spawn player)
		// Press any button to continue
		
		// PAUSE MENU
		
		/*
		if (PhotonNetwork.room == null || gameState == GameState.Paused)
		{
			if (GUILayout.Button(mute))
			{
				if (!AudioListener.pause)
				{
					AudioListener.pause = true;
					mute = "Unmute";
				} else {
					AudioListener.pause = false;
					mute = "Mute";
				}
				
			}
			
			GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));
			if (GUILayout.Button("Click here to fill out the alpha feedback form :)"))
				Application.OpenURL("https://docs.google.com/spreadsheet/viewform?formkey=dEVJY2NzYll4dC1OSWhFMklRcHY3TGc6MA");
			GUILayout.Label("Main Menu");
			GUILayout.Label("Create room");
			GUILayout.BeginHorizontal();
			roomName = GUILayout.TextField(roomName);
			if (GUILayout.Button("Create"))
			{
				if (PhotonNetwork.room != null)
				{
					PhotonNetwork.LeaveRoom();
				}
				PhotonNetwork.CreateRoom(roomName, true, true, MAX_PLAYERS_IN_ROOM);
				gameState = GameState.InGame;
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(30);
			GUILayout.Label("Room Listing");
			
			//Show all rooms
			foreach (RoomInfo room in PhotonNetwork.GetRoomList())
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button(room.name))
				{
					if (PhotonNetwork.room != null)
					{
						PhotonNetwork.LeaveRoom();
					}
					PhotonNetwork.JoinRoom(room.name);
					gameState = GameState.InGame;
				}
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
		*/
	}
	
	// Photon Callback
	void OnJoinedRoom()
	{
		PhotonNetwork.Instantiate("PlayerOnePrefab", spawnPoint, Quaternion.identity, 0);
		Instantiate(Resources.Load("MapPrefab"), Vector3.zero, Quaternion.identity);
		
	}
	
	//TODO: Remove stuff if you leave a room
	void OnLeftLobby()
	{
		//delete all the stuff
		print ("a player left the room");
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{
		if (Input.GetButtonDown("P2join") && !p2exists)
		{
			PhotonNetwork.Instantiate("PlayerTwoPrefab", spawnPoint, Quaternion.identity, 0);
			p2exists = true;
		}
		
		if (Input.GetButtonDown("Pause") && gameState == GameState.InGame)
		{
			gameState = GameState.Paused;
		}
		else if (gameState == GameState.Paused && Input.GetButtonDown("Pause"))
		{
			gameState = GameState.InGame;
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
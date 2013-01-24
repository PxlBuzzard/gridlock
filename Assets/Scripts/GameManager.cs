using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;
using System.Collections.Generic;

/// <summary>
/// The primary class to run code for the game.
/// </summary>
public class GameManager : Photon.MonoBehaviour {
	
	#region Variables
	private const int MAX_PLAYERS_IN_ROOM = 20;
	public const int KILLS_TO_WIN = 15;
	private string roomName;
	private Vector3 spawnPoint = new Vector3(0,0,0);
	private Vector2 cameraPosition = Vector2.zero;
	private float cameraZoom = 0f;
	private float xDist;
	private float yDist;
	private bool p2exists = false;
	private string mute = "Mute";
	private Timer timer = new Timer();
	private Timer timer2 = new Timer();
	private int timeRemaining = 10;
	public Texture2D superCompanyLogo;
	public Texture2D logoText;
	public Texture2D logoPlanet;
	public Texture2D pressStart;
	public Texture2D controllerHelp;
	public OTSound mainMusic;
	
	public OTAnimatingSprite player;
	public OTTileMap map;

	public enum GameState { Startup, MainMenu, Loading, Paused, InGame, Leaderboard };
	
	public GameState gameState;
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
		
		//start the background music
		mainMusic = new OTSound("main_theme");
		mainMusic.Volume(.5f);
		mainMusic.Loop();
		mainMusic.Stop();
		
		//create a random room name
		roomName = "Gridlock Prototype Server"; //"Gridlock Test Room #" + (int)(Random.value * 1000);
		
		// black background
		camera.clearFlags = CameraClearFlags.SolidColor;
		camera.backgroundColor = Color.black;
		
		//skip to game if debugging in editor
		if (Application.isEditor)
			gameState = GameState.Loading;
		else
			gameState = GameState.Startup;
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
					if (timer.isFinished)
					{
						// (Fade in)Super Company presents...(fade out)
						Color startColor = Color.white;
						startColor.a = Mathf.Sin((Time.fixedTime - .95f) * 1f);
						GUI.color = startColor;
			
						if (startColor.a > -0.45f)
						{
							GUI.DrawTexture(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 200), superCompanyLogo);
						}
						else
						{
							// start playing music here
							mainMusic.Play();
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
					GUILayout.BeginArea(new Rect((Screen.width - 1000) / 2, Mathf.Sin((Time.time / 8f) * (Screen.height - 900) / 2), 1000, 1000));
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
					break;
				}
			
			//TODO: load the map during this time, load player when they leave this screen
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
						//OTTextSprite pCount = (Instantiate(Resources.Load("TextPrefab")) as GameObject).GetComponent<OTTextSprite>();
						//pCount.text = "Player count: " + (Mathf.Clamp(PhotonNetwork.countOfPlayersInRooms - 1, 0, 20));
						//pCount.transform.position = new Vector3((Screen.width - 400) / 2, (Screen.height - 100) / 2, 0);
						GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 100) / 2, 400, 100));
							GUILayout.Label("Player count: " + (Mathf.Clamp(PhotonNetwork.countOfPlayersInRooms - 1, 0, 20)), companyStyle);
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
					//gamemode info
					GUIStyle killStyle = new GUIStyle();
					killStyle.fontSize = 32;
					killStyle.alignment = TextAnchor.MiddleCenter;
					killStyle.normal.textColor = Color.white;
					GUILayout.BeginArea(new Rect((Screen.width - 600) / 2, 10, 600, 200));
						GUILayout.Label("Free-For-All", killStyle);
						GUILayout.Space(20);
						GUILayout.Label("Kills to win: " + KILLS_TO_WIN, killStyle);
					GUILayout.EndArea();
					
					//check pause button
					if (Input.GetButtonDown("Pause"))
					{
						gameState = GameState.Paused;
					}
					break;
				}
			
			case GameState.Paused:
				{
					GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));
			
						//return to game
						if (GUILayout.Button("Return to Game"))
						{
							gameState = GameState.InGame;
						}
			
						//mute button
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
			
						//quit button
						if (GUILayout.Button("Quit"))
						{
							Application.Quit();
						}
					GUILayout.EndArea();
			
					//check pause button
					if (Input.GetButtonDown("Pause"))
					{
						gameState = GameState.InGame;
					}
					break;
				}
			
			case GameState.Leaderboard:
				{
					//win or lose text
					GUIStyle leaderboardStyle = new GUIStyle();
					leaderboardStyle.fontSize = 50;
					leaderboardStyle.alignment = TextAnchor.MiddleCenter;
					leaderboardStyle.normal.textColor = Color.white;
					GUILayout.BeginArea(new Rect((Screen.width - 800) / 2, (Screen.height + 200) / 2, 800, 300));
			
					if (GameObject.Find("PlayerOnePrefab(Clone)").GetComponent<playerUpdate>().killScore >= 15 ||
						GameObject.Find("PlayerTwoPrefab(Clone)").GetComponent<playerUpdate>().killScore >= 15)
					{
						GUILayout.Label("You won this round!", leaderboardStyle);
					
					}
					else
					{
						GUILayout.Label("You lost this round.", leaderboardStyle);
					}
					GUILayout.Space(30);
					GUILayout.Label("New round in: " + timeRemaining, leaderboardStyle);
					GUILayout.EndArea();
			
			
					if (timer.isFinished)
					{
						timeRemaining--;
						timer.Reset();
					}
					else if (timer2.isFinished)
					{
						GameObject.Find("PlayerOnePrefab(Clone)").GetComponent<playerUpdate>().ResetRound();
						if (p2exists)
							GameObject.Find("PlayerTwoPrefab(Clone)").GetComponent<playerUpdate>().ResetRound();
						timer.Reset();
						timer2.Reset();
						gameState = GameState.InGame;
					}
					else if (!timer.isRunning)
					{
						timer.Countdown(1f);
					}
					else if (!timer2.isRunning)
					{
						timer2.Countdown(10f);
					}
					else
					{
						timer.Update();
						timer2.Update();
					}
					break;
				}
		}
		
		/* // ========= OLD MENU CODE, USED FOR REFERENCE ===========
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
	
	/// <summary>
	/// Called when the local player joins a room.
	/// </summary>
	void OnJoinedRoom()
	{	
		GameObject playerGameObject = PhotonNetwork.Instantiate("PlayerOnePrefab", spawnPoint, Quaternion.identity, 0);
		player = playerGameObject.GetComponent<OTAnimatingSprite>();
		map = (Instantiate(Resources.Load("MapPrefab")) as GameObject).GetComponent<OTTileMap>();
		player.GetComponent<playerUpdate>().map = map;
	}
	
	//TODO: Remove stuff if you leave a room
	void OnLeftLobby()
	{
		//delete all the stuff
		//print ("a player left the room");
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{
		//check for input from a second player
		if (Input.GetButtonDown("P2join") && !p2exists && gameState == GameState.InGame)
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
		
		//update camera position and zoom
		OT.view.position = cameraPosition;
		OT.view.zoom = cameraZoom;
	}
}
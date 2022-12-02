using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

//TODO should possibly be singleton
public class PUNPlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject networkedPlayerPrefab;
    
    private RoomOptions _roomSettings = new RoomOptions();
    private string _roomName = TEST_ROOM;
    private GameObject _playerInstance;

    private const byte MAX_PLAYERS = 0;        //TODO change eventually
    private const string NEW_ROOM_NAME = "NewRoom";        //TODO don't hardcode
    private const string TEST_ROOM = "TestRoom";        //TODO don't hardcode


    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        _roomSettings.MaxPlayers = MAX_PLAYERS;
        Connect();
        
        if (PhotonNetwork.IsConnectedAndReady && !_playerInstance)
        {
            Setup();
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //TODO check which scene is loaded, instantiate corresponding prefab
        //TODO use ObjectPool
        InstantiatePlayer();
    }

    public override void OnConnectedToMaster()
    {
        DebugLogger.Debug(nameof(OnConnectedToMaster), $"{nameof(PhotonNetwork.LocalPlayer)}={PhotonNetwork.LocalPlayer} has connected to master.", this);

        PhotonNetwork.JoinOrCreateRoom(_roomName, _roomSettings, TypedLobby.Default);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        DebugLogger.Debug(nameof(OnJoinRoomFailed), $"{nameof(PhotonNetwork.LocalPlayer)}={PhotonNetwork.LocalPlayer} Join room failed -- creating room.", this);

        //TODO don't hardcode room name
        PhotonNetwork.CreateRoom(NEW_ROOM_NAME, _roomSettings, TypedLobby.Default);
    }
    
    public override void OnJoinedRoom()
    {
        DebugLogger.Debug(nameof(OnJoinedRoom), $"{nameof(PhotonNetwork.LocalPlayer)}={PhotonNetwork.LocalPlayer} has joined room.", this);

        //TODO use ObjectPool
        //TODO don't always instantiate same player prefab, scene dependant
        Setup();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        DebugLogger.Error(nameof(OnCreateRoomFailed), $"{nameof(PhotonNetwork.LocalPlayer)}={PhotonNetwork.LocalPlayer} Failed to create room -- room already exists", this);
    }
    
    private void Connect()
    {
        DebugLogger.Debug(nameof(Connect), $"{nameof(PhotonNetwork.LocalPlayer)}={PhotonNetwork.LocalPlayer} is connecting.", this);

        // #Critical, we must first and foremost connect to Photon Online Server.
        //PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        DebugLogger.Debug(nameof(OnPlayerLeftRoom), $"{nameof(PhotonNetwork.LocalPlayer)}={PhotonNetwork.LocalPlayer}: otherPlayer={otherPlayer} has left room.", this);

        if (otherPlayer.IsInactive)
        {
            DebugLogger.Info(nameof(OnPlayerLeftRoom), $"{nameof(PhotonNetwork.LocalPlayer)}={PhotonNetwork.LocalPlayer}: otherPlayer={otherPlayer} IsInactive.", this);
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            DebugLogger.Debug(nameof(OnPlayerLeftRoom), $"{nameof(PhotonNetwork.IsMasterClient)}. Calling {nameof(PhotonNetwork.DestroyPlayerObjects)} on {nameof(otherPlayer)}={otherPlayer}", this);
            PhotonNetwork.DestroyPlayerObjects(otherPlayer);
        }
        
        //Unsubscribe();
    }
    
    private void Setup()
    {
        InstantiatePlayer();
        //Subscribe();
    }
    
    private void InstantiatePlayer()
    {
        if (_playerInstance)
        {
            DebugLogger.Info(nameof(InstantiatePlayer), $"{nameof(_playerInstance)} already exists.", this);
            return;
        }
        
        _playerInstance = PhotonNetwork.Instantiate(networkedPlayerPrefab.name, Vector3.zero, Quaternion.identity, 0);
        if (!_playerInstance)
        {
            DebugLogger.Error(nameof(InstantiatePlayer), $"{nameof(_playerInstance)} is null.", this);
            return;
        }
    }
    
    private void Subscribe()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        //TODO scene unloaded or changed?
    }

    private void Unsubscribe()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        //TODO scene unloaded or changed?
    }
}
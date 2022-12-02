using Photon.Realtime;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;
using VRUiKits.Utils;

public class NetworkMenuController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject menuOptions;
    [SerializeField] private GameObject roomListItemPrefab;
    [SerializeField] private GameObject noRoomsAvailableNotify;
    [SerializeField] private GameObject connectingNotification;
    [SerializeField] private GameObject passwordGameObject;
    [SerializeField] private InputField passwordInputField;
    [SerializeField] private GameObject roomListPanel;
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private GameObject numpad;
    [SerializeField] private GameObject keyboard;
    [SerializeField] private XRUIInputModule xrUiInputModule;
    [SerializeField] private string roomName;
    [SerializeField] private string password;

    
    [Header("Level Loading Settings")] 
    //[SerializeField] private SceneHandler sceneHandler;
    [SerializeField] private string sceneName;
    [SerializeField] private UIKitInputField roomNameInputField;
    
    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, GameObject> playerListEntries;
    private const string ISREADY = "IsReady";
    private string[] customPropertiesForLobby;

    public string levelToLoadTesting;
    public GameObject viewport;
    public GameObject roomList;


    #region UNITY

    public void Start()
    {
        //sceneHandler = SceneHandler.Instance;
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();
        
        //Make sure this is forced on for UI. Developer is aware of bug.
        if (xrUiInputModule)
        {
            xrUiInputModule.enabled = true;    
        }
        else
        {
            DebugLogger.Error("Awake", "XR Input module missing. Typically found on event system.", this);
        }
        
        Connect();
    }

    public void Connect()
    {
        if (PhotonNetwork.InLobby)
            return;

        DebugLogger.Info("Connect", "Connect being called.", this);
        
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        connectingNotification.SetActive(true);
    }

    public void CreateOfflineRoom()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        
        PhotonNetwork.CreateRoom(roomName, new RoomOptions {
            IsVisible = false,
            MaxPlayers = 1
        });
    }

    #endregion

    #region PUN CALLBACKS

    public override void OnConnectedToMaster()
    {
        DebugLogger.Info("OnConnectedToMaster", "OnConnectedToMaster being called.", this);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        DebugLogger.Info("OnJoinedLobby", "OnJoinedLobby being called.", this);
        connectingNotification.SetActive(false);
    }
    

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();
        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
        ClearRoomListView();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        DebugLogger.Info("OnCreateRoomFailed", "OnCreateRoomFailed being called.", this);
        menuOptions.SetActive(true);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        DebugLogger.Info("OnJoinRoomFailed", "OnJoinRoomFailed being called.", this);

        menuOptions.SetActive(true);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        DebugLogger.Info("OnJoinRandomFailed", "OnJoinRandomFailed being called.", this);

        CreateRoom();
    }

    public override void OnJoinedRoom()
    {
        DebugLogger.Info("OnJoinedRoom", "OnJoinedRoom being called.", this);

       
        /*if (sceneHandler)
        {
            sceneHandler.StartLoadTransition(sceneName);
        }
        else
        {
            DebugLogger.Error("OnJoinedRoom", "Scene Handler is missing.", this);
        }*/
        
        var customPlayerProperties = new ExitGames.Client.Photon.Hashtable();
        customPlayerProperties.Add(ISREADY, false);
        PhotonNetwork.LocalPlayer.SetCustomProperties(customPlayerProperties);
        
        PhotonNetwork.LoadLevel(levelToLoadTesting);
    }
    
    #endregion

    #region UI CALLBACKS

    public void OnBackButtonClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        menuOptions.SetActive(true);
    }

    public void OnJoinRandomRoomButtonClicked()
    {
        //sceneHandler.StartLoadTransitionForJoiningRoom();
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    #endregion

    private void ClearRoomListView()
    {
        foreach (GameObject entry in roomListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        roomListEntries.Clear();
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            // Update cached room info
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            // Add new room info to cache
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
    }

    private void UpdateRoomListView()
    {
        if (cachedRoomList.Count == 0)
        {
            //Display text notifying players they need to create room.
            connectingNotification.SetActive(false);
            noRoomsAvailableNotify.SetActive(true);
        }
        else
        {
            connectingNotification.SetActive(false);
            noRoomsAvailableNotify.SetActive(false);
            roomList.SetActive(true);
            viewport.SetActive(true);
        }

        foreach (RoomInfo info in cachedRoomList.Values)
        {
            GameObject entry = Instantiate(roomListItemPrefab);
            entry.transform.SetParent(viewport.transform);
            entry.transform.localPosition = Vector3.zero;
            entry.transform.localScale = Vector3.one;
            
            string nameOfMasterClient = info.CustomProperties["masterClient"].ToString();
            string ping = info.CustomProperties["ping"].ToString();
            string password = info.CustomProperties["password"].ToString();
            entry.GetComponent<RoomListItem>().Initialize(info.Name, (byte) info.PlayerCount, info.MaxPlayers, nameOfMasterClient, ping, password);
            roomListEntries.Add(info.Name, entry);
        }
    }
    
    private string CreateRandomString(int stringLength = 4) {
        int _stringLength = stringLength - 1;
        string randomString = "";
        string[] characters = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
        for (int i = 0; i <= _stringLength; i++) {
            randomString = randomString + characters[Random.Range(0, characters.Length)];
        }
        return randomString;
    }

    public void CreateRoom()
    {
        DebugLogger.Info("CreateRoom", "CreateRoom being called.", this);

        if (PhotonNetwork.LocalPlayer.NickName == "")
        {
            PhotonNetwork.LocalPlayer.NickName = "Player" + Random.Range(1000, 2000);
        }

        if (roomNameInputField.text == "")
        {
            roomName = CreateRandomString();
        }

        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        
        var customRoomPropertiesToSet = new ExitGames.Client.Photon.Hashtable();
        customRoomPropertiesToSet.Add("masterClient", PhotonNetwork.LocalPlayer.NickName);
        customRoomPropertiesToSet.Add("ping", PhotonNetwork.GetPing().ToString());
        customRoomPropertiesToSet.Add("password", passwordInputField.text);
        
        customPropertiesForLobby = new string[3];
        customPropertiesForLobby[0] = "masterClient";
        customPropertiesForLobby[1] = "ping";
        customPropertiesForLobby[2] = "password";

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;
        options.PublishUserId = true;
        options.CustomRoomProperties = customRoomPropertiesToSet;
        options.CustomRoomPropertiesForLobby = customPropertiesForLobby;
        
        PhotonNetwork.CreateRoom(roomName, options, null);
    }
    
    public void OnPrivateRoomButtonClicked()
    {
        passwordGameObject.SetActive(true);
        passwordInputField.text = CreatePasswordPin();
    }
    
    public string CreatePasswordPin()
    {
        int _min = 1000;
        int _max = 9999;
        int rndNum = Random.Range(_min, _max);
        password = rndNum.ToString();
        return password;
    }
    
    public void CreateRoomNotificationButtonPressed()
    {
        roomListPanel.SetActive(false);
        createRoomPanel.SetActive(true);
    }

    public void OnKeyboardEnterClicked()
    {
        keyboard.SetActive(false);
        roomName = roomNameInputField.text;
    }
    
    public void OnNumpadEnterClicked()
    {
        numpad.SetActive(false);
        
        if (passwordInputField.text == "")
        {
            passwordInputField.text = password;
        }
        else
        {
            password = passwordInputField.text;
        }
    }

    [Button]
    public void TestRoomItemPosition()
    {
        GameObject entry = Instantiate(roomListItemPrefab);
        entry.transform.SetParent(viewport.transform, false);
        entry.transform.localPosition = Vector3.zero;
        entry.transform.localScale = Vector3.one;
        Canvas.ForceUpdateCanvases();
    }
}


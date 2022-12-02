using System;
using System.Collections;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    public Text RoomNameText;
    public Text RoomPlayersText;
    public Text masterClientName;
    public Text pingText;
    public GameObject joinRoomButtonObject;
    public Button JoinRoomButton;
    public GameObject passwordInputField;
    public InputField numpadText;
    private string roomName;
    private string masterClient;
    //private SceneHandler sceneHandler;
    private string roomPassword;

    private void Awake()
    {
        //sceneHandler = SceneHandler.Instance;
    }

    public void Start() 
    {
        JoinRoomButton.onClick.AddListener(() =>
        {
            if (roomPassword == "")
            {
                if (PhotonNetwork.InLobby)
                {
                    PhotonNetwork.LeaveLobby();
                }

                joinRoomButtonObject.SetActive(false);
                PhotonNetwork.JoinRoom(roomName);
                /*if (sceneHandler)
                {
                    sceneHandler.StartLoadTransitionForJoiningRoom(roomName);
                    joinRoomButtonObject.SetActive(false);
                }*/
            }
            else
            {
                //Display panel in order to enter password.
                passwordInputField.SetActive(true);
                joinRoomButtonObject.SetActive(false);
            }
        });
    }

    public void Initialize(string name, byte currentPlayers, byte maxPlayers, string masterClient, string ping, string password)
    {
        roomName = name;
        RoomNameText.text = name;
        RoomPlayersText.text = "Players: " + currentPlayers + "/" + maxPlayers;
        pingText.text = "Ping: " + ping;
        masterClientName.text = masterClient;
        roomPassword = password;
    }

    public void OnNumpadEnter()
    {
        if (numpadText.text == roomPassword)
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            PhotonNetwork.JoinRoom(roomName);
            /*if (sceneHandler)
            {
                sceneHandler.StartLoadTransitionForJoiningRoom(roomName);
            }*/
        }
        else
        {
            Debug.Log("Incorrect password. Try again.");
        }
    }

    [Button]
    public void CLick()
    {
        if (roomPassword == "")
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            joinRoomButtonObject.SetActive(false);
            PhotonNetwork.JoinRoom(roomName);
            /*if (sceneHandler)
            {
                sceneHandler.StartLoadTransitionForJoiningRoom(roomName);
                joinRoomButtonObject.SetActive(false);
            }*/
        }
        else
        {
            //Display panel in order to enter password.
            passwordInputField.SetActive(true);
            joinRoomButtonObject.SetActive(false);
        }
    }
}
using System;
using System.Net;
using Photon.Pun;
using UnityEngine;

public class TeamHandler : MonoBehaviour
{
    [SerializeField] private PhotonView photonView;
    public bool isTeamInPossession;
    public int _teamNumber = -1;
    public TeamSetup teamSetup;
    
    // Start is called before the first frame update
    void Start()
    {
        teamSetup = GameObject.Find("Team Setup").GetComponent<TeamSetup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!teamSetup) return;
        if (_teamNumber == teamSetup.GetTeamInPossession() && _teamNumber != -1)
        {
            isTeamInPossession = true;
        }
        else
        {
            isTeamInPossession = false;
        }
    }

    public void RpcSetTeam(int teamNum)
    {
        if (!photonView.IsMine) return;
        photonView.RPC("SetTeam", RpcTarget.All, teamNum);
    }
    
    [PunRPC]
    private void SetTeam(int teamNum)
    {
        _teamNumber = teamNum;
    }
    
    public int GetTeam()
    {
        return _teamNumber;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class TeamSetup : MonoBehaviour
{
    [SerializeField] private int teamInPossession;
    public PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    public void SetTeamInPossession(int teamNum)
    {
        teamInPossession = teamNum;
    }

    public int GetTeamInPossession()
    {
        return teamInPossession;
    }
    
    public void RpcSetTeamInPossession(int teamNum)
    {
        if (!photonView.IsMine) return;
        photonView.RPC("SetTeamInPossession", RpcTarget.All, teamNum);
    }
}

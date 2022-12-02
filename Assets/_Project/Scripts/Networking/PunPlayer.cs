using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PunPlayer : MonoBehaviour
{
    [SerializeField] private PhotonView photonView;

    private GameObject _thisGameObject;
    private int _photonViewId;
    private PUNPlayerList _punPlayerList;

    private void Awake()
    {
        if (DebugLogger.IsNullWarning(photonView, this, "Should be set in editor. Attempting to find."))
        {
            photonView = GetComponent<PhotonView>();
            if (DebugLogger.IsNullError(photonView, this, "Should be set in editor. Not found.")) return;
        }
        
        photonView.Owner.TagObject = this;
        _photonViewId = photonView.ViewID;
        _thisGameObject = gameObject;

        AddSelfToPlayerList();
    }

    private void OnDestroy()
    {
        RemoveSelfFromPlayerList();
    }

    private void AddSelfToPlayerList()
    {
        _punPlayerList = FindObjectOfType<PUNPlayerList>();
        if (DebugLogger.IsNullInfo(_punPlayerList, this, "Not found in scene.")) return;
        
        _punPlayerList.AddPlayer(_thisGameObject);
    }
    
    private void RemoveSelfFromPlayerList()
    {
        _punPlayerList = FindObjectOfType<PUNPlayerList>();
        if (DebugLogger.IsNullInfo(_punPlayerList, this, "Not found in scene.")) return;
        
        _punPlayerList.RemovePlayer(this);
    }

    public int GetPhotonViewId()
    {
        return _photonViewId;
    }
}

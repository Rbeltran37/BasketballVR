using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    [SerializeField] private PhotonView thisPhotonView;
    [SerializeField] private Transform headAnchor;
    [SerializeField] private Transform leftAnchor;
    [SerializeField] private Transform rightAnchor;
    [SerializeField] private Transform headAlias;
    [SerializeField] private Transform leftAlias;
    [SerializeField] private Transform rightAlias;
    [SerializeField] private GameObject localGameObject;
    [SerializeField] private GameObject remoteGameObject;
    
    private bool IsRemoteClient => thisPhotonView && !thisPhotonView.IsMine;
    

    private void Awake()
    {
        //if (PhotonNetwork.OfflineMode) return;

        if (!thisPhotonView)
        {
            DebugLogger.Warning(nameof(Awake), $"{thisPhotonView} is null.", this);
        }
        
        if (IsRemoteClient)
        {
            SetupRemoteClient();
        }
        else
        {
            SetupLocalClient();
        }
    }

    private void SetupRemoteClient()
    {
        DeparentAliases();

        if (!localGameObject)
        {
            DebugLogger.Error(nameof(SetupRemoteClient), $"{localGameObject} is null. Must be set in editor.", this);
            return;
        }
        
        localGameObject.SetActive(false);
    }
    
    private void DeparentAliases()
    {
        if (!headAlias)
        {
            DebugLogger.Error(nameof(DeparentAliases), $"{headAlias} is null. Must be set in editor.", this);
            return;
        }

        headAlias.SetParent(null);
        
        if (!leftAlias)
        {
            DebugLogger.Error(nameof(DeparentAliases), $"{leftAlias} is null. Must be set in editor.", this);
            return;
        }
        
        leftAlias.SetParent(null);
        
        if (!rightAlias)
        {
            DebugLogger.Error(nameof(DeparentAliases), $"{rightAlias} is null. Must be set in editor.", this);
            return;
        }

        rightAlias.SetParent(null);
    }

    private void SetupLocalClient()
    {
        ParentAliases();
        
        if (!remoteGameObject)
        {
            DebugLogger.Error(nameof(SetupLocalClient), $"{remoteGameObject} is null. Must be set in editor.", this);
            return;
        }
        
        remoteGameObject.SetActive(false);
    }

    private void ParentAliases()
    {
        if (!headAlias)
        {
            DebugLogger.Error(nameof(ParentAliases), $"{headAlias} is null. Must be set in editor.", this);
            return;
        }
        
        if (!headAnchor)
        {
            DebugLogger.Error(nameof(ParentAliases), $"{headAnchor} is null. Must be set in editor.", this);
            return;
        }

        headAlias.SetParent(headAnchor);
        
        if (!leftAlias)
        {
            DebugLogger.Error(nameof(ParentAliases), $"{leftAlias} is null. Must be set in editor.", this);
            return;
        }
        
        if (!leftAnchor)
        {
            DebugLogger.Error(nameof(ParentAliases), $"{leftAnchor} is null. Must be set in editor.", this);
            return;
        }
        
        leftAlias.SetParent(leftAnchor);
        
        if (!rightAlias)
        {
            DebugLogger.Error(nameof(ParentAliases), $"{rightAlias} is null. Must be set in editor.", this);
            return;
        }
        
        if (!rightAnchor)
        {
            DebugLogger.Error(nameof(ParentAliases), $"{rightAnchor} is null. Must be set in editor.", this);
            return;
        }

        rightAlias.SetParent(rightAnchor);
    }
}

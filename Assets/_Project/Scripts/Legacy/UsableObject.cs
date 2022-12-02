using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class UsableObject : MonoBehaviour, IUsable
{
    [SerializeField] protected PhotonView ThisPhotonView;
    
    public UnityEvent WasUsed;


    //Only local player calls
    public void AttemptUse(Interactor interactor)
    {
        Use();
        SendUse();
    }

    public void Use()
    {
        WasUsed?.Invoke();
    }

    protected void SendUse()
    {
        if (PhotonNetwork.OfflineMode) return;
        
        if (!ThisPhotonView)
        {
            DebugLogger.Error(nameof(SendUse), $"{nameof(ThisPhotonView)} is null. Must be set in editor.", this);
            return;
        }
        
        ThisPhotonView.RPC(nameof(RPCUse), RpcTarget.OthersBuffered);
    }

    [PunRPC]
    protected void RPCUse()
    {
        Use();
    }
}

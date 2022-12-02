using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.Events;

public class Interactable : GrabbableObject, IUsable
{
    public UnityEvent WasUsed;

    
    //Only local player calls
    public void AttemptUse(Interactor interactor)
    {
        if (CurrentInteractor == null) return;
        if (!CurrentInteractor.Equals(interactor)) return;
        
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
        
        if (DebugLogger.IsNullError(ThisPhotonView, this, "Must be set in editor.")) return;

        ThisPhotonView.RPC(nameof(RPCUse), RpcTarget.OthersBuffered);
    }

    [PunRPC]
    protected void RPCUse()
    {
        Use();
    }
}

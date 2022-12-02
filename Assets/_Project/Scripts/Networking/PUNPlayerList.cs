using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class PUNPlayerList : MonoBehaviour
{
    [SerializeField] private PhotonView photonView;
    [SerializeField] private PUNPlayerManager punPlayerManager;

    public HashSet<PunPlayer> _players = new HashSet<PunPlayer>();
    private Dictionary<Player, int> _playerIndexes = new Dictionary<Player, int>();
    private int _currentPlayerIndex;

    public Action<PunPlayer> WasAdded;
    public Action<PunPlayer> WasRemoved;


    private void Awake()
    {
        if (DebugLogger.IsNullWarning(punPlayerManager, this, "Should be set in editor. Attempting to find..."))
        {
            punPlayerManager = FindObjectOfType<PUNPlayerManager>();
            if (DebugLogger.IsNullError(punPlayerManager, this, "Should be set in editor. Unable to find.")) return;
        }
    }

    public List<PunPlayer> GetPlayers()
    {
        return _players.ToList();
    }

    public int GetNumPlayers()
    {
        return _players.Count;
    }
    
    public void AddPlayer(GameObject playerInstance)
    {
        if (DebugLogger.IsNullError(playerInstance, this)) return;

        var playerPhotonView = playerInstance.GetComponent<PhotonView>();
        if (DebugLogger.IsNullError(playerPhotonView, this)) return;

        SetPlayerTagObject(playerPhotonView);
        
        photonView.RPC(nameof(RPCAddPlayer), RpcTarget.AllBuffered, playerPhotonView.ViewID);
    }

    private void SetPlayerTagObject(PhotonView playerPhotonView)
    {
        if (DebugLogger.IsNullError(playerPhotonView, this)) return;

        var player = playerPhotonView.GetComponent<PunPlayer>();
        if (DebugLogger.IsNullError(player, this)) return;

        var owner = playerPhotonView.Owner;
        owner.TagObject = player;
    }

    [PunRPC]
    private void RPCAddPlayer(int photonViewId)
    {
        var playerPhotonView = PhotonNetwork.GetPhotonView(photonViewId);
        if (DebugLogger.IsNullError(playerPhotonView, this)) return;

        SetPlayerTagObject(playerPhotonView);
        AttemptAddPlayerIndex(playerPhotonView.Owner);
        
        var owner = playerPhotonView.Owner;
        var player = (PunPlayer)owner.TagObject;
        AddPlayer(player);
    }

    private void AttemptAddPlayerIndex(Player player)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (_playerIndexes.ContainsKey(player)) return;

        AddPlayerIndex(player, _currentPlayerIndex);
        SendAddPlayerIndex(player, _currentPlayerIndex);
        _currentPlayerIndex++;
    }

    private void AddPlayerIndex(Player player, int index)
    {
        if (_playerIndexes.ContainsKey(player)) return;
        
        _playerIndexes.Add(player, index);
    }

    private void SendAddPlayerIndex(Player player, int index)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        photonView.RPC(nameof(RPCAddPlayerIndex), RpcTarget.OthersBuffered, player, index);
    }

    [PunRPC]
    private void RPCAddPlayerIndex(Player player, int index)
    {
        AddPlayerIndex(player, index);
    }
    
    private void AddPlayer(PunPlayer punPlayerToAdd)
    {
        if (DebugLogger.IsNullError(punPlayerToAdd, this)) return;

        foreach (var player in _players)
        {
            if (DebugLogger.IsNullWarning(punPlayerToAdd, this)) return;

            if (player.Equals(punPlayerToAdd))
            {
                DebugLogger.Warning(MethodBase.GetCurrentMethod().Name, $"{punPlayerToAdd} is already in playerTargets.", this);
                return;
            }
        }

        _players.Add(punPlayerToAdd);
        
        WasAdded?.Invoke(punPlayerToAdd);
    }
    
    public void RemovePlayer(PunPlayer punPlayer)
    {
        if (DebugLogger.IsNullError(punPlayer, this)) return;

        var photonViewId = punPlayer.GetPhotonViewId();
        
        photonView.RPC(nameof(RPCRemovePlayer), RpcTarget.AllBuffered, photonViewId);
    }

    [PunRPC]
    private void RPCRemovePlayer(int photonViewId)
    {
        var playerPhotonView = PhotonNetwork.GetPhotonView(photonViewId);
        if (DebugLogger.IsNullError(playerPhotonView, this)) return;

        var owner = playerPhotonView.Owner;
        var player = (PunPlayer)owner.TagObject;
        RemoveThisPlayer(player);
    }
    
    private void RemoveThisPlayer(PunPlayer punPlayerToRemove)
    {
        _players.Remove(punPlayerToRemove);
        WasRemoved?.Invoke(punPlayerToRemove);
    }

    [Button]
    public void PrintPlayers()
    {
        var line = "";
        foreach (var playerTarget in _players)
        {
            line += playerTarget.ToString() + ", ";
        }
        
        print(line);
    }
    
    public PunPlayer GetRandomPlayer()
    {
        if (DebugLogger.IsNullOrEmptyWarning(_players, this)) return null;
        
        var randomIndex = Random.Range(0, _players.Count);
        return _players.ToList()[randomIndex];
    }

    public int GetPlayerIndex(Player player)
    {
        if (!_playerIndexes.ContainsKey(player)) return _currentPlayerIndex;
        
        return _playerIndexes[player];
    }
}

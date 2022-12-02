using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CoroutineCaller : Singleton<CoroutineCaller>
{
    private static bool _isLocked;
    private static int _numLocks = NUM_CALLS_PER_FRAME;
    
    private const float WAIT_INTERVAL = .1f;
    private const int NUM_CALLS_PER_FRAME = 5;


    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public static void WaitToConnect(IEnumerator coroutine)
    {
        Instance.StartCoroutine(WaitToConnectCoroutine(coroutine));
    }
    
    public static void WaitToConnect(Action callback)
    {
        Instance.StartCoroutine(WaitToConnectCoroutine(callback));
    }

    private static IEnumerator WaitToConnectCoroutine(IEnumerator coroutine)
    {
        if (PhotonNetwork.OfflineMode) yield break;
        
        while (!PhotonNetwork.IsConnectedAndReady)
        {
            yield return new WaitForSeconds(WAIT_INTERVAL);
        }

        Instance.StartCoroutine(coroutine);
    }
    
    private static IEnumerator WaitToConnectCoroutine(Action callback)
    {
        if (PhotonNetwork.OfflineMode) yield break;
        
        while (!PhotonNetwork.IsConnectedAndReady)
        {
            yield return new WaitForSeconds(WAIT_INTERVAL);
        }

        callback.Invoke();
    }
    
    public static void CachePerFrame(IEnumerator call)
    {
        Instance.StartCoroutine(CachePerFrameCoroutine(call));
    }

    private static IEnumerator CachePerFrameCoroutine(IEnumerator call)
    {
        while (_isLocked)
        {
            yield return null;
        }
        
        _isLocked = true;
        yield return Instance.StartCoroutine(call);
        _isLocked = false;
    }

    public static void CachePerCall(IEnumerator call)
    {
        Instance.StartCoroutine(CachePerCallCoroutine(call));
    }
    
    private static IEnumerator CachePerCallCoroutine(IEnumerator call)
    {
        while (_numLocks == 0)
        {
            yield return null;
        }

        _numLocks--;
        yield return Instance.StartCoroutine(call);
        _numLocks++;
    }
}

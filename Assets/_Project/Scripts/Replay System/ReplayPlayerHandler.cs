using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayPlayerHandler : MonoBehaviour
{
    [SerializeField] private ReplayCapture headCapture;
    [SerializeField] private ReplayCapture leftHandCapture;
    [SerializeField] private ReplayCapture rightHandCapture;
    [SerializeField] private GameObject ghostPrefab;
    
    private GameObject _ghost;
    
    private ReplayController _replayController;

    // Start is called before the first frame update
    void Start()
    {
        _replayController = FindObjectOfType<ReplayController>();
        if (!_replayController) return;
        
        AddPlayerReplayCaptures();
        AddGhostPlayer();
        _replayController.StartRecording();
    }

    private void OnDestroy()
    {
        RemovePlayerCaptures();
        RemoveGhostPlayer();
    }

    private void AddPlayerReplayCaptures()
    {
        if (!_replayController) return;
        _replayController.captures.Add(headCapture);
        _replayController.captures.Add(leftHandCapture);
        _replayController.captures.Add(rightHandCapture);
    }

    private void RemovePlayerCaptures()
    {
        if (!_replayController) return;
        _replayController.captures.Remove(headCapture);
        _replayController.captures.Remove(leftHandCapture);
        _replayController.captures.Remove(rightHandCapture);
    }

    private void AddGhostPlayer()
    {
        if (!_replayController) return;
        _ghost = Instantiate(ghostPrefab, _replayController.GetOffsetPosition(), Quaternion.identity);
        SetGhostReplayCaptures(_ghost);
    }

    private void SetGhostReplayCaptures(GameObject ghostGameObject)
    {
        if (!ghostGameObject) return;
        var ghostReplayReference = ghostGameObject.GetComponent<GhostReplayReference>();
        if (!ghostReplayReference) return;
        headCapture.transformToApplyTo = ghostReplayReference.GetHeadPosition();
        leftHandCapture.transformToApplyTo = ghostReplayReference.GetLeftHandPosition();
        rightHandCapture.transformToApplyTo = ghostReplayReference.GetRightHandPosition();
    }
    
    private void RemoveGhostPlayer()
    {
        Destroy(_ghost);
    }
}

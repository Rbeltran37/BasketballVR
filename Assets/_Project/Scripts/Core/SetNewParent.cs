using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNewParent : MonoBehaviour
{
    [SerializeField] private Transform newParent;
    [SerializeField] private Vector3 newLocalPosition;
    [SerializeField] private Vector3 newLocalEuler;
    [SerializeField] private bool setCameraAsParent;
    [SerializeField] private CustomEnums.Execution execution;

    private void Awake()
    {
        if (execution != CustomEnums.Execution.Awake) return;

        if (setCameraAsParent) SetCameraAsNewParent();
        
        PositionUnderNewParent();
    }

    private void OnEnable()
    {
        if (execution != CustomEnums.Execution.OnEnable) return;

        if (setCameraAsParent) SetCameraAsNewParent();
        
        PositionUnderNewParent();
    }

    private void Start()
    {
        if (execution != CustomEnums.Execution.Start) return;

        if (setCameraAsParent) SetCameraAsNewParent();
        
        PositionUnderNewParent();
    }

    private void SetCameraAsNewParent()
    {
        if (Camera.main) newParent = Camera.main.transform;
        if (DebugLogger.IsNullError(newParent, this)) return;
    }
    
    private void PositionUnderNewParent()
    {
        Transform thisTransform;
        (thisTransform = transform).SetParent(newParent);
        thisTransform.localPosition = newLocalPosition;
        thisTransform.localEulerAngles = newLocalEuler;
    }
}

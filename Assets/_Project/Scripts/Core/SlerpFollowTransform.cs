using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlerpFollowTransform : MonoBehaviour
{
    [SerializeField] private Transform thisTransform;
    [SerializeField] private Transform transformToFollow;
    [SerializeField] private float slerpRatio = .1f;
    [SerializeField] private AxisToFollow axisToFollow = AxisToFollow.All;
    
    
    private enum AxisToFollow
    {
        All,
        X,
        Y,
        Z,
    }

    private void Awake()
    {
        if (DebugLogger.IsNullWarning(thisTransform, this, "Should be set in editor")) thisTransform = transform;
    }

    private void LateUpdate()
    {
        if (!transformToFollow) return;

        var transformRotation = thisTransform.rotation;
        var transformToFollowRotation = transformToFollow.rotation;
        var x = axisToFollow == AxisToFollow.All || axisToFollow == AxisToFollow.X ? transformToFollowRotation.x : transformRotation.x;
        var y = axisToFollow == AxisToFollow.All || axisToFollow == AxisToFollow.Y ? transformToFollowRotation.y : transformRotation.y;
        var z = axisToFollow == AxisToFollow.All || axisToFollow == AxisToFollow.Z ? transformToFollowRotation.z : transformRotation.z;
        var w = transformToFollowRotation.w;
        var targetRotation = new Quaternion(x, y, z, w);
        thisTransform.rotation = Quaternion.Slerp(thisTransform.rotation, targetRotation, slerpRatio);
    }
}

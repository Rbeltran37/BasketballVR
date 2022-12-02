using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothDampFollow : MonoBehaviour
{
    [SerializeField] private Transform thisTransform;
    [SerializeField] private Transform transformToFollow;
    [SerializeField] private float smoothTime = 0.3F;

    private Vector3 _velocity;


    private void Update()
    {
        thisTransform.position = Vector3.SmoothDamp(thisTransform.position, transformToFollow.position, ref _velocity, smoothTime);
    }

    public void SetFollowTransform(Transform transform)
    {
        transformToFollow = transform;
    }
}

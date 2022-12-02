using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowTransform : MonoBehaviour
{
    [SerializeField] private Transform transformToFollow;
    [SerializeField] [Range(.1f, 1)] private float fractionToTravel = .5f;
    [SerializeField] private float maxDistance = 1;
    


    private void FixedUpdate()
    {
        transform.Translate((transformToFollow.position - transform.position) * fractionToTravel);
        //transform.rotation = transformToFollow.rotation;
    }
}

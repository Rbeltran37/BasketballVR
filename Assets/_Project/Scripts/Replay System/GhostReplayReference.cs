using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostReplayReference : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;

    public Transform GetHeadPosition()
    {
        return head;
    }
    
    public Transform GetLeftHandPosition()
    {
        return leftHand;
    }
    
    public Transform GetRightHandPosition()
    {
        return rightHand;
    }
}

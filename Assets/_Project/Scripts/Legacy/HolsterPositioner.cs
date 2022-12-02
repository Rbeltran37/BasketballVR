using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolsterPositioner : MonoBehaviour
{
    [SerializeField] private Transform leftWaistHolster;
    [SerializeField] private Transform rightWaistHolster;
    [SerializeField] private Transform headSet;
    [SerializeField] private Transform waistHolsterParent;
    [SerializeField] private float waistDistanceFromHeadset = .6f;
    [SerializeField] private float waistDistanceFromCenter = .3f;
    
    
    private void Awake()
    {
        SetHolsterDistance();
    }

    void LateUpdate()
    {
        var headSetPosition = headSet.position;
        
        waistHolsterParent.position = new Vector3(headSetPosition.x, 
            headSetPosition.y + -waistDistanceFromHeadset, headSetPosition.z);
        waistHolsterParent.localEulerAngles = new Vector3(0, waistHolsterParent.localEulerAngles.y, 0);
    }

    private void SetHolsterDistance() 
    {
        leftWaistHolster.localPosition = new Vector3(-waistDistanceFromCenter, 0, leftWaistHolster.localPosition.z);
        rightWaistHolster.localPosition = new Vector3(waistDistanceFromCenter, 0, rightWaistHolster.localPosition.z);
    }
}

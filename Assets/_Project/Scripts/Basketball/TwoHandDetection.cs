using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoHandDetection : MonoBehaviour
{
    private bool isTwoHandedGrabbed;
    private int interactorCount = 0;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Interactor>())
        {
            interactorCount++;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        isTwoHandedGrabbed = interactorCount == 2;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Interactor>())
        {
            interactorCount--;
        }
    }

    public bool IsGrabbedByTwoHands()
    {
        return isTwoHandedGrabbed;
    }
}

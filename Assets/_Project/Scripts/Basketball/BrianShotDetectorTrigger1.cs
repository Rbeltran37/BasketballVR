using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrianShotDetectorTrigger1 : MonoBehaviour
{
    public bool inFirstTrigger;
    private CodyBasketball basketball;
    [SerializeField] private BrianShotDetector shotDetector;

    private void OnTriggerEnter(Collider other)
    {
        basketball = other.GetComponent<CodyBasketball>();
        
        if (basketball)
        {
            inFirstTrigger = true;
            Debug.Log("attempt");
            //Debug.Log(inFirstTrigger);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        basketball = other.GetComponent<CodyBasketball>();
        
        if (basketball)
        {
            inFirstTrigger = false;

            if (shotDetector.isShotSuccessful == false)
            {
                Debug.Log("Miss");
            }
            //Debug.Log(inFirstTrigger);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotDetectorHelper : MonoBehaviour
{
    public bool hasEntered;
    public ShotDetector shotDetector;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CodyBasketball>())
        {
            hasEntered = true;
            Debug.Log("has entered top trigger");
        }
    }
}

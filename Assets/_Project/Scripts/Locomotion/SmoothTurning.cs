using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothTurning : MonoBehaviour
{
    [SerializeField] private Transform xrRig;
    [SerializeField] private JoystickInput joystickInput;
    [SerializeField] private float stickDeadZone;
    [SerializeField] private float rotationSpeed;
    

    // Update is called once per frame
    void Update()
    {
        var stickInput = joystickInput.GetStickPosition();
        var xAxis = stickInput.x;
        
        if (xAxis < 0 - stickDeadZone)
        {
            xrRig.Rotate(Vector3.down * (rotationSpeed * Time.deltaTime));
        }
        
        if (xAxis > 0 + stickDeadZone)
        {
            xrRig.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
        }
    }
}

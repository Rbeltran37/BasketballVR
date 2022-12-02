using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothLocomotion : MonoBehaviour
{
    [SerializeField] private Transform xrRig;
    [SerializeField] private Transform head;
    [SerializeField] private JoystickInput joystickInput;
    [SerializeField] private bool isLocomotionActive = true;
    [SerializeField] private float speed;
    [SerializeField] private bool isSprinting = false;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float jogSpeed;
    
    

    // Update is called once per frame
    void Update()
    {
        if(!isLocomotionActive) return;
        var stickPosition = joystickInput.GetStickPosition();
        var deltaTime = Time.deltaTime;
        var xAxis = stickPosition.x * speed * deltaTime;
        var zAxis = stickPosition.y * speed * deltaTime;
        var position = xrRig.position;
        Quaternion headYaw = Quaternion.Euler(0, head.transform.eulerAngles.y, 0);
        position += headYaw * new Vector3(xAxis, position.y, zAxis);
        xrRig.position = position;
    }

    public void Sprint()
    {
        isSprinting = true;
        speed = sprintSpeed;;
    }

    public void Jog()
    {
        isSprinting = false;
        speed = jogSpeed;
    }
    
    public bool IsSprinting()
    {
        return isSprinting;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapTurning : MonoBehaviour
{
    [SerializeField] private Transform xrRig;
    [SerializeField] private JoystickInput joystickInput;
    [SerializeField] private float stickDeadZone;
    [SerializeField] private float snapAngle;
    [SerializeField] private float turnTimeout;
    private float _time;

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;
        if (_time <= turnTimeout) return;
        var stickInput = joystickInput.GetStickPosition();
        var xAxis = stickInput.x;

        if (xAxis < 0 - stickDeadZone)
        {
            xrRig.Rotate(0,xrRig.rotation.y - snapAngle, 0);
            _time = 0;
            return;
        }
        
        if (xAxis > 0 + stickDeadZone)
        {
            xrRig.Rotate(0,xrRig.rotation.y + snapAngle, 0);
            _time = 0;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerAxisValues : MonoBehaviour
{
    private List<UnityEngine.XR.InputDevice> _leftDevice = new List<UnityEngine.XR.InputDevice>();
    private List<UnityEngine.XR.InputDevice> _rightDevice = new List<UnityEngine.XR.InputDevice>();
    
    
    private void Awake()
    {
        InitializeController();
    }

    private void InitializeController() 
    {
        _leftDevice = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesWithRole(UnityEngine.XR.InputDeviceRole.LeftHanded, _leftDevice);

        _rightDevice = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesWithRole(UnityEngine.XR.InputDeviceRole.RightHanded, _rightDevice);
    }

    public Vector2 GetAxisValueFromController(bool isLeftController) 
    {
        var thumbStickAxisValue = new Vector2();
        
        var currentDevice = isLeftController ? _leftDevice : _rightDevice;
        if (currentDevice.Count == 0) InitializeController();
        if (currentDevice.Count == 0) return thumbStickAxisValue;
        
        currentDevice[0].TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out thumbStickAxisValue);

        return thumbStickAxisValue;
    }
}

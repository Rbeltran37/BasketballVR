using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerHaptics : MonoBehaviour
{
    [SerializeField] private CustomEnums.Hand hand;
    [SerializeField] private ControllerHaptics otherControllerHaptics;

    private UnityEngine.XR.InputDevice _thisController;


    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        var handedness = hand == CustomEnums.Hand.Left
            ? UnityEngine.XR.InputDeviceRole.LeftHanded
            : UnityEngine.XR.InputDeviceRole.RightHanded;
        var devices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesWithRole(handedness, devices);

        if (devices.Count > 0) _thisController = devices[0];
    }

    public void ActivateHaptics(float amplitude, float duration) 
    {
        if (!_thisController.isValid) Initialize();

        UnityEngine.XR.HapticCapabilities capabilities;
        if (_thisController.TryGetHapticCapabilities(out capabilities)) {
            if (capabilities.supportsImpulse) {
                uint channel = 0;

                _thisController.SendHapticImpulse(channel, amplitude, duration);
            }
        }
    }
    
    public static void ActivateHaptics(float amplitude, float duration, bool isLeftController) 
    {
        if (isLeftController) ActivateLeftControllerHaptics(amplitude, duration);
        else ActivateRightControllerHaptics(amplitude, duration);
    }

    public ControllerHaptics GetOtherControllerHaptics()
    {
        return otherControllerHaptics;
    }
    
    private static void ActivateRightControllerHaptics(float amplitude, float duration)
    {
        var devices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesWithRole(UnityEngine.XR.InputDeviceRole.RightHanded, devices);

        foreach (var device in devices) {
            UnityEngine.XR.HapticCapabilities capabilities;
            if (device.TryGetHapticCapabilities(out capabilities)) {
                if (capabilities.supportsImpulse) {
                    uint channel = 0;

                    device.SendHapticImpulse(channel, amplitude, duration);
                }
            }
        }
    }
    
    private static void ActivateLeftControllerHaptics(float amplitude, float duration)
    {
        var devices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesWithRole(UnityEngine.XR.InputDeviceRole.LeftHanded, devices);

        foreach (var device in devices) {
            UnityEngine.XR.HapticCapabilities capabilities;
            if (device.TryGetHapticCapabilities(out capabilities)) {
                if (capabilities.supportsImpulse) {
                    uint channel = 0;

                    device.SendHapticImpulse(channel, amplitude, duration);
                }
            }
        }
    }
}

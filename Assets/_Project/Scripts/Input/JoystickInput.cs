using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class JoystickInput : MonoBehaviour
{
    private static readonly Dictionary<string, InputFeatureUsage<Vector2>> availableJoystickInput =
        new Dictionary<string, InputFeatureUsage<Vector2>>
        {
            {"primary2DAxis", CommonUsages.primary2DAxis}
        };
    
    public enum JoystickOption
    {
        primaryAxis
    };

    [SerializeField] private Vector2 axisPosition;
    
    [SerializeField] private float minValue;
    
    
    [Tooltip("Input device role (left or right controller)")]
    public InputDeviceRole deviceRole;
    
    // to obtain input devices
    List<InputDevice> inputDevices;
    Vector2 inputValue;
    
    // Start is called before the first frame update
    void Start()
    {
        // init list
        inputDevices = new List<InputDevice>();
    }

    // Update is called once per frame
    void Update()
    {
        InputDevices.GetDevicesWithRole(deviceRole, inputDevices);

        for (int i = 0; i < inputDevices.Count; i++)
        {
            if (inputDevices[i].TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 position))
            {
                if (Mathf.Abs(position.x) > minValue && Mathf.Abs(position.y) > minValue)
                {
                    axisPosition = position;
                }
                else
                {
                    axisPosition = Vector2.zero;
                }
            }
        }
    }

    public Vector2 GetStickPosition()
    {
        return axisPosition;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEnums : Singleton<CustomEnums>
{
    public enum Execution
    {
        Awake,
        OnEnable,
        Start
    }
    
    public enum SearchParameter
    {
        Tag,
        Name,
        Layer,
        Component
    }
    
    public enum Axis
    {
        Up,
        Down,
        Left,
        Right,
        Forward,
        Back
    }
    
    public enum Hand
    {
        Left,
        Right
    }
    
    private Dictionary<Axis, Vector3> _axis;

    private void Awake()
    {
        Initialize();
    }

    public Vector3 GetAxis(Axis axis)
    {
        if (_axis == null)
        {
            Initialize();
        }
        
        return _axis[axis];
    }

    private void Initialize()
    {
        _axis = new Dictionary<Axis, Vector3>();
            
        _axis.Add(Axis.Up, Vector3.up);
        _axis.Add(Axis.Down, Vector3.down);
        _axis.Add(Axis.Left, Vector3.left);
        _axis.Add(Axis.Right, Vector3.right);
        _axis.Add(Axis.Forward, Vector3.forward);
        _axis.Add(Axis.Back, Vector3.back);
    }
}

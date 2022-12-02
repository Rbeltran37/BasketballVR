using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnOffToggleSwitch : MonoBehaviour
{
    private bool _isOn;


    private void Awake()
    {
        Initialize();
    }

    protected abstract void Initialize();

    protected void SetIsOn(bool isOn)
    {
        _isOn = isOn;
    }

    public bool Toggle()
    {
        _isOn = !_isOn;
        
        return _isOn;
    }
}

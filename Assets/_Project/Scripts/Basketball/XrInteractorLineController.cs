using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XrInteractorLineController : MonoBehaviour
{
    public XRInteractorLineVisual lineVisual;
    public Transform uiPosition;
    public Transform playerHead;
    public float angleThreshold;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!uiPosition)
            lineVisual.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        IsLookingAtUi();
    }

    private void IsLookingAtUi()
    {
        if (!uiPosition) return;
        var angle = Vector3.Angle(uiPosition.forward, playerHead.forward);
        //Debug.Log(angle);
        if (angle > angleThreshold)
        {
            lineVisual.enabled = false;
        }
        else
        {
            lineVisual.enabled = true;
        }
    }
}

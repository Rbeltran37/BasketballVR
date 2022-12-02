using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;


public class HandAnimationController : MonoBehaviour
{
    [SerializeField] private PhotonView photonView;

    public InputDeviceCharacteristics controllerType;
    private InputDevice _thisController;

    public Animator _animationController;
    private bool _IsControllerFound;

    // Start is called before the first frame update
    void Start()
    {
        _animationController = GetComponent<Animator>();
        Initialise();
    }

    private void Initialise()
    {
        if (photonView && !photonView.IsMine) return;
        
        List<InputDevice> xrDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerType, xrDevices);

        if(xrDevices.Count == 0)
        {
            //Garbage in memory
            //Debug.Log("no XR devices");
        }
        else
        {
            _thisController = xrDevices[0];
            _IsControllerFound = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!_IsControllerFound)
        {
            Initialise();
        }
        else
        {
            if(_thisController.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
            {
                _animationController.SetFloat("Trigger", triggerValue);
            }
            if(_thisController.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
            {
                _animationController.SetFloat("Grip", gripValue);
            }
        }
    }
}

using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] public Transform AttachTransform;
    [SerializeField] public bool IsLeftHand;

    [SerializeField] private PhotonView handTargetPhotonView;
    [SerializeField] private ConfigurableJoint attachConfigurableJoint;
    [SerializeField] private AverageVelocityEstimator averageVelocityEstimator;
    [SerializeField] private PlayerActionHandler playerActionHandler;

    private IGrabbable _currentGrabbable;
    private IUsable _currentUsable;
    private bool _isGrabbing;

    public IGrabbable CurrentGrabbable => _currentGrabbable;
    public bool IsCurrentlyGrabbing => _isGrabbing;
    

    private void Awake()
    {
        if (IsLeftHand) AttachTransform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckForGrabbableObjects(other);
        CheckForUsableObjects(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CheckForGrabbableObjects(other);
        CheckForUsableObjects(other);
    }

    private void OnTriggerExit(Collider other)
    {
        GrabExit(other);
        UseExit(other);
    }

    private void CheckForGrabbableObjects(Collider other)
    {
        if (_currentGrabbable != null) return;
        if (_isGrabbing) return;

        var grabbable = other.GetComponent<IGrabbable>();
        if (grabbable == null) return;
        
        _currentGrabbable = grabbable;
        _currentGrabbable.ToggleHighlight(true);
    }
    
    private void CheckForUsableObjects(Collider other)
    {
        if (_currentUsable != null) return;

        var usable = other.GetComponent<IUsable>();
        if (usable == null) return;

        _currentUsable = usable;
    }
    
    private void GrabExit(Collider other)
    {
        if (_isGrabbing) return;

        var grabbable = other.GetComponent<IGrabbable>();
        if (grabbable == null) return;
        if (_currentGrabbable != grabbable) return;

        _currentGrabbable.ToggleHighlight(false);
        _currentGrabbable = null;
    }
    
    private void UseExit(Collider other)
    {
        var usable = other.GetComponent<IUsable>();
        if (usable == null) return;

        _currentUsable = null;
    }

    public void AttemptGrab()
    {
        _isGrabbing = true;
        
        if (_currentGrabbable == null) return;

        _currentGrabbable.AttemptGrab(this);
        if (IsGrabbing(_currentGrabbable))
        {
            playerActionHandler.AttemptSetBall(_currentGrabbable);
        }
    }

    public void AttemptUnGrab()
    {
        _isGrabbing = false;
        if (_currentGrabbable == null) return;
        
        if (_currentGrabbable.IsGrabbedBy(this))
        {
            _currentGrabbable.ToggleHighlight(false);
            _currentGrabbable.AttemptUnGrab();
        }

        _currentGrabbable = null;
    }

    public void AttemptUse()
    {
        _currentUsable?.AttemptUse(this);
    }

    public PhotonView GetHandTargetPhotonView()
    {
        return handTargetPhotonView;
    }

    public void Attach(Rigidbody rigidbodyToConnect)
    {
        attachConfigurableJoint.connectedBody = rigidbodyToConnect;
        attachConfigurableJoint.autoConfigureConnectedAnchor = true;
    }

    public void Detach()
    {
        attachConfigurableJoint.connectedBody = null;
        attachConfigurableJoint.autoConfigureConnectedAnchor = false;
    }

    public Vector3 GetAverageVelocity()
    {
        if (!averageVelocityEstimator)
            return Vector3.zero;
        
        return averageVelocityEstimator.GetVelocity();
    }
    
    public Vector3 GetAverageAngularVelocity()
    {
        if (!averageVelocityEstimator)
            return Vector3.zero;
        
        return averageVelocityEstimator.GetAngularVelocity();
    }

    public bool IsGrabbing(IGrabbable grabbable)
    {
        return _currentGrabbable == grabbable;
    }
}

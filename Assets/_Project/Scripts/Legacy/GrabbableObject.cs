using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GrabbableObject : MonoBehaviour, IGrabbable
{
    [SerializeField] protected PhotonView ThisPhotonView;
    [SerializeField] protected Transform ThisTransform;
    [SerializeField] protected Rigidbody ThisRigidbody;
    [SerializeField] protected float SmoothTime = 0.1F;
    [SerializeField] protected float SlerpRatio = .1f;
    
    [SerializeField] private Transform attachTransform;
    [SerializeField] private bool snapToPositionOnGrab = true;
    [SerializeField] private AudioSource thisAudioSource;
    [SerializeField] private SimpleAudioEvent grabAudioEvent;
    [SerializeField] private SimpleHapticEvent grabHapticEvent;
    [SerializeField] private Outline outline;

    protected Interactor CurrentInteractor;
    protected Vector3 Velocity = Vector3.zero;

    private bool _grabIsComplete;
    private Vector3 _interactorLocalPosition;
    private Quaternion _interactorLocalRotation;

    protected bool IsMine => ThisPhotonView && ThisPhotonView.IsMine;

    protected const float POSITION_TOLERANCE = .01f;


    protected virtual void Awake()
    {
        if (DebugLogger.IsNullWarning(ThisTransform, this, "Should be set in editor."))
        {
            ThisTransform = transform;
        }
        
        if (DebugLogger.IsNullWarning(ThisRigidbody, this, "Should be set in editor. Attempting to set."))
        {
            ThisRigidbody = ThisTransform.GetComponent<Rigidbody>();
            if (DebugLogger.IsNullError(ThisRigidbody, this, "Should be set in editor. Could not find.")) return;
        }
        
        ToggleHighlight(false);
    }
    
    protected virtual void Update()
    {
        if (!IsGrabbed()) return;
        if (snapToPositionOnGrab) return;
        if (_grabIsComplete) return;

        UpdateInteractorLocalPose();
        var targetPosition = GetWorldAttachPosition(CurrentInteractor);
        if (Vector3.Distance(ThisTransform.position, targetPosition) < POSITION_TOLERANCE)
        {
            CompleteGrab();
            return;
        }
        
        var targetRotation = GetWorldAttachRotation(CurrentInteractor);

        ThisTransform.position = Vector3.SmoothDamp(ThisTransform.position, targetPosition, ref Velocity, SmoothTime);
        ThisTransform.rotation = Quaternion.Slerp(ThisTransform.rotation, targetRotation, SlerpRatio);
    }

    //Only local player calls
    public void AttemptGrab(Interactor interactor)
    {
        if (CurrentInteractor) AttemptUnGrab();

        TransferOwnership();
        Grab(interactor);
        SendGrab(interactor);
    }

    public virtual void Grab(Interactor interactor)
    {
        CurrentInteractor = interactor;
        if (snapToPositionOnGrab)
        {
            CompleteGrab();
        }
    }
    
    private void CompleteGrab()
    {
        if (_grabIsComplete) return;
        
        _grabIsComplete = true;

        if (grabAudioEvent) grabAudioEvent.Play(thisAudioSource);
        if (grabHapticEvent) grabHapticEvent.Play(CurrentInteractor.IsLeftHand);
        
        SnapToPosition();
        AttachRigidbody();
        ToggleHighlight(false);
    }
    
    private void SnapToPosition()
    {
        UpdateInteractorLocalPose();
        SnapToTarget();
    }
    
    private void AttachRigidbody()
    {
        CurrentInteractor.Attach(ThisRigidbody);
    }
    
    private void UpdateInteractorLocalPose()
    {
        // In order to move the Interactable to the Interactor we need to
        // calculate the Interactable attach point in the coordinate system of the
        // Interactor's attach point.
        var attachPosition = attachTransform.position;
        var attachOffset = ThisRigidbody.worldCenterOfMass - attachPosition;
        var localAttachOffset = attachTransform.InverseTransformDirection(attachOffset);

        var inverseLocalScale = CurrentInteractor.AttachTransform.lossyScale;
        inverseLocalScale = new Vector3(1f / inverseLocalScale.x, 1f / inverseLocalScale.y, 1f / inverseLocalScale.z);
        localAttachOffset.Scale(inverseLocalScale);

        _interactorLocalPosition = localAttachOffset;
        _interactorLocalRotation = Quaternion.Inverse(Quaternion.Inverse(ThisRigidbody.rotation) * attachTransform.rotation);
        if (CurrentInteractor.IsLeftHand) _interactorLocalRotation *= Quaternion.Euler(0, 0, 180f);
    }
    
    private void SnapToTarget()
    {
        // Compute the unsmoothed target world position and rotation
        var rawTargetWorldPosition = GetWorldAttachPosition(CurrentInteractor);
        var rawTargetWorldRotation = GetWorldAttachRotation(CurrentInteractor);

        ThisTransform.position = rawTargetWorldPosition;
        ThisTransform.rotation = rawTargetWorldRotation;
    }

    private Vector3 GetWorldAttachPosition(Interactor interactor)
    {
        return interactor.AttachTransform.position + interactor.AttachTransform.rotation * _interactorLocalPosition;
    }

    private Quaternion GetWorldAttachRotation(Interactor interactor)
    {
        return interactor.AttachTransform.rotation * _interactorLocalRotation;
    }

    protected void SendGrab(Interactor interactor)
    {
        if (DebugLogger.IsNullError(interactor, this)) return;
        
        if (PhotonNetwork.OfflineMode) return;
        
        if (DebugLogger.IsNullError(ThisPhotonView, this, "Must be set in editor.")) return;

        var photonView = interactor.GetHandTargetPhotonView();
        if (DebugLogger.IsNullError(photonView, this)) return;

        var photonViewId = photonView.ViewID;
        ThisPhotonView.RPC(nameof(RPCGrab), RpcTarget.OthersBuffered, photonViewId);
    }

    [PunRPC]
    protected void RPCGrab(int photonViewId)
    {
        var interactorPhotonView = PhotonNetwork.GetPhotonView(photonViewId);
        if (DebugLogger.IsNullError(interactorPhotonView, this)) return;

        var interactorReference = interactorPhotonView.GetComponent<InteractorReference>();
        if (DebugLogger.IsNullError(interactorReference, this)) return;

        var interactor = interactorReference.Interactor;
        if (DebugLogger.IsNullError(interactor, this)) return;
        
        Grab(interactor);
    }

    public void AttemptUnGrab()
    {
        if (!CurrentInteractor) return;
        
        UnGrab();
        SendUnGrab();
    }

    public virtual void UnGrab()
    {
        if (!CurrentInteractor) return;
        
        DetachRigidbody();
        
        CurrentInteractor = null;
        _grabIsComplete = false;
    }

    private void DetachRigidbody()
    {
        ThisRigidbody.velocity = GetAverageVelocity();
        ThisRigidbody.angularVelocity = GetAverageAngularVelocity();
        CurrentInteractor.Detach();
    }

    protected void SendUnGrab()
    {
        if (PhotonNetwork.OfflineMode) return;

        ThisPhotonView.RPC(nameof(RPCUnGrab), RpcTarget.OthersBuffered);
    }

    [PunRPC]
    protected void RPCUnGrab()
    {
        UnGrab();
    }

    public bool IsGrabbed()
    {
        return CurrentInteractor != null;
    }

    public bool IsGrabbedBy(Interactor interactor)
    {
        return CurrentInteractor == interactor;
    }

    private void TransferOwnership()
    {
        if (PhotonNetwork.OfflineMode) return;
        if (IsMine) return;

        if (DebugLogger.IsNullError(ThisPhotonView, this, "Must be set in editor.")) return;

        ThisPhotonView.TransferOwnership(PhotonNetwork.LocalPlayer);
    }

    protected Vector3 GetAverageVelocity()
    {
        if(!CurrentInteractor)
            return Vector3.zero;

        return CurrentInteractor.GetAverageVelocity();
    }
    
    protected Vector3 GetAverageAngularVelocity()
    {
        if(!CurrentInteractor)
            return Vector3.zero;

        return CurrentInteractor.GetAverageAngularVelocity();
    }

    public void ToggleHighlight(bool state)
    {
        if (!outline) return;
        
        outline.enabled = state;
    }
}

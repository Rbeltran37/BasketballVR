using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basketball : GrabbableObject
{
    [SerializeField] private TwoHandDetection twoHandDetection;

    private BallState _currentBallState = BallState.Held;

    public Transform BallTransform => ThisTransform;
    public Rigidbody BallRigidbody => ThisRigidbody;
    public BallState CurrentBallState => _currentBallState;
    public bool IsGrabbedByTwoHands => twoHandDetection.IsGrabbedByTwoHands();

    
    public enum BallState
    {
        Held,
        Dribbled,
        Live
    }
    
    public void Hold(Interactor interactor)
    {
        ThisRigidbody.isKinematic = false;
        _currentBallState = BallState.Held;
        Grab(interactor);
    }

    public void Dribble()
    {
        ThisRigidbody.isKinematic = true;
        _currentBallState = BallState.Dribbled;
    }

    public void Live()
    {
        ThisRigidbody.isKinematic = false;
        _currentBallState = BallState.Live;
    }

    public void Shoot()
    {
        Live();
    }
}

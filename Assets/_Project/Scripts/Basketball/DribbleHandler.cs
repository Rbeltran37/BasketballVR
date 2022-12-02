using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DribbleHandler : MonoBehaviour
{
    [SerializeField] private Transform playAreaTransform;
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform rightHandTransform;
    [SerializeField] private float speed = .2f;

    private bool _hasDribble;
    private bool _isBouncingTowardsFloor;
    private float _distanceFromTarget;
    private Transform _currentTarget;
    private Transform _nextTarget;
    private Transform _ballTransform;
    private Vector3 _bouncePosition;
    private Vector3 _currentTargetPosition;
    private Vector3 _ballPosition;
    private Vector3 _nextTargetMidpoint;
    private Vector3 _previousPlayAreaPosition;
    private Vector3 _currentPlayAreaPosition;
    private Vector3 _travelVector;
    private Basketball _basketball;
    
    private const float BALL_RADIUS = .1511944f;
    private const float FLOOR_HEIGHT = 0;

    private void FixedUpdate()
    {
        _previousPlayAreaPosition = _currentPlayAreaPosition;
        _currentPlayAreaPosition = playAreaTransform.position;
        if (!_hasDribble) return;

        _ballPosition = _ballTransform.position;
        if (_isBouncingTowardsFloor)
        {
            if (_ballPosition.y < BALL_RADIUS)
            {
                BounceFromFloor();
                return;
            }
            
            _ballPosition += (_bouncePosition - _ballPosition) * speed;
        }
        else
        {
            _currentTargetPosition = _currentTarget.position;
            _distanceFromTarget = Vector3.Distance(_ballPosition, _currentTargetPosition);
            if (_distanceFromTarget < BALL_RADIUS)
            {
                BounceToFloor();
                return;
            }

            _travelVector = _currentPlayAreaPosition - _previousPlayAreaPosition;
            _ballPosition += _travelVector + (_currentTarget.position - _ballTransform.position) * speed;
        }

        _ballTransform.position = _ballPosition;
    }

    private void BounceToFloor()
    {
        _isBouncingTowardsFloor = true;
        
        if (_currentTarget == _nextTarget)
        {
            _bouncePosition = new Vector3(_ballPosition.x, FLOOR_HEIGHT, _ballPosition.z);
            return;
        }

        _nextTargetMidpoint = _currentTargetPosition + (_nextTarget.position - _currentTargetPosition) / 2;
        _bouncePosition = new Vector3(_nextTargetMidpoint.x, FLOOR_HEIGHT, _nextTargetMidpoint.z);
    }
    
    private void BounceFromFloor()
    {
        _currentTarget = _nextTarget;
        _isBouncingTowardsFloor = false;
    }

    public void SetPossession(Basketball basketball)
    {
        _basketball = basketball;
        _ballTransform = _basketball.BallTransform;
    }
    
    public void AttemptDribble(Interactor interactor)
    {
        if (!_basketball) return;

        if (interactor.IsLeftHand)
        {
            _currentTarget = leftHandTransform;
        }
        else
        {
            _currentTarget = rightHandTransform;
        }
        
        _nextTarget = _currentTarget;
        
        _ballPosition = _ballTransform.position;
        BounceToFloor();
        _basketball.Dribble();
        _hasDribble = true;
    }

    public void EndDribble()
    {
        _hasDribble = false;
    }

    public void LosePossession()
    {
        EndDribble();
        _basketball = null;
    }

    public bool IsBeingDribbledBy(Interactor interactor)
    {
        var isLeftHand = interactor.IsLeftHand;
        var handTransformToCheck = isLeftHand ? leftHandTransform : rightHandTransform;
        return _currentTarget == handTransformToCheck;
    }

    public void Crossover(Interactor interactor)
    {
        if (interactor.IsLeftHand) TargetLeftHand();
        else TargetRightHand();
    }
    
    private void TargetLeftHand()
    {
        _nextTarget = leftHandTransform;
    }

    private void TargetRightHand()
    {
        _nextTarget = rightHandTransform;
    }
}

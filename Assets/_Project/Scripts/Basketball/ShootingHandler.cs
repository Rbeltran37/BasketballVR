using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SimpleBallistic;
using UnityEngine;

public class ShootingHandler : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform headTarget;
    [SerializeField] private JumpHandler jumpHandler;
    [SerializeField] private float maxTargetError = .5f;
    [SerializeField] private float shotReleasePenalty = .75f;
    [SerializeField] private float shotGrabPenalty = .75f;
    
    private float _shotDistanceErrorPercentage;
    private float _shotGrabErrorPercentage;
    private float _shotReleaseErrorPercentage;
    private Transform _basketballTransform;
    private Transform _basketOne;
    private Transform _basketTwo;
    private Transform _currentBasket;
    private Transform _shotTarget;
    private Rigidbody _basketballRigidbody;
    private Basketball _basketball;

    private const float SPIN_FORCE = 10f;
    private const float THREE_POINT_DISTANCE = 8.25f;
    private const float COURT_LENGTH = 30;
    private const float MIN_SHOT_ANGLE = 25f;
    private const float MAX_SHOT_ANGLE = 45f;
    private const float ADDED_SHOT_ANGLE_MIN = 0;
    private const float ADDED_SHOT_ANGLE_MAX = 15F;
    private const float MIN_DISTANCE = 3f;
    private const float MAX_DISTANCE = 15f;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitToGetSceneItems());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_basketball) return;
        
        CalculateShotGrabError();
        CalculateShotReleaseError();
    }

    private IEnumerator WaitToGetSceneItems()
    {
        //Needs to be fixed. Just getting baskets at start of game for now.
        //This will need to be changed when teams are implemented.
        yield return new WaitForSeconds(3f);
        _basketOne = GameObject.Find("Basket Target (Right)").transform;
        _basketTwo = GameObject.Find("Basket Target (Left)").transform;
    }

    public void Shoot()
    {
        if (!InShootingPosition())
            return;

        _currentBasket = FacingBasket();

        _shotTarget = _currentBasket;
        
        CalculateShotDistanceError();
        
        //Gets the angle of the shot based on distance and then adds more angle based on release point. 
        var shotAngle = CalculateAngleOfShotBasedOnDistance();
        shotAngle += CalculateAdditionalShotAngleTilt();

        //add error together
        var odds = (_shotReleaseErrorPercentage + _shotGrabErrorPercentage + _shotDistanceErrorPercentage) / 100;
        var randomValue = Random.value;

        if (randomValue >= odds)
        {
            var calculatedForce = Ballistics.GetForce(_basketballTransform.position, _currentBasket.position, shotAngle);
            LaunchBall(shotAngle, calculatedForce);
        } 
        else {
            var randomXAxisPosition = Random.Range(-maxTargetError, maxTargetError);
            var randomZAxisPosition = Random.Range(-maxTargetError, maxTargetError);
            var remappedErrorModifierToDistance = Remap(DistanceToBasket(_currentBasket), 0, COURT_LENGTH, 0, 1);
            var direction = headTarget.position - _currentBasket.position;
            direction.y = 0;
            var rotation = Quaternion.LookRotation(direction);
            _shotTarget.rotation = rotation;
            var updatedShotWithMovementError = _shotTarget.position + _shotTarget.forward * (-randomZAxisPosition*remappedErrorModifierToDistance) + _shotTarget.right * (-randomXAxisPosition*remappedErrorModifierToDistance);
            var calculatedForce = Ballistics.GetForce(_basketballTransform.position, updatedShotWithMovementError, shotAngle);
            LaunchBall(shotAngle, calculatedForce);
        }

        _basketballRigidbody.AddTorque(Vector3.forward * SPIN_FORCE, ForceMode.Impulse);
        _basketballTransform = null;
    }

    private Transform FacingBasket()
    {
        if (!_basketOne || !_basketTwo) return null;
        
        var headPosition = headTarget.position;
        var headForward = headTarget.forward;
        var basketOneAngle = Vector3.Angle(headForward, _basketOne.transform.position - headPosition);
        var basketTwoAngle = Vector3.Angle(headForward, _basketTwo.transform.position - headPosition);
        _currentBasket = basketOneAngle < basketTwoAngle ? _basketOne : _basketTwo;
        return _currentBasket;
    }

    private void LaunchBall(float angle, float force)
    {
        if (float.IsNaN(force))
            return;

        LookAtTarget(angle);
        _basketballRigidbody.velocity = _basketballTransform.forward * force;
    }

    private void LookAtTarget(float angle)
    {
        Vector3 axis = _currentBasket.position - _basketballTransform.position;
        axis.y = 0;
        axis.Normalize();
        axis = Quaternion.AngleAxis(angle, Vector3.Cross(axis, Vector3.up)) * axis;
        _basketballTransform.forward = axis;
    }

    private float CalculateAdditionalShotAngleTilt()
    {
        if (!_basketballTransform || !_currentBasket)
            return 0;
        
        var angleToBasket = Vector3.Angle(headTarget.transform.up, _basketballTransform.transform.position - headTarget.position);
        var remappedAngleToBasket = Remap(angleToBasket, 90, 0, ADDED_SHOT_ANGLE_MIN, ADDED_SHOT_ANGLE_MAX);

        if (remappedAngleToBasket > 0)
        {
            return remappedAngleToBasket;
        }
        return 0;
    }
    
    private void CalculateShotReleaseError()
    {
        if (!InShootingPosition() || !jumpHandler.IsJumping()) return;

        var remappedShotReleaseError = Remap(jumpHandler.GetCurrentHeight(), jumpHandler.GetGroundHeight(), jumpHandler.GetMaxJumpHeight(), 1, 0);

        _shotReleaseErrorPercentage = shotReleasePenalty;
        _shotReleaseErrorPercentage *= remappedShotReleaseError;
    }

    private void CalculateShotGrabError()
    {
        if (IsGrabbedByTwoHands())
        {
            _shotGrabErrorPercentage = 0;
        }
        else
        {
            _shotGrabErrorPercentage = shotGrabPenalty;
            _shotGrabErrorPercentage *= 1;
        }
    }
    
    private void CalculateShotDistanceError()
    {
        _shotDistanceErrorPercentage = 0;
        var dist = DistanceToBasket(_currentBasket);
        
        if (dist >= THREE_POINT_DISTANCE)
        {
            var remappedDistanceToShotError = Remap(dist, THREE_POINT_DISTANCE, COURT_LENGTH, 25, 75);
            _shotDistanceErrorPercentage = remappedDistanceToShotError;
        }
    }

    private float CalculateAngleOfShotBasedOnDistance()
    {
        var dist = DistanceToBasket(_currentBasket);
        var remappedAngleFromDistance = Remap(dist, MIN_DISTANCE, MAX_DISTANCE, MAX_SHOT_ANGLE, MIN_SHOT_ANGLE);

        if (remappedAngleFromDistance < MIN_SHOT_ANGLE)
            return MIN_SHOT_ANGLE;
        
        return remappedAngleFromDistance;
    }

    private bool InShootingPosition()
    {
        return _basketballTransform && (headTarget.position.y < _basketballTransform.position.y);
    }

    private float DistanceToBasket(Transform basket)
    {
        return Vector3.Distance(basket.position, player.transform.position);
    }

    public void SetPossession(Basketball basketball)
    {
        _basketball = basketball;
        _basketballTransform = _basketball.BallTransform;
        _basketballRigidbody = _basketball.BallRigidbody;
    }

    public void LosePossession()
    {
        _basketball = null;
        _basketballTransform = null;
        _basketballRigidbody = null;
    }
    
    private bool IsGrabbedByTwoHands()
    {
        return _basketball.IsGrabbedByTwoHands;
    }
    
    private static float Remap (float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}

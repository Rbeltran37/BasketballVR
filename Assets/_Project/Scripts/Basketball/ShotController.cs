using System.Collections;
using System.Security.Cryptography;
using Sirenix.OdinInspector;
using UnityEngine;
using SimpleBallistic;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class ShotController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform headTarget;
    [SerializeField] private GameObject lineRendererPrefab;
    [SerializeField] private JumpHandler jumpHandler;
    [SerializeField] private bool isShowingTrajectory = false;
    [SerializeField] private float maxTargetError;
    [SerializeField] private float shotReleasePenalty;
    [SerializeField] private float shotGrabPenalty;
    private float _shotDistanceErrorPercentage;
    private float _shotGrabErrorPercentage;
    private float _shotReleaseErrorPercentage;
    private Transform _currentBall;
    private Rigidbody _currentBallRigidbody;
    private Transform _basketOne;
    private Transform _basketTwo;
    private Transform _shotTarget;
    private CodyBasketball _basketballController;
    private LineRenderer _debugLineRenderer;
    private bool _isBallGrabbed = false;
    private Transform _currentBasket;
    private const float START_WIDTH = .1f;
    private const float END_WIDTH = 1f;
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

    [Button]
    public void Shoot()
    {
        if (!InShootingPosition() || !jumpHandler.IsJumping())
            return;
        
        _currentBasket = FacingBasket();

        /////////////// Line Renderer for debugging. remove in production
        if (isShowingTrajectory)
        {
            var lineRendererGo = Instantiate(lineRendererPrefab, Vector3.zero, Quaternion.identity);
            _debugLineRenderer = lineRendererGo.GetComponent<LineRenderer>();
            _debugLineRenderer.startWidth = START_WIDTH;
            _debugLineRenderer.endWidth = END_WIDTH;
        }

        _shotTarget = _currentBasket;
        //_shotTarget.position = _currentBasket.position;
        
        CalculateShotDistanceError();
        SetPointsBasedOnDistance();
        
        //Gets the angle of the shot based on distance and then adds more angle based on release point. 
        var shotAngle = CalculateAngleOfShotBasedOnDistance();
        shotAngle += CalculateAdditionalShotAngleTilt();

        //add error together
        var odds = (_shotReleaseErrorPercentage + _shotGrabErrorPercentage + _shotDistanceErrorPercentage) / 100;
        var randomValue = Random.value;

        if (randomValue >= odds)
        {
            var calculatedForce = Ballistics.GetForce(_currentBall.position, _currentBasket.position, shotAngle);
            LaunchBall(shotAngle, calculatedForce);
            
            if (isShowingTrajectory) Ballistics.TrajectoryProjection(_currentBall.position, _currentBasket.position-_currentBall.position, calculatedForce, shotAngle, 25, 0.25f, _debugLineRenderer);
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
            var calculatedForce = Ballistics.GetForce(_currentBall.position, updatedShotWithMovementError, shotAngle);
            LaunchBall(shotAngle, calculatedForce);

            if (isShowingTrajectory) Ballistics.TrajectoryProjection(_currentBall.position, updatedShotWithMovementError - _currentBall.position, calculatedForce, shotAngle, 25, 0.25f, _debugLineRenderer);
        }

        _currentBallRigidbody.AddTorque(Vector3.forward * SPIN_FORCE, ForceMode.Impulse);
        _currentBall = null;
    }

    private Transform FacingBasket()
    {
        if (!_basketOne || !_basketTwo) return null;
        
        var playerTransform = player.transform;
        var playerPosition = playerTransform.position;
        var playerForward = playerTransform.forward;
        var basketOneAngle = Vector3.Angle(playerForward, _basketOne.transform.position - playerPosition);
        var basketTwoAngle = Vector3.Angle(playerForward, _basketTwo.transform.position - playerPosition);
        _currentBasket = basketOneAngle < basketTwoAngle ? _basketOne : _basketTwo;
        return _currentBasket;
    }

    private void LaunchBall(float angle, float force)
    {
        if (float.IsNaN(force))
            return;

        _currentBallRigidbody.isKinematic = false;
        LookAtTarget(angle);
        _currentBallRigidbody.velocity = _currentBall.forward * force;
    }

    private void LookAtTarget(float _angle)
    {
        Vector3 axis = _currentBasket.position - _currentBall.position;
        axis.y = 0;
        axis.Normalize();
        axis = Quaternion.AngleAxis(_angle, Vector3.Cross(axis, Vector3.up)) * axis;
        _currentBall.forward = axis;
    }

    private float CalculateAdditionalShotAngleTilt()
    {
        if (!_currentBall || !_currentBasket)
            return 0;
        
        var angleToBasket = Vector3.Angle(headTarget.transform.up, _currentBall.transform.position - headTarget.position);
        var remappedAngleToBasket = Remap(angleToBasket, 90, 0, ADDED_SHOT_ANGLE_MIN, ADDED_SHOT_ANGLE_MAX);

        if (remappedAngleToBasket > 0)
        {
            return remappedAngleToBasket;
        }
        return 0;
    }

    #region Inaccuracy Calculations

    private void CalculateShotReleaseError()
    {
        if (!_isBallGrabbed || !InShootingPosition() || !jumpHandler.IsJumping()) return;
        
        var remappedIndicatorValueToHeight = Remap(jumpHandler.GetCurrentHeight(), jumpHandler.GetGroundHeight(), jumpHandler.GetMaxJumpHeight(), 0, _basketballController.GetMaxIndicatorSize());
        //divided by two so you can see the color change more. 
        var clampedForColorOfIndicator = Mathf.Clamp(jumpHandler.GetCurrentHeight()/2, jumpHandler.GetGroundHeight(), jumpHandler.GetMaxJumpHeight());
            
        if(_basketballController) _basketballController.SetIndicatorScale(remappedIndicatorValueToHeight);
        if(_basketballController) _basketballController.LerpColorOfIndicator(clampedForColorOfIndicator);

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
    
    #endregion

    private void SetPointsBasedOnDistance()
    {
        var dist = DistanceToBasket(_currentBasket);
        if (!_basketballController) _basketballController = _currentBall.GetComponent<CodyBasketball>();

        if (dist < THREE_POINT_DISTANCE)
        {
            _basketballController.points = CodyBasketball.ShotPoints.twoPoints;
        }

        if (dist >= THREE_POINT_DISTANCE)
        {
            _basketballController.points = CodyBasketball.ShotPoints.threePoints;
        }
    }

    #region Helper Functions

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
        return _currentBall && (headTarget.position.y < _currentBall.position.y);
    }

    private float DistanceToBasket(Transform basket)
    {
        return Vector3.Distance(basket.position, player.transform.position);
    }

    public void SetBasketballController(CodyBasketball basketballController)
    {
        this._basketballController = basketballController;
    }

    private bool IsGrabbedByTwoHands()
    {
        return _basketballController && _basketballController.IsGrabbedByTwoHands();
    }
    
    private static float Remap (float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public void SetCurrentBall(Transform ball)
    {
        _currentBall = ball;
    }
    
    public Transform GetCurrentBall()
    {
        return _currentBall;
    }

    public void SetCurrentBallRigidbody(Rigidbody rigidbody)
    {
        _currentBallRigidbody = rigidbody;
    }

    public Rigidbody GetCurrentBallRigidbody()
    {
        return _currentBallRigidbody;
    }

    public void IsBallGrabbed(bool isGrabbed)
    {
        _isBallGrabbed = isGrabbed;
    }

    #endregion Helper Functions
}

using SimpleBallistic;
using UnityEngine;


public class PassHandler : MonoBehaviour
{
    [SerializeField] private Transform headTarget;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Interactor interactorLeft;
    [SerializeField] private Interactor interactorRight;
    [SerializeField] private float passForce;
    private Transform _currentPassDestination;
    private Basketball _basketball;
    private Transform _ballTransform;
    

    // Update is called once per frame
    void Update()
    {
        CastForPlayers();
    }

    //This 100% needs to be redone. Not efficient to be checking for interactors. 
    private void CastForPlayers()
    {
        if (!_basketball) return;
        
        RaycastHit[] hits;
        Vector3 ballPosition = _ballTransform.position;

        hits = Physics.SphereCastAll(ballPosition, 2, headTarget.forward, 70f, layerMask, QueryTriggerInteraction.UseGlobal);
        
        for (int i = 0; i < hits.Length; i++)
        {
            var interactor = hits[i].transform.GetComponent<Interactor>();
            if (interactor && interactor!= interactorLeft && interactor != interactorRight)
            {
                _currentPassDestination = hits[i].transform;
                return;
            }
        }

        _currentPassDestination = null;
    }

    public void Pass()
    {
        if (!_currentPassDestination)
            return;
        
        var direction = _currentPassDestination.position;
        var playerVelocityEstimator = _currentPassDestination.GetComponent<AverageVelocityEstimator>();

        if (!playerVelocityEstimator)
            return;
        
        var playerVelocity = playerVelocityEstimator.GetVelocity();
        direction += playerVelocity;
        if(_basketball) _basketball.UnGrab();
        var basketballPosition = _ballTransform.position;
        passForce = Ballistics.GetForce(basketballPosition, direction, 25f);
        _basketball.BallRigidbody.isKinematic = false;
        LookAtAngle(25f, _ballTransform, direction, basketballPosition);
        _basketball.BallRigidbody.velocity = _ballTransform.forward * passForce;
    }

    public void PhysicsPass()
    {
        var basketballRigidbody = _basketball.BallRigidbody;
        if(_basketball) _basketball.UnGrab();
        basketballRigidbody.isKinematic = false;
        basketballRigidbody.AddForce(_ballTransform.forward * passForce, ForceMode.Impulse);
    }

    private void LookAtAngle(float _angle, Transform currentBall ,Vector3 destination, Vector3 startPos)
    {
        Vector3 axis = destination - startPos;
        axis.y = 0;
        axis.Normalize();
        axis = Quaternion.AngleAxis(_angle, Vector3.Cross(axis, Vector3.up)) * axis;
        currentBall.forward = axis;
    }
    
    public void SetPossession(Basketball basketball)
    {
        _basketball = basketball;
        _ballTransform = _basketball.BallTransform;
    }
    
    public void LosePossession()
    {
        _basketball = null;
        _ballTransform = null;
    }
}

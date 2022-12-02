using System.Collections;
using System.Collections.Generic;
using SimpleBallistic;
using UnityEngine;


public class BrianPassController : MonoBehaviour
{
    [SerializeField] private ShotController shotController;
    [SerializeField] private Transform headTarget;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Interactor interactorLeft;
    [SerializeField] private Interactor interactorRight;
    private Transform _currentPassDestination;
    private float _passForce;
    private Transform _lastPassDestination;
    private Transform _landingMarker;
    private MeshRenderer _landingMarkerMesh;
    

    // Update is called once per frame
    void Update()
    {
        CastForPlayers();
    }

    //This 100% needs to be redone. Not efficient to be checking for interactors. 
    private void CastForPlayers()
    {
        if (!shotController.GetCurrentBall())
        {
            return;
        }

        //Temp fix to stop ability to pass ball while not grabbed - Brian
        if (!shotController.GetCurrentBall().GetComponent<CodyBasketball>().IsGrabbed())
        {
            if (_currentPassDestination != null)
            {
                _currentPassDestination.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                _currentPassDestination = null;
                _lastPassDestination = null;
            }
            
            return;
        }

        RaycastHit[] hits;
        Vector3 currentBallPosition = shotController.GetCurrentBall().position;
        
        hits = Physics.SphereCastAll(currentBallPosition, 2, headTarget.forward, 70f, layerMask, QueryTriggerInteraction.UseGlobal);
        
        for (int i = 0; i < hits.Length; i++)
        {
            //DebugLogger.Info("CastForPlayers", hits[i].transform.name, this);
            var interactor = hits[i].transform.GetComponent<Interactor>();
            if (interactor && interactor!= interactorLeft && interactor != interactorRight)
            {
                _currentPassDestination = hits[i].transform;
                
                //enables marker above CurrentPassDestination and disables marker above the previous one if not the same - Brian
                if (_currentPassDestination != _lastPassDestination)
                {
                    if (_lastPassDestination != null)
                    {
                        _lastPassDestination.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                    }
                    
                    _currentPassDestination.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                    _lastPassDestination = _currentPassDestination;
                }
                
                return;
            }
        }
        
        //Disables marker above currentPassDestination - Brian
        if (_currentPassDestination != null)
        {
            _currentPassDestination.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            _lastPassDestination = null;
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
        //pass in front of player
        direction += playerVelocity;
        
        //Displays a transparent sphere using direction - Brian
        DisplayLandZone(direction);

        var basketballController = shotController.GetCurrentBall().GetComponent<CodyBasketball>();
        if(basketballController)
            basketballController.UnGrab();

        _passForce = Ballistics.GetForce(shotController.GetCurrentBall().position, direction, 25f);
        shotController.GetCurrentBallRigidbody().isKinematic = false;

        LookAtAngle(25f, shotController.GetCurrentBall(), direction, shotController.GetCurrentBall().position);
        shotController.GetCurrentBallRigidbody().velocity = shotController.GetCurrentBall().forward * _passForce;
    }

    private void LookAtAngle(float _angle, Transform currentBall ,Vector3 destination, Vector3 startPos)
    {
        Vector3 axis = destination - startPos;
        axis.y = 0;
        axis.Normalize();
        axis = Quaternion.AngleAxis(_angle, Vector3.Cross(axis, Vector3.up)) * axis;
        currentBall.forward = axis;
    }

    private void DisplayLandZone(Vector3 direction)
    {
        if (!_landingMarker)
        {
            _landingMarker = GameObject.Find("PassLandMarker").GetComponent<Transform>();
            _landingMarkerMesh = _landingMarker.GetChild(0).GetComponent<MeshRenderer>();
        }

        _landingMarker.position = direction;

        if (_landingMarkerMesh.enabled == true)
        {
            _landingMarkerMesh.enabled = false;
            StopCoroutine(nameof(DisplayTime));
        }
        StartCoroutine(nameof(DisplayTime));
    }

    private IEnumerator DisplayTime()
    {
        _landingMarkerMesh.enabled = true;

        yield return new WaitForSeconds(2f);

        _landingMarkerMesh.enabled = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodyDribble : MonoBehaviour
{
    private Transform _currBall;
    private CodyBasketball _basketballController;
    public AverageVelocityEstimator velocityEstimator;
    
    //Bounce speed needs to change the closer the ball is to the floor.
    public float bounceSpeed = 2.0f;
    private Interactor _interactor;
    public JumpHandler jumpHandlerController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!_currBall)
            return;

        var interactorPosition = _interactor.transform.position;
        var yPos = Bounce((Time.time * bounceSpeed)%1) * interactorPosition.y;
        _currBall.position = new Vector3(interactorPosition.x, yPos, interactorPosition.z);
        
        //yucky. I know. Will clean it up when official implementation is chosen.
        //If the player jumps or stops moving then ball is grabbed and placed back in players hands. Dribbling while standing, not in my house.
        if ((jumpHandlerController.IsJumping() && _currBall && _basketballController) || (!IsMoving() && _currBall && _basketballController))
        {
            if (!_basketballController.IsGrabbed())
                _basketballController.Grab(_interactor);
            
            //most likely needs to be slerped to ball.
            _currBall.position = _interactor.transform.position;
        }
    }

    private bool IsMoving()
    {
        return velocityEstimator.GetVelocity().magnitude > .1f;
    }

    private float Bounce(float t){
        return Mathf.Sin(Mathf.Clamp01(t) * Mathf.PI);
    }

    public void SetCurrentBall(Transform currentBall)
    {
        _currBall = currentBall;
    }
    
    public void SetBasketballController(CodyBasketball basketballController)
    {
        _basketballController = basketballController;
    }

    public void SetInteractor(Interactor interactor)
    {
        _interactor = interactor;
    }
}

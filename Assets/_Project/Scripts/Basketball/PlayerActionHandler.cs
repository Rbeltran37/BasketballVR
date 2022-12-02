using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PlayerActionHandler : MonoBehaviour
{
    [SerializeField] private DribbleHandler dribbleHandler;
    [SerializeField] private ShootingHandler shootingHandler;
    [SerializeField] private PassHandler passHandler;
    [SerializeField] private JumpHandler jumpHandler;

    private Basketball _basketball;

    private bool IsDribbling => _basketball.CurrentBallState == Basketball.BallState.Dribbled;
    private bool IsBallHeld => _basketball.CurrentBallState == Basketball.BallState.Held;


    public void OnPress(Interactor interactor)
    {
        if (!_basketball) interactor.AttemptGrab();

        if (!_basketball) return;
        
        if (IsDribbling)
        {
            if (IsOffHand(interactor))
            {
                Crossover(interactor);
                return;
            }
                
            dribbleHandler.EndDribble();
        }
            
        _basketball.Hold(interactor);
    }

    public void OnRelease(Interactor interactor)
    {
        interactor.AttemptUnGrab();

        if (!_basketball) return;
        if (!IsBallHeld) return;
            
        if (jumpHandler.IsJumping()) Shoot(interactor);
        else AttemptDribble(interactor);
    }
    
    public void AttemptSetBall(IGrabbable grabbable)
    {
        _basketball = (Basketball) grabbable;
        if (_basketball == null) return;

        passHandler.SetPossession(_basketball);
        dribbleHandler.SetPossession(_basketball);
        shootingHandler.SetPossession(_basketball);
    }

    private void LosePossession()
    {
        _basketball.Live();
        _basketball = null;
        
        passHandler.LosePossession();
        dribbleHandler.LosePossession();
        shootingHandler.LosePossession();
    }

    private void Crossover(Interactor interactor)
    {
        dribbleHandler.Crossover(interactor);
    }

    private void AttemptDribble(Interactor interactor)
    {
        if (jumpHandler.IsJumping()) return;

        dribbleHandler.AttemptDribble(interactor);
    }

    private void Shoot(Interactor interactor)
    {
        if (!jumpHandler.IsJumping()) return;

        _basketball.UnGrab();
        _basketball.Shoot();
        shootingHandler.Shoot();
        
        LosePossession();
    }

    public void Pass(Interactor interactor)
    {
        if (!_basketball) return;
        
        Hold(interactor);
        
        passHandler.PhysicsPass();
        
        LosePossession();
    }
    
    
    private void Hold(Interactor interactor)
    {
        if (_basketball.CurrentBallState == Basketball.BallState.Dribbled)
        {
            if (!dribbleHandler.IsBeingDribbledBy(interactor))
            {
                dribbleHandler.Crossover(interactor);
                return;
            }
            
            dribbleHandler.EndDribble();
        }
        
        _basketball.Hold(interactor);
    }

    public void Jump()
    {
        jumpHandler.Jump();
    }
    
    private bool IsOffHand(Interactor interactor)
    {
        return !dribbleHandler.IsBeingDribbledBy(interactor);
    }
}

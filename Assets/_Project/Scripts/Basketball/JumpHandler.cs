using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class JumpHandler : MonoBehaviour
{
    [SerializeField] private Vector3 gravity;
    [SerializeField] private Transform xrRig;
    [SerializeField] private float maxJumpShotHeight;
    [SerializeField] private float maxDunkHeight;
    [SerializeField] private SmoothLocomotion locomotion;
    [SerializeField] private AverageVelocityEstimator avgVelocityEstimator;
    private Vector3 _currVelocity;
    private bool _isJumping;
    private Vector3 _velocity;
    private const float GROUND_HEIGHT = 0f;
    
    void FixedUpdate()
    {
        if (_isJumping)
        {
            locomotion.enabled = false;
            _velocity += gravity * Time.deltaTime;
            Vector3 position = xrRig.position + _velocity * Time.deltaTime;

            if (position.y <= GROUND_HEIGHT) {
                position.y = GROUND_HEIGHT;
                locomotion.enabled = true;
                _isJumping = false;
                _velocity = Vector3.zero;
            }
            xrRig.position = position;
        }
    }

    public void Jump()
    {
        if (_isJumping) return; 
        _isJumping = true;
        var jumpSpeed = 0f;
        _currVelocity = avgVelocityEstimator.GetVelocity();
        jumpSpeed = CalculateJumpSpeed(_currVelocity.magnitude > 2f ? maxDunkHeight : maxJumpShotHeight, gravity.magnitude);
        _velocity = new Vector3(_currVelocity.x, jumpSpeed, _currVelocity.z);
    }

    private float CalculateJumpSpeed(float jumpHeight, float gravity)
    {
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    public float GetCurrentHeight()
    {
        return xrRig.transform.position.y;
    }
    
    public float GetGroundHeight()
    {
        return GROUND_HEIGHT;
    }

    public float GetMaxJumpHeight()
    {
        return maxJumpShotHeight;
    }

    public bool IsJumping()
    {
        return _isJumping;
    }
}

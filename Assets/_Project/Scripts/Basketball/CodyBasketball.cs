using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class CodyBasketball : Basketball
{
    private ShotController _shotController;
    private CodyDribble _dribbleController;
    private TeamHandler _teamHandler; 
    public Transform shotIndicator;
    public float indicatorIncrementValue;
    private bool isScalingUp = false;
    private Renderer shotIndicatorRenderer;
    public Color shotReadyColor;
    public Color shotNotReadyColor;
    private static readonly int Color = Shader.PropertyToID("_Color");
    public List<AudioClip> hitRimSounds = new List<AudioClip>();
    public List<AudioClip> floorBounceSounds = new List<AudioClip>();
    public AudioSource audioSource;
    public TwoHandDetection twoHandDetection;
    public TeamSetup teamSetup;
    private float velocity;
    private Vector3 previousPosition;
    private Vector3 FrameVelocity;
    private const float MAX_INDICATOR_SIZE = 0.32f;

    public enum ShotPoints
    {
        twoPoints = 2,
        threePoints = 3
    }

    public ShotPoints points;

    // Start is called before the first frame update
    void Start()
    {
        //ResetShotIndicator();
        shotIndicatorRenderer = shotIndicator.GetComponent<Renderer>();
    }

    private void FixedUpdate()
    {
        if (IsGrabbed())
        {
            CalculateVelocity();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.name == "Rim")
        {
            //play sound
            if (ThisPhotonView.IsMine)
            {
                RpcPlayRimSound();
            }
        }
        
        if (other.transform.name == "Floor")
        {
            //play sound
            if (ThisPhotonView.IsMine)
            {
                RpcPlayFloorBounceSound();
            }
        }
    }

    private void RpcPlayRimSound()
    {
        ThisPhotonView.RPC("PlayRimSound", RpcTarget.All);
    }
    
    [PunRPC]
    public void PlayRimSound()
    {
        var clip = hitRimSounds[Random.Range(0, hitRimSounds.Count)];
        audioSource.clip = clip;
        audioSource.Play();
    }
    
    private void RpcPlayFloorBounceSound()
    {
        ThisPhotonView.RPC("PlayFloorBounceSound", RpcTarget.All);
    }
    
    [PunRPC]
    public void PlayFloorBounceSound()
    {
        var clip = floorBounceSounds[Random.Range(0, floorBounceSounds.Count)];
        audioSource.pitch = Random.Range(.9f, 1.1f);
        audioSource.clip = clip;
        audioSource.Play();
    }

    public override void Grab(Interactor interactor)
    {
        base.Grab(interactor);
        ThisRigidbody.isKinematic = false;
        
        ResetShotIndicator();
        var root = interactor.transform.root;
        _shotController = root.GetComponent<ShotController>();
        _dribbleController = root.GetComponent<CodyDribble>();
        _teamHandler = root.GetComponent<TeamHandler>();
        if (_teamHandler)
        {
            teamSetup.RpcSetTeamInPossession(_teamHandler._teamNumber);
        }

        if (_dribbleController)
        {
            _dribbleController.SetInteractor(interactor);
            _dribbleController.SetCurrentBall(transform);
            _dribbleController.SetBasketballController(this);
        }
        
        if (_shotController)
        {
            _shotController.SetCurrentBall(transform);
            _shotController.SetCurrentBallRigidbody(ThisRigidbody);
            _shotController.SetBasketballController(this);
            _shotController.IsBallGrabbed(true);
        }
    }

    public override void UnGrab()
    {
        base.UnGrab();
        
        if (_teamHandler)
        {
            teamSetup.RpcSetTeamInPossession(-1);
        }
        
        if (_dribbleController)
        {
            _dribbleController.SetInteractor(null);
            _dribbleController.SetCurrentBall(null);
            _dribbleController.SetBasketballController(null);
        }

        if (_shotController)
        {
            _shotController.IsBallGrabbed(false);
            _shotController.SetBasketballController(null);
            _shotController = null;
            ThisRigidbody.velocity = FrameVelocity;
        }
    }

    [Button]
    public void StartShotIndicator()
    {
        isScalingUp = true;
        StartCoroutine(ScaleUpShotIndicator());
    }
    
    [Button]
    public void StopShotIndicator()
    {
        isScalingUp = false;
        StopCoroutine(ScaleUpShotIndicator());

        if (shotIndicator.localScale.x >= MAX_INDICATOR_SIZE)
        {
            shotIndicatorRenderer.material.SetColor(Color, shotReadyColor);
        }
    }

    public void LerpColorOfIndicator(float clampedColorValue)
    {
        Color lerpColor = UnityEngine.Color.Lerp(shotNotReadyColor, shotReadyColor, clampedColorValue);
        shotIndicatorRenderer.material.SetColor(Color, lerpColor);
    }

    private IEnumerator ScaleUpShotIndicator()
    {
        var currentIndicatorSize = shotIndicator.localScale.x;
        while (currentIndicatorSize < MAX_INDICATOR_SIZE && isScalingUp)
        {
            currentIndicatorSize += indicatorIncrementValue;
            shotIndicator.localScale = new Vector3(currentIndicatorSize, currentIndicatorSize, currentIndicatorSize);
            yield return null;
        }
        
        if (shotIndicator.localScale.x >= MAX_INDICATOR_SIZE)
        {
            shotIndicatorRenderer.material.SetColor(Color, shotReadyColor);
        }
    }

    [Button]
    public void ResetShotIndicator()
    {
        if (!shotIndicatorRenderer) return;
        shotIndicator.localScale = Vector3.zero;
        shotIndicatorRenderer.material.SetColor(Color, UnityEngine.Color.white);
    }

    public void SetIndicatorScale(float size)
    {
        shotIndicator.localScale = new Vector3(size, size, size);
    }

    public bool IsGrabbedByTwoHands()
    {
        return twoHandDetection.IsGrabbedByTwoHands();
    }
    
    private static float Remap (float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    private void CalculateVelocity()
    {
        //an average velocity due to fixed update irregularity, else we will occasionally get 0 velocity
        Vector3 currFrameVelocity = (transform.position - previousPosition) / Time.deltaTime;
        FrameVelocity = Vector3.Lerp(FrameVelocity, currFrameVelocity, 0.1f);
        previousPosition = transform.position;
    }

    public Vector3 GetVelocity()
    {
        return FrameVelocity;
    }

    public Rigidbody GetRigidbody()
    {
        return ThisRigidbody;
    }
    
    public float GetMaxIndicatorSize()
    {
        return MAX_INDICATOR_SIZE;
    }
}

using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class ShotDetectorThreePointContest : MonoBehaviour
{
    public PhotonView photonView;
    public AudioSource audioSource;
    public List<AudioClip> madeShot = new List<AudioClip>();
    public int points;
    private Basketball basketball;
    public TextMeshProUGUI scoreboard;
    public UnityEvent hasMadeShot;
    private const string REGULAR_BALL = "Basketball";
    private const string MONEY_BALL = "Moneyball";
    public OnFireCheck onFireCheck;

    private void OnTriggerEnter(Collider other)
    {
        basketball = other.GetComponent<Basketball>();
        if(basketball) CheckForDunk(basketball);

        if (basketball && !IsBallComingFromAbove(basketball.transform))
        {
            basketball.Live();
            basketball.UnGrab();
            basketball.BallRigidbody.velocity = Vector3.zero;
        }
        
        if (basketball)
        {
            //play sound
            if (photonView.IsMine)
            {
                other.GetComponent<ThreePointBall>().BallMade = true;
                RpcPlaySound();
                RpcAddPoints();
                hasMadeShot?.Invoke();
            }
        }
    }
    
    //needs to be planned out. 
    private void RpcAddPoints()
    {
        if(basketball.name.Equals(REGULAR_BALL))
        {
            onFireCheck.madeShots++;
            photonView.RPC("RPCUpdateScoreboardBall", RpcTarget.All);
            photonView.RPC(nameof(RPCBasketballPoint), RpcTarget.All);
            return;
        }
        if(basketball.name.Equals(MONEY_BALL))
        {
            onFireCheck.madeShots++;
            photonView.RPC("RPCUpdateScoreboardBall", RpcTarget.All);
            photonView.RPC(nameof(RPCMoneyballPoint), RpcTarget.All);
        }
    }
    
    [PunRPC]
    public void RPCBasketballPoint()
    {
        points++;
        scoreboard.text = points.ToString();
    }

    [PunRPC]
    public void RPCMoneyballPoint()
    {
        points+=2;
        scoreboard.text = points.ToString();
    }

    [PunRPC]
    public void RPCUpdateScoreboardBall()
    {
        basketball.GetComponent<ScoreboardRacks>().ChangeScoreboardMaterial();
    }

    private void RpcPlaySound()
    {
        photonView.RPC(nameof(PlaySound), RpcTarget.Others);
    }
    
    [PunRPC]
    public void PlaySound()
    {
        var clip = madeShot[Random.Range(0, madeShot.Count)];
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void CheckForDunk(Basketball basketball)
    {
        if (basketball.IsGrabbed())
        {
            basketball.Live();
            basketball.UnGrab();
        }
    }

    private bool IsBallComingFromAbove(Transform ball)
    {
        return ball.position.y > transform.position.y;
    }
    
}

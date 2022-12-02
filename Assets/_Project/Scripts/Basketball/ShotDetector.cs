using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class ShotDetector : MonoBehaviour
{
    public PhotonView photonView;
    public AudioSource audioSource;
    public List<AudioClip> madeShot = new List<AudioClip>();
    public int points;
    private Basketball basketball;
    public TextMeshProUGUI scoreboard;
    public UnityEvent hasMadeShot;

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
                RpcPlaySound();
                //RpcAddPoints();
                hasMadeShot?.Invoke();
            }
        }
    }
    
    //needs to be planned out. 
    /*private void RpcAddPoints()
    {
        photonView.RPC("AddPoints", RpcTarget.All);
    }
    
    [PunRPC]
    public void AddPoints()
    {
        points += (int)basketball.points;
        scoreboard.text = points.ToString();
    }*/

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

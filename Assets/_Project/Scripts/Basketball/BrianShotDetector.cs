using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class BrianShotDetector : MonoBehaviour
{
    public PhotonView photonView;
    public AudioSource audioSource;
    public List<AudioClip> madeShot = new List<AudioClip>();
    public int points;
    private CodyBasketball basketball;
    public TextMeshProUGUI scoreboard;
    [SerializeField] private List<string> horseLetters = new List<string>();
    private string boardLetters;
    private int timesFailed = 0;
    [SerializeField] private BrianShotDetectorTrigger1 trigger1;
    public bool isShotSuccessful;

    private void Start()
    {
        horseLetters.Add("H");
        horseLetters.Add("O");
        horseLetters.Add("R");
        horseLetters.Add("S");
        horseLetters.Add("E");
        
    }

    private void OnTriggerEnter(Collider other)
    {
        basketball = other.GetComponent<CodyBasketball>();
        if(basketball) CheckForDunk(basketball);

        if (basketball && !IsBallComingFromAbove(basketball.transform))
        {
            basketball.UnGrab();
            basketball.GetRigidbody().velocity = Vector3.zero;
        }

        if (basketball && IsBallComingFromAbove(basketball.transform) && trigger1.inFirstTrigger)
        {
            //play sound
            if (photonView.IsMine)
            {
                Debug.Log("success");
                isShotSuccessful = true;
                RpcPlaySound();
                RpcAddPoints();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (basketball)
        {
            isShotSuccessful = false;
        }
    }

    protected void RpcAddPoints()
    {
        photonView.RPC("AddPoints", RpcTarget.All);
    }
    
    [PunRPC]
    public void AddPoints()
    {
        boardLetters += horseLetters[timesFailed];
        timesFailed++;
        scoreboard.text = boardLetters;
    }

    protected void RpcPlaySound()
    {
        photonView.RPC("PlaySound", RpcTarget.All);
    }
    
    [PunRPC]
    public void PlaySound()
    {
        var clip = madeShot[Random.Range(0, madeShot.Count)];
        audioSource.clip = clip;
        audioSource.Play();
    }

    protected void CheckForDunk(CodyBasketball basketball)
    {
        if (basketball.IsGrabbed())
        {
            basketball.UnGrab();
        }
    }

    protected bool IsBallComingFromAbove(Transform ball)
    {
        return ball.position.y > transform.position.y;
    }
}

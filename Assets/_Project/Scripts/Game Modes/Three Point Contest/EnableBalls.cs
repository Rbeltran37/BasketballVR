using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableBalls : MonoBehaviour
{
    [SerializeField]
    private AudioSource countdownAudio;

    [SerializeField]
    private GameObject[] regularBasketballs;

    [SerializeField]
    private GameObject[] moneyballs;

    public float timerToEnableBalls;

    [SerializeField]
    private float resetTimerToEnableBalls;

    public GameTimer gameTimer;

    [SerializeField]
    private AudioSource crowdCheeringAudio;

    private void OnEnable()
    {
        timerToEnableBalls = resetTimerToEnableBalls;
    }

    // Update is called once per frame
    void Update()
    {
        timerToEnableBalls -= Time.deltaTime;
        if(timerToEnableBalls <= 0)
        {
            for(int i=0; i < regularBasketballs.Length; i++)
            {
                regularBasketballs[i].name = "Basketball";
            }

            for(int i=0; i < moneyballs.Length; i++)
            {
                moneyballs[i].name = "Basketball";
            }
            enabled = false;
            gameTimer.enabled = true;
            crowdCheeringAudio.Play();
        }
    }

    public void PlayAudio()
    {
        countdownAudio.Play();
    }
}

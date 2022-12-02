using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float gameTimer;

    [SerializeField]
    private float resetGameTimer;

    [SerializeField]
    private TMP_Text shotClockText;

    [SerializeField]
    private AudioSource buzzerAudio;

    public ChangeBallName changeBallName;
    private bool buzzerAudioPlayed;
    
    [SerializeField]
    private AudioSource crowdCheeringAudio;

    

    private void OnEnable()
    {
        gameTimer = resetGameTimer;
    }

    void Update()
    {
        gameTimer -= Time.deltaTime;
        shotClockText.text = gameTimer.ToString("F0");

        if(gameTimer <= 1.5f && !buzzerAudioPlayed)
        {
            buzzerAudio.Play();
            buzzerAudioPlayed = true;
        }

        if(gameTimer <= 0)
        {
            crowdCheeringAudio.Stop();
            changeBallName.ChangeToDeadBall();
            enabled = false;
        }
    }
}

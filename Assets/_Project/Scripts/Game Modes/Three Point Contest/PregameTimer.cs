using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PregameTimer : MonoBehaviour
{
    [SerializeField]
    private float shotClockTimer;

    [SerializeField]
    private float resetShotClockTimer;

    [SerializeField]
    private TMP_Text shotClockText;
    public EnableBalls enableBalls;
    public GameTimer gameTimer;
    
    private void OnEnable()
    {
        shotClockTimer = resetShotClockTimer;
    }
    void Update()
    {
        shotClockTimer -= Time.deltaTime;
        shotClockText.text = shotClockTimer.ToString("F0");

        if(shotClockTimer <= 0)
        {
            enabled = false;
            enableBalls.enabled = true;
            enableBalls.PlayAudio();
            shotClockText.text = gameTimer.gameTimer.ToString("F0");;
        }
    }
}

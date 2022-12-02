using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFireCheck : MonoBehaviour
{
    [SerializeField]
    private AudioSource heatingUpAudio;

    [SerializeField]
    private AudioSource onFireAudio;

    public int madeShots;
    private bool heatingUpAudioPlayed;
    private bool onFireAudioPlayed;

    void Update()
    {
        if(madeShots == 0)
        {
            heatingUpAudioPlayed = false;
            onFireAudioPlayed = false;
            return;
        }

        if(madeShots == 2 && !heatingUpAudioPlayed)
        {
            heatingUpAudio.Play();
            heatingUpAudioPlayed = true;
            return;
        }

        if(madeShots == 3 && !onFireAudioPlayed)
        {
            onFireAudio.Play();
            onFireAudioPlayed = true;
            return;
        }
    }
    public void HeatingUp()
    {
        heatingUpAudio.Play();
    }

    public void OnFire()
    {
        onFireAudio.Play();
    }
}

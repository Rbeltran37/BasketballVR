using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TeamMarker : MonoBehaviour
{
    public int teamNumber;
    public AudioSource audioSource;
    private bool _hasBeenActivated;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Interactor>() || _hasBeenActivated)
            return;
        var root = other.transform.root;
        var teamHandler = root.GetComponent<TeamHandler>();
        if (teamHandler)
        {
            //already has been assigned team. 
            if (teamHandler.GetTeam() != -1)
                return;
            
            teamHandler.RpcSetTeam(teamNumber);
        }
        StartCoroutine(PlaySoundAndDisable());
        _hasBeenActivated = true;
    }

    private IEnumerator PlaySoundAndDisable()
    {
        audioSource.Play();
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        gameObject.SetActive(false);
        yield return null;
    }
}

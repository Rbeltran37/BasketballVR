using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ReplayCapture : MonoBehaviour
{
    public List<Vector3> positions = new List<Vector3>();
    public List<Quaternion> rotations = new List<Quaternion>();
    private bool isRecording = false;
    private bool isPlaying = false;
    private Rigidbody rb;
    public Transform transformToRecord;
    public Transform transformToApplyTo;
    public Vector3 offset;
    private const int LENGTH = 500;
    private bool _isFinishedPlaying;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isRecording)
        {
            if (positions.Count >= LENGTH)
            {
                positions.RemoveAt(0);
                rotations.RemoveAt(0);
            }
            positions.Add(transformToRecord.position);
            rotations.Add(transformToRecord.rotation);
        }
    }

    [Button]
    public void StartRecording()
    {
        isRecording = true;
    }
    
    [Button]
    public void StopRecording()
    {
        isRecording = false;
    }
    
    [Button]
    public void PlayRecording()
    {
        isRecording = false;
        StartCoroutine(StartPlaying());
    }

    private IEnumerator StartPlaying()
    {
        int index = 0;
        while (index < positions.Count)
        {
            transformToApplyTo.position = positions[index] + offset;
            transformToApplyTo.rotation = rotations[index];
            index++;
            yield return null;
        }
        _isFinishedPlaying = true;
        yield return null;
    }

    public bool IsFinishedPlaying()
    {
        return _isFinishedPlaying;
    }

    public void Reset()
    {
        _isFinishedPlaying = false;
    }

    public void ClearData()
    {
        positions.Clear();
        rotations.Clear();
    }
}

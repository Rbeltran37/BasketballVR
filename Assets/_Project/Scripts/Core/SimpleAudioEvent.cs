using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "_AudioEvent", menuName="ScriptableObjects/Core/SimpleAudioEvent")]
public class SimpleAudioEvent : ScriptableObject
{
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private RangedFloat volume;
    [MinMaxFloatRange(0, 3)]
    [SerializeField] private RangedFloat pitch;

    private int _numClips;

    public void Play(AudioSource audioSource)
    {
        if (DebugLogger.IsNullOrEmptyError(clips, this, $"Must be set in editor.")) return;
        
        if (_numClips == 0) _numClips = clips.Length;

        if (DebugLogger.IsNullError(audioSource, this)) return;

        audioSource.clip = clips[Random.Range(0, clips.Length)];
        audioSource.volume = Random.Range(volume.minValue, volume.maxValue);
        audioSource.pitch = Random.Range(pitch.minValue, pitch.maxValue);
        audioSource.Play();
    }
    
    public void Play(AudioSource audioSource, float pitchValue)
    {
        if (clips.Length == 0) return;

        audioSource.clip = clips[Random.Range(0, clips.Length)];
        audioSource.volume = Random.Range(volume.minValue, volume.maxValue);
        audioSource.pitch = pitchValue;
        audioSource.Play();
    }

    public void Stop(AudioSource audioSource)
    {
        audioSource.Stop();
    }
}
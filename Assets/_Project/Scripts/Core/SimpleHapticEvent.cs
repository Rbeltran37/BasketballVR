using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "_HapticEvent", menuName="ScriptableObjects/Core/SimpleHapticEvent")]
public class SimpleHapticEvent : ScriptableObject
{
    [SerializeField] private RangedFloat amplitude;
    [MinMaxFloatRange(0, 3)]
    [SerializeField] private RangedFloat duration;

    private int _numClips;
    

    public void Play(ControllerHaptics controllerHaptics)
    {
        if (DebugLogger.IsNullError(controllerHaptics, this)) return;

        var randomAmplitude = Random.Range(amplitude.minValue, amplitude.maxValue);
        var randomDuration = Random.Range(duration.minValue, duration.maxValue);
        controllerHaptics.ActivateHaptics(randomAmplitude, randomDuration);
    }
    
    public void Play(bool isLeftController)
    {
        var randomAmplitude = Random.Range(amplitude.minValue, amplitude.maxValue);
        var randomDuration = Random.Range(duration.minValue, duration.maxValue);
        ControllerHaptics.ActivateHaptics(randomAmplitude, randomDuration, isLeftController);
    }
    
    public void Play(ControllerHaptics controllerHaptics, float amplitudeValue, float durationValue)
    {
        if (DebugLogger.IsNullError(controllerHaptics, this)) return;

        controllerHaptics.ActivateHaptics(amplitudeValue, durationValue);
    }
}
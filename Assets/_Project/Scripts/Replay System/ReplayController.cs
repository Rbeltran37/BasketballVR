using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class ReplayController : MonoBehaviour
{
    public List<ReplayCapture> captures = new List<ReplayCapture>();
    public Renderer renderer;
    public Camera camera;
    public Transform backboardPosition;
    public Transform courtsidePosition;
    public float transitionTime;
    private Color _lerpedColor;
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
    private Vector3 _offsetPosition = new Vector3(0,0,-30);
    private bool _IsRecording = false;
    public PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        //This could be called when game starts so it's not always running. 
        StartRecording();
        StartCoroutine(FadeIn());
        SwitchToBackboardCamera();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button]
    public void StartRecording()
    {
        foreach (var replayCapture in captures)
        {
            replayCapture.StartRecording();
        }
    }
    
    [Button]
    public void StopRecording()
    {
        if (!photonView.IsMine) return;
        photonView.RPC(nameof(RPCStopRecording), RpcTarget.All);
    }

    [PunRPC]
    private void RPCStopRecording()
    {
        StartCoroutine(DelayStopRecording());
    }
    
    [Button]
    public void PlayRecording()
    {
        StartCoroutine(FadeOut());
        camera.enabled = true;
        foreach (var replayCapture in captures)
        {
            replayCapture.PlayRecording();
        }

        StartCoroutine(WaitForRecordingToFinish());
    }

    private IEnumerator WaitForRecordingToFinish()
    {
        //gets last capture in list. 
        var lastCapture = captures[captures.Count-1];
        while (!lastCapture.IsFinishedPlaying())
        {
            yield return null;
        }

        StartCoroutine(FadeIn());
        
        camera.enabled = false;
        ClearRecordings();
        ResetCaptures();
        StartRecording();
    }

    private IEnumerator FadeOut()
    {
        float transitionRate = 0;
        while(transitionRate < 1)
        {
            //this next line is how we change our material color property. We Lerp between the current color and newColor
            _lerpedColor = Color.Lerp(Color.white, Color.black, Time.deltaTime * transitionRate);
            renderer.material.SetColor(BaseColor, _lerpedColor);
            transitionRate += Time.deltaTime / transitionTime; // Increment transitionRate over the length of transitionTime
            yield return null; // wait for a frame then loop again
        }
    }
    
    private IEnumerator FadeIn()
    {
        float transitionRate = 0;
        while(transitionRate < 1)
        {
            //this next line is how we change our material color property. We Lerp between the current color and newColor
            _lerpedColor = Color.Lerp(Color.black, Color.white, Time.deltaTime * transitionRate);
            renderer.material.SetColor("_BaseColor", _lerpedColor);
            transitionRate += Time.deltaTime / transitionTime; // Increment transitionRate over the length of transitionTime
            yield return null; // wait for a frame then loop again
        }
    }

    private IEnumerator DelayStopRecording()
    {
        yield return new WaitForSeconds(1.5f);
        
        foreach (var replayCapture in captures)
        {
            replayCapture.StopRecording();
        }

        yield return null;
        
        PlayRecording();
    }
    
    public void ClearRecordings()
    {
        foreach (var replayCapture in captures)
        {
            replayCapture.ClearData();
        }
    }

    public void SwitchToBackboardCamera()
    {
        var cam = camera.transform;
        cam.position = backboardPosition.position;
        cam.rotation = backboardPosition.rotation;
    }

    public void SwitchToSideCamera()
    {
        var cam = camera.transform;
        cam.position = courtsidePosition.position;
        cam.rotation = courtsidePosition.rotation;
    }

    public void ResetCaptures()
    {
        foreach (var replayCapture in captures)
        {
            replayCapture.Reset();
        }
    }

    public Vector3 GetOffsetPosition()
    {
        return _offsetPosition;
    }
}

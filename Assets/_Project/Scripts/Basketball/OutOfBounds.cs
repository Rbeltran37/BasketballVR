using System.Collections;
using SimpleBallistic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    public bool inGame;
    public bool isOutOfBounds;
    public AudioSource outOfBoundsAudioSource;
    private const float BALL_REPOSITION_DELAY = 2f;
    private Basketball _currBall;
    public Transform ballSpawnPosition;
    private float timer = 0.0f;
    private bool hasMoved = true;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasMoved)
        {
            Move(_currBall);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _currBall = other.transform.GetComponent<Basketball>();
        if (inGame && _currBall && !isOutOfBounds)
        {
            outOfBoundsAudioSource.transform.position = _currBall.transform.position;
            outOfBoundsAudioSource.Play();
            inGame = false;
            isOutOfBounds = true;
            hasMoved = false;
        }
    }

    private void Move(Basketball currBall)
    {
        timer += Time.deltaTime;
        if(timer >= BALL_REPOSITION_DELAY){
            if (!currBall) return;
            var rb = currBall.BallRigidbody;
            if(rb) rb.isKinematic = true;
            currBall.transform.position = ballSpawnPosition.position;
            Reset();
            timer = 0.0f;
            hasMoved = true;
        }
    }

    public void Reset()
    {
        inGame = true;
        isOutOfBounds = false;
    }
}

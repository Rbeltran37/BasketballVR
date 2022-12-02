using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardRacks : MonoBehaviour
{
    public Material madeBasketball;
    public GameObject scoreboardBall;

    public void ChangeScoreboardMaterial()
    {
        scoreboardBall.GetComponent<MeshRenderer>().material = madeBasketball;
    }
}

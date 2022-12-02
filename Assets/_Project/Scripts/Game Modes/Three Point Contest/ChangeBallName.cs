using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBallName : MonoBehaviour
{
    [SerializeField]
    private GameObject[] basketballs;

    public void ChangeToDeadBall()
    {
        for(int i=0; i < basketballs.Length; i++)
        {
            basketballs[i].name = "Deadball";
        }
    }
}

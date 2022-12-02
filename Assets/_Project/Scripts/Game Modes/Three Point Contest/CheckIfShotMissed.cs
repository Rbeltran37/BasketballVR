using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfShotMissed : MonoBehaviour
{
    public OnFireCheck onFireCheck;
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ball")
        {
            if(other.gameObject.GetComponent<ThreePointBall>().deaball == false)
            {
                if(other.gameObject.GetComponent<ThreePointBall>().ballisShot)
                {
                    if(other.gameObject.GetComponent<ThreePointBall>().BallMade == false)
                    {
                        onFireCheck.madeShots = 0;
                    }
                    other.gameObject.GetComponent<ThreePointBall>().enabled = false;
                    other.gameObject.GetComponent<ThreePointBall>().deaball = true;
                    
                }
            }
        }
    }
}

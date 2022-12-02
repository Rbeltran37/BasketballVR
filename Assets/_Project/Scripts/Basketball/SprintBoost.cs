using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintBoost : MonoBehaviour
{
    [SerializeField] private float totalBoost = 100;
    [SerializeField] private SmoothLocomotion locomotion;
    [SerializeField] private float consumptionAmount;
    [SerializeField] private float recoveryAmount;
    
    void Update()
    {
        if (locomotion.IsSprinting())
        {
            if (totalBoost <= 0)
            {
                totalBoost = 0;
                locomotion.Jog();
                return;
            }
            
            //enable boost effects
            totalBoost -= consumptionAmount;
        }

        if (!locomotion.IsSprinting())
        {
            if (totalBoost >= 100)
            {
                totalBoost = 100;
                return;
            }
            
            //disable boost effects
            totalBoost += recoveryAmount;
        }
    }
}

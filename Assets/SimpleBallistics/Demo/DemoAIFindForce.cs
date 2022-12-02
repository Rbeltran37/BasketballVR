using UnityEngine;
using SimpleBallistic;

public class DemoAIFindForce : DemoCanon
{
    [SerializeField]
    private float m_Angle = 35;

    void Update()
    {
        // Get the needed force with the given parameter
        float firingSolution = Ballistics.GetForce(transform.position, m_Target.transform.position, m_Angle);
        
        Shoot(m_Angle, firingSolution);
    }
}

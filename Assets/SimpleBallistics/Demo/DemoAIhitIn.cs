using UnityEngine;
using SimpleBallistic;

public class DemoAIhitIn : DemoCanon {

    [SerializeField]
    private float m_Time = 1;

    void Update()
    {
        // Get the needed force and angle to hit the target in the given time
        float[] firingSolution = Ballistics.GetAngleAndForceWithTime(transform.position, m_Target.transform.position, m_Time);
        Shoot(firingSolution[0], firingSolution[1]);
    }
}

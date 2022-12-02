using UnityEngine;
using SimpleBallistic;

public class DemoAIFindAngle : DemoCanon
{
    [SerializeField]
    private float m_Force = 50;

	// Update is called once per frame
	void Update ()
    {
        // Get the needed angle with the given parameter
        float[] firingSolution = Ballistics.GetAngle(transform.position, m_Target.transform.position, m_Force);

        // If the shot is possible
        if (firingSolution != null)
        {
            Shoot(firingSolution[1], m_Force);
        }
    }
}

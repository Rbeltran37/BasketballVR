using UnityEngine;
using SimpleBallistic;

public class DemoCanon : MonoBehaviour
{
    [SerializeField]
    protected float m_ShootEach = 1;
    protected float m_Timer = 0;
    [SerializeField]
    protected GameObject m_Projectile;
    [SerializeField]
    protected GameObject m_Target;
    [SerializeField]
    protected LineRenderer m_LineRenderer = null;

    protected void Shoot(float angle, float force)
    {
        m_Timer += Time.deltaTime;
        LookAtAngle(angle);
        if (m_Timer >= m_ShootEach)
        {
            m_Timer = 0;
            GameObject canonBall = Instantiate(m_Projectile, transform.position, Quaternion.identity) as GameObject;                 
            canonBall.GetComponent<Rigidbody>().velocity = transform.forward * force;
            Destroy(canonBall, 20f);
        }
        Ballistics.TrajectoryProjection(transform.position, transform.forward, force, angle, 100, 0.5f, m_LineRenderer);
    }

    protected void LookAtAngle(float _angle)
    {
        Vector3 axis = m_Target.transform.position - transform.position;
        axis.y = 0;
        axis.Normalize();
        axis = Quaternion.AngleAxis(_angle, Vector3.Cross(axis, Vector3.up)) * axis;
        transform.forward = axis;
    }
}

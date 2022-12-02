using UnityEngine;

// Just a sample target that destroy the object that hit it and then teleport in a random position
public class DemoTarget : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_RandomRange = new Vector3(3, 3, 3);

    private Vector3 m_InitialPos;

    private void Start()
    {
        m_InitialPos = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(collision.gameObject);
        
        float randX = Random.Range(-m_RandomRange.x, m_RandomRange.x);
        float randY = Random.Range(-m_RandomRange.y, m_RandomRange.y);
        float randZ = Random.Range(-m_RandomRange.z, m_RandomRange.z);
        Vector3 newPos = new Vector3(randX, randY, randZ);
        transform.position = m_InitialPos + newPos;
    }

}

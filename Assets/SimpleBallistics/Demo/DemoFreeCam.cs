using UnityEngine;

public class DemoFreeCam : MonoBehaviour
{

    [SerializeField]
    private float m_FlySpeed = 30;
    [SerializeField]
    private float m_MouseSensitivity = 3;

    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;      
        transform.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * m_MouseSensitivity;

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += transform.forward * m_FlySpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            transform.position -= transform.forward * m_FlySpeed * Time.deltaTime;
        }
    }
}

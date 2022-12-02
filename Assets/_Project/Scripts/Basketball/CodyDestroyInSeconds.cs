using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodyDestroyInSeconds : MonoBehaviour
{
    public float timeTillDestroy;
    
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timeTillDestroy);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

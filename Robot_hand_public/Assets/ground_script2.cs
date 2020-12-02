using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ground_script2 : MonoBehaviour
{
    private int count =0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        count++;
       
    }

    void Reset()
    {
        count = 0;
    }

    int get_count()
    {
        return count;
    }
}

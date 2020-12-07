using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1 : MonoBehaviour
{
    public GameObject up, down;
    float UpSpeed;
    float DownSpeed;

    // Start is called before the first frame update
    void Start()
    {
        UpSpeed = 10f;
        DownSpeed = -10f;
    }

    // Update is called once per frame
    void Update()
    {
        if (up.transform.position.y > 5)
        {
            UpSpeed = 0;
        }
        else
        {
            UpSpeed = 10f;
        }
        if (down.transform.position.y < -5)
        {
            DownSpeed = 0;
        }
        else
        {
            DownSpeed = -10f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(0, UpSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            transform.Translate(0, DownSpeed * Time.deltaTime, 0);
        }
    }

}
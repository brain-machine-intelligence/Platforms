using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground_SC : MonoBehaviour
{
    private int count = 0;
    private List<GameObject> collided = new List<GameObject>();

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

        if (collision.gameObject.tag.Equals("Item"))
        {
            if (!collided.Contains(collision.gameObject))
            { 
                count++;
                collided.Add(collision.gameObject);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Item"))
        {
            if (!collided.Contains(collision.gameObject))
            {
                collided.Add(collision.gameObject);
            }
        }

    }

    public bool Is_grounded(GameObject gobj)
    {
        if (!gobj.tag.Equals("Item"))
            return false;

        if (!collided.Contains(gobj))
        {
            return false;
        }
        else
        {
            return true;
        }

    }

    public void Reset()
    {
        count = 0;
        collided.Clear();
    }

    public int Get_count()
    {
    
        return count-1;
    }
}

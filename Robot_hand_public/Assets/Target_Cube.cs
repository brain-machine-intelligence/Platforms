using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_Cube : MonoBehaviour
{
    private int count = 0;
    Collider myCollider;

    List<Collider> Initial_friends = new List<Collider>();


    // Start is called before the first frame update
    void Start()
    {

        myCollider = this.transform.GetComponent<Collider>();
    }

    /*
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.gameObject.tag == "Item")
        {
            count += 1;
            print(count);
        }
    
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.gameObject.tag == "Item")
        { 
            count -= 1;
            print(count);
        }
    }*/

    public int[] check_state()
    {
        int count_moving_out = 0;
        int count_moving_in = 0;

        foreach (Transform trans in this.transform.parent)
        {
            if (trans.gameObject.tag == "Item")
            {
                MeshCollider col = new MeshCollider();
                col = trans.GetChild(0).GetComponent<MeshCollider>();

                if (Initial_friends.Contains(col))
                {
                    if (myCollider.bounds.Contains(col.bounds.center))
                        continue;
                    else
                        count_moving_out += 1;


                } else if (myCollider.bounds.Contains(col.bounds.center))
                    count_moving_in += 1;
            }

        }
        return new int[]{ count_moving_in, count_moving_out};
    }


    public int Get_count(GameObject grabbed_object, int released_object)
    {
        int tem_count = 0;
        foreach (Transform trans in this.transform.parent)
        {
            if (trans.gameObject.tag == "Item")
            {
                MeshCollider col = new MeshCollider();
                col = trans.GetChild(0).GetComponent<MeshCollider>();

                if (Initial_friends.Contains(col))
                {
                    if (myCollider.bounds.Contains(col.bounds.center))
                        continue;
                    
                }
                if (myCollider.bounds.Contains(col.bounds.center))
                {
                    if (grabbed_object != null && grabbed_object.Equals(trans.gameObject))
                        continue;

                    Initial_friends.Add(col);
                    if(released_object == 0)
                    { 
                        count += 1;
                        tem_count += 1;
                    }else if (released_object == trans.gameObject.GetInstanceID())
                    {
                        count += 1;
                        tem_count += 5;
                    }
                    else
                    {
                        count += 1;
                        tem_count += 1;
                    }
                }
            }
                
        }
        return tem_count;
    }

    public void Start_Counting()
    {
        foreach (Transform trans in this.transform.parent)
        {
            if (trans.gameObject.tag == "Item")
            {
                MeshCollider col = new MeshCollider();
                col = trans.GetChild(0).GetComponent<MeshCollider>();

                if (myCollider.bounds.Contains(col.bounds.center))
                    Initial_friends.Add(trans.GetChild(0).GetComponent<MeshCollider>());
            }

        }
    }


    /*

    public int Get_count()
    {
        return count;
    }*/


    public void Reset()
    {
        count = 0;
        Initial_friends.Clear();
    }


}

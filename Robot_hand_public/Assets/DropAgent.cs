using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropAgent : MonoBehaviour
{

    public GameObject object_type1;
    public GameObject object_type2;
    public GameObject object_type3;
    public GameObject object_type4;


    public GameObject boundary_box;


    private float action_boundary_max_x;
    private float action_boundary_min_x;
    private float action_boundary_max_y;
    private float action_boundary_min_y;
    private float action_boundary_max_z;
    private float action_boundary_min_z;

    private List<GameObject> drop_object_list = new List<GameObject>();

    private int action_total_type = 4;
    private const float time_delay =5.0f;
    private float timeWait = time_delay;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 10;
        Time.timeScale = 1;
        action_boundary_max_x = (float)(boundary_box.transform.localPosition.x + boundary_box.transform.localScale.x * 0.5);
        action_boundary_min_x = (float)(boundary_box.transform.localPosition.x - boundary_box.transform.localScale.x * 0.5);
        action_boundary_max_y = (float)(boundary_box.transform.localPosition.y + boundary_box.transform.localScale.y * 0.5);
        action_boundary_min_y = (float)(boundary_box.transform.localPosition.y - boundary_box.transform.localScale.y * 0.5);
        action_boundary_max_z = (float)(boundary_box.transform.localPosition.z + boundary_box.transform.localScale.z * 0.5);
        action_boundary_min_z = (float)(boundary_box.transform.localPosition.z - boundary_box.transform.localScale.z * 0.5);

        drop_object_list.Add(object_type1);
        drop_object_list.Add(object_type2);
        drop_object_list.Add(object_type3);
        drop_object_list.Add(object_type4);

    }


    public void Spawn_Object(GameObject obj, float x, float y, float z)
    {
        
        float posx = x;
        float posy = y;
        float posz = z;
        GameObject Parent_Object;

        Parent_Object = this.transform.parent.gameObject;

        posx += Parent_Object.transform.position.x;
        posy += Parent_Object.transform.position.y;
        posz += Parent_Object.transform.position.z;

       
        Vector3 objPos = new Vector3(posx, posy, posz);

        print("Spawned");
        Quaternion objRot = Quaternion.EulerAngles(obj.transform.eulerAngles);
        
        var instantiate_result = Instantiate(obj, objPos, objRot);
        instantiate_result.transform.parent = Parent_Object.transform;
        instantiate_result.transform.GetChild(0).gameObject.layer = 11;
        instantiate_result.layer = 11;
 

    }


    // Update is called once per frame
    void FixedUpdate()
    {

        timeWait -= Time.deltaTime;

        if (timeWait > 0.0f)
            return;


        // random drop
        float x_random = UnityEngine.Random.Range(action_boundary_min_x, action_boundary_max_x);
        float y_random = UnityEngine.Random.Range(action_boundary_min_y, action_boundary_max_y);
        float z_random = UnityEngine.Random.Range(action_boundary_min_z, action_boundary_max_z);


        int action_random = (int)(UnityEngine.Random.Range(0, action_total_type-1));


        Spawn_Object(drop_object_list[action_random], x_random, y_random, z_random);


        timeWait = time_delay;


    }
}

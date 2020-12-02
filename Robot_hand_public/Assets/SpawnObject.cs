using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class SpawnObject : MonoBehaviour
{
    public GameObject container;
    public GameObject C1;
    public GameObject C2;
    public GameObject C3;
    public GameObject C4;  

    public List<GameObject> prefabs = new List<GameObject>();
    public List<int> amounts = new List<int>();
    public List<bool> rotates = new List<bool>();
    List<Vector3> goPosInScene = new List<Vector3>();
    private List<GameObject> generated = new List<GameObject>();
    private Transform contTransform;
    private GameObject Parent_Object;
    private GameObject gObj;

    private List<GameObject> shakingObjAuto = new List<GameObject>();

    private int[,,] voxel_repre;

    private readonly float x_min_range = -2f;
    private readonly float x_max_range = -0.20f;  // 1.8
    private readonly float y_min_range = -0.05f;  
    private readonly float y_max_range = 0.25f;   // 0.6
    private readonly float z_min_range = -0.95f;
    private readonly float z_max_range = 0.35f;    // 1.2

    private readonly float interval = 0.05f;

    private bool spawn_finish = false;
    private bool fixed_check = false;


    private bool first_checked = false;

    private bool check_pls = false;
    private float eps = 0.1f;

    private float timeWait = 13.0f;
    private AllData actVoxRew;
    private bool training_over_test = false;
    List<string> linesToWrite = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        Parent_Object = this.transform.parent.gameObject;
        gObj = this.transform.gameObject; 

        //spawns objects based on inputted prefabs, amount values, and rotation boolean 
        if (prefabs.Count == amounts.Count && prefabs.Count == rotates.Count && amounts.Count == rotates.Count)
        {
            Spawn_Objects_new(prefabs, amounts, rotates);
            /*
            for (int i = 0; i < prefabs.Count; i++)
            {
                SpawnObjects(prefabs[i], amounts[i], rotates[i]); 
            }*/

            foreach (GameObject gen in generated)
            {
                goPosInScene.Add(new Vector3(gen.transform.localPosition.x, gen.transform.localPosition.y, gen.transform.localPosition.z));
                
            } 

        } 
        //ensures that # of prefabs, # of amounts, and # of bools in the 3 public lists are the same 
        else
        {
            Debug.LogError("Lists not of same length, cannot spawn objects. Update Lists."); 
        }
        //VoxelInfo scr = gObj.GetComponent<VoxelInfo>();
        //if (scr.isActiveAndEnabled) scr.VoxelGen();
        foreach (Transform child in this.transform.parent)
        {
            if (child.gameObject.tag.Equals("Table"))
            {
                foreach (Transform child_tr in child.transform)
                {
                    if (child_tr.gameObject.tag.Equals("Table"))
                    {
                        shakingObjAuto.Add(child_tr.gameObject);
                    }
                }

                shakingObjAuto.Add(child.gameObject);

            }
        }

        gameObject.GetComponent<VoxelInfo>().VoxelInfoOnStart();

        foreach (GameObject gen in generated)
        {
            goPosInScene.Add(new Vector3(gen.transform.localPosition.x, gen.transform.localPosition.y, gen.transform.localPosition.z));

        }

        spawn_finish = true;
        //Estimate_voxel();
        //StartCoroutine(GetEulers(8f, 0)); 
    } 

    public void GetEulers(int action_count, List<List<Vector3[]>> voxelTransform, List<List<int>> objectTypes, List<List<float>> objectVisibility, bool collect_data)
    {
        if(!collect_data)
        {
            return;
        }
        VoxelInfo scr = gObj.GetComponent<VoxelInfo>();

        if (scr.isActiveAndEnabled)
        {
            scr.VoxelGen(generated, voxelTransform, objectTypes, objectVisibility, collect_data);
        }
        
        //return voxelTransform;
    }

    public bool get_spawn_finish()
    {
        return spawn_finish;
    }

    public void Do_check()
    {
        check_pls = true;
        // print("Hello spawn");
    }

    public bool get_fixed_check()
    {
        return fixed_check;
    }

    //spawns new object in the scene within bounds that are set by 4 empty objects set at each 
    //side of the table to create a boundary within which the new objects' center can be placed
    public void Spawn_Objects_new(List<GameObject> objs, List<int> amounts, List<bool> rotates)
    {
        contTransform = container.transform;

        int game_obj_num = objs.Count;

        List<float> pos_x_arr = new List<float>();
        List<float> pos_z_arr = new List<float>();

        for(int ii=0; ii< game_obj_num; )  // Random location Sampler
        {
            bool is_valid = true;
            float pos_x = UnityEngine.Random.Range(C1.transform.localPosition.x, C4.transform.localPosition.x);
            float pos_z = UnityEngine.Random.Range(C3.transform.localPosition.z, C2.transform.localPosition.z);

            for(int jj=0; jj< pos_x_arr.Count; jj++)  // check the closeness
            {
                if(Math.Abs(pos_x_arr[jj] - pos_x) < 0.35 && Math.Abs(pos_z_arr[jj] - pos_z) < 0.35)
                {
                    is_valid = false;
                }

            }

            if(is_valid)
            {
                pos_x_arr.Add(pos_x);
                pos_z_arr.Add(pos_z);
                ii++;
            }
            
        }

        for(int ii=0; ii < game_obj_num; ii++)
        {
            GameObject obj = objs[ii];
            int amount = amounts[ii];
            bool rotate = rotates[ii];

            float[] posx = new float[amount];
            float[] posy = new float[amount];
            float[] posz = new float[amount];
            Vector3[] objPos = new Vector3[amount];
            Quaternion[] objRot = new Quaternion[amount];

            posx[0] = pos_x_arr[ii];
            posy[0] = contTransform.localPosition.y;
            posz[0] = pos_z_arr[ii];

            posx[0] += Parent_Object.transform.position.x;
            posy[0] += Parent_Object.transform.position.y;
            posz[0] += Parent_Object.transform.position.z;

            for (int i = 0; i < amount - 1; i++)
            {
                posx[i + 1] = posx[0];
                posy[i + 1] = posy[0] + (0.72f * (i + 1));
                posz[i + 1] = posz[0];
            }
            for (int i = 0; i < amount; i++)
            {
                objPos[i] = new Vector3(posx[i], posy[i], posz[i]);
                if (rotate == true)
                {
                    objRot[i] = new Quaternion(0, UnityEngine.Random.Range(-contTransform.localRotation.y / 2, contTransform.localRotation.y / 2), 0, 1);
                }
                else
                {
                    
                    objRot[i] = Quaternion.EulerAngles(obj.transform.eulerAngles);
                }
                var instantiate_result = Instantiate(obj, objPos[i], objRot[i]);
                instantiate_result.transform.parent = Parent_Object.transform;
                instantiate_result.transform.GetChild(0).gameObject.layer = 11;
                instantiate_result.layer = 11;
                generated.Add(instantiate_result);
            }


        }
                

    }

    public void SpawnObjects(GameObject obj, int amount, bool rotate)
    {
        contTransform = container.transform;
        float[] posx = new float[amount]; 
        float[] posy = new float[amount];
        float[] posz = new float[amount];
        Vector3[] objPos = new Vector3[amount]; 
        Quaternion[] objRot = new Quaternion[amount]; 

        posx[0] = UnityEngine.Random.Range(C4.transform.localPosition.x, C1.transform.localPosition.x); 
        posy[0] = contTransform.localPosition.y;
        posz[0] = UnityEngine.Random.Range(C2.transform.localPosition.z, C3.transform.localPosition.z);

        posx[0] += Parent_Object.transform.position.x;
        posy[0] += Parent_Object.transform.position.y;
        posz[0] += Parent_Object.transform.position.z;


        /*
        if (posz[0] < 0)
        {
            posz[0] = posz[0] + 0.2f;
        }
        else
        {
            posz[0] = posz[0] - 0.2f;
        }
        */
        //stacks the 'amount' of objects on top of one another 
        for (int i = 0; i < amount - 1; i++)
        {
            posx[i + 1] = posx[0];
            posy[i + 1] = posy[0] + (0.6f * (i + 1));
            posz[i + 1] = posz[0];
        }
        for (int i = 0; i < amount; i++)
        { 
            objPos[i] = new Vector3(posx[i], posy[i], posz[i]);  
            if (rotate == true)
            {
                objRot[i] = new Quaternion(0, UnityEngine.Random.Range(-contTransform.localRotation.y / 2, contTransform.localRotation.y / 2), 0, 1);
            }
            else
            {
                objRot[i] = obj.transform.localRotation; 
            }
            var instantiate_result = Instantiate(obj, objPos[i], objRot[i]);
            instantiate_result.transform.parent = Parent_Object.transform;
            generated.Add(instantiate_result);  
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int[,,] Estimate_voxel()
    {
        int x_len = (int)((x_max_range - x_min_range) / interval);
        int y_len = (int)((y_max_range - y_min_range) / interval);
        int z_len = (int)((z_max_range - z_min_range) / interval);
        //print("x_len: " + x_len);
        //print("y_len: " + y_len); 
        //print("z_len: " + z_len); 
        voxel_repre = new int[x_len, y_len, z_len];
        //print(voxel_repre);
        //for (int x = 0; x < x_len; x++)
        //    for (int y = 0; y < y_len; y++)
        //        for (int z = 0; z < z_len; z++)
        //        {
        //            Vector3 target_pos = new Vector3(x_min_range + (interval * x), y_min_range + (interval * y), z_min_range + (interval * z));
        //            print("target_pos: " + target_pos);
                    //            foreach ( GameObject g_obj in generated)
                    //            {
                    //                Collider col = g_obj.GetComponent<Collider>();
                    //                print(col); 

                    //                //Vector3 center = col.bounds.center - new Vector3(Parent_Object.transform.position.x, Parent_Object.transform.position.y, Parent_Object.transform.position.z);


                    //                if (col.bounds.Contains(target_pos + new Vector3(Parent_Object.transform.position.x, Parent_Object.transform.position.y, Parent_Object.transform.position.z)))
                    //                {
                    //                    //print("Hello voxel");
                    //                    //print(x_len * y_len * z_len);
                    //                    //print(g_obj.name);
                    //                    voxel_repre[x, y, z] = 1;
                    //                    break;
                    //                }

                    //                /*
                    //                Vector3 direction = center - target_pos;
                    //                Ray ray = new Ray(target_pos, direction);
                    //                RaycastHit hitinfo;

                    //                bool hit= col.Raycast(ray, out hitinfo, direction.magnitude);

                    //                if (!hit)   // point is inside the collider
                    //                {
                    //                    print("Hello voxel");
                    //                    print(x_len * y_len * z_len);
                    //                    voxel_repre[x, y, z] = 1;
                    //                    break;
                    //                }
                    //                */
                    //            }
                    //  }

                    return voxel_repre;
    }

    public void ActivateEarthquakeAuto()
    {
       
        StartCoroutine(EarthQuake_n_Stop(1.5f));
       
    }
    
    private bool Spawn_Check()
    {
        bool is_spawned = false;

        foreach(GameObject gener_obj in generated)
        {
            if (gener_obj.activeInHierarchy)
            { 
                is_spawned = true;
                continue;
            }
            else
            { 
                is_spawned = false;
                break;
            }
        }
        return is_spawned;
    }


    private void FixedUpdate()
    {  
        if (check_pls)
        {

            timeWait -= Time.deltaTime;
            
            if (spawn_finish && !fixed_check)
            { 
                if(timeWait > 10.0f)  // pls wait more to make object actually spawned 
                {
                    return;
                }

                if(timeWait <0)
                {
                    print("time out");
                    timeWait = 13.0f;
                    check_pls = false;
                    fixed_check = true;
                    return;
                }
                foreach (GameObject go in generated)
                {
                    if (go.GetComponent<Rigidbody>().velocity.magnitude > eps)
                    {
                        if (go.transform.localPosition.magnitude > 100)
                            continue;
                        
                        return;
                    }

                }           
                timeWait = 13.0f;
                check_pls = false;
                fixed_check = true;
            }

        }

    }

    IEnumerator EarthQuake_n_Stop(float sec)
    {
        List<Quaternion> rot_origin = new List<Quaternion>();

        foreach (GameObject Table in shakingObjAuto)
        {
            Quaternion origin_rot = Table.transform.localRotation;
            rot_origin.Add(origin_rot);
        }


        foreach (GameObject Table in shakingObjAuto)
        {
            
            Vector3 origin_rot = Table.transform.eulerAngles;

            iTween.RotateTo(Table, iTween.Hash("x", 15, "easeType", "easeInOutBack", "loopType", "pingPong", "delay", 0.05, "time", 0.15));

        }
        yield return new WaitForSeconds(sec);

        for (int i = 0; i< shakingObjAuto.Count; i++)
        {

            iTween.Stop(shakingObjAuto[i]);
            shakingObjAuto[i].transform.localRotation = rot_origin[i];
        }
    }

    public void Reset()
    {
        check_pls = false;
        spawn_finish = false;
        fixed_check = false; 

        foreach (GameObject instantiated in generated)
        {
            Destroy(instantiated);
            
        } 
        generated.Clear();

        Spawn_Objects_new(prefabs, amounts, rotates);
        /*
        for (int i = 0; i < prefabs.Count; i++)
        {
            SpawnObjects(prefabs[i], amounts[i], rotates[i]);
            
        } */
        /*

        if (generated != null)
        {
            for (int i = 0; i < generated.Count; i++)
            { 
                generated[i].transform.localPosition = goPosInScene[i]; 
            }
        }  
        */
        spawn_finish = true;
        fixed_check = false;
        timeWait = 13.0f; 
    }

    public float Evaluate_Action()
    {
        float score = 0.0f;
        /*
        foreach (GameObject game_obj in generated)
        {

           
            //if (game_obj.transform.Position.y < 0.05)
            if (game_obj.transform.position.y < 0.05)
            {
                score -= 1.0f;
            }
            game_obj.transform.up.y

        }
        */


        Reset();

        return score;
    }
}


     


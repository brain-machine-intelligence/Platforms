using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System;

public class VoxelInfo_wgan : MonoBehaviour
{
    public Camera agentCamera;
    public LayerMask layerMask;

    private GameObject pgo;
    private string goName;
    private string goNameNew;
    private string folderPath; 

    private StreamReader theReader;
    private string voxelLines;
    private List<int[]> voxelVals = new List<int[]>();
    
    private List<Vector3[]> transformVals = new List<Vector3[]>();
    
    private List<Vector3> init_pos = new List<Vector3>();
    private List<Vector3> init_rot = new List<Vector3>();
    private List<string> init_name = new List<string>();
    
    private List<string> goNames = new List<string>();


    private GameObject ground_Object;


    private string tempGoNameNew;
    private int tempindex1;
    private List<Quaternion> rotation = new List<Quaternion>();
    //private List<Vector3> rotation = new List<Vector3>();
    private List<Vector3> distances = new List<Vector3>(); 
    private List<Matrix4x4> matrices = new List<Matrix4x4>();
    
    string[] name_list = { "bowl", "coffee", "lego", "cracker box", "spam can"};

    private int vox_write_counter = 0;
    string parentNewName;
    int envNumber;

    private float threshold;

    private float epsilon;

    //private string writePath = "C:/Users/User/AppData/Roaming/Microsoft/Windows/Network Shortcuts/bml (143.248.32.61)/RobotArmData/";
    //string writePath = "//143.248.32.61/bml/RobotArmData/";
    //string writePath = "D:/Github_/robot_hand_data/RobotTrainingVARData/";
    //readonly string writePath = "D:\\Github_\\robot_hand_data\\VoxelData\\";
    readonly string writePath = ".\\data\\VoxelData\\";
    // Start is called before the first frame update
    public void VoxelInfoOnStart()
    {
        envNumber = UnityEngine.Random.Range(0, 50);

        threshold = 0.005f;
        epsilon = 1e-8f;
        //grabbing original voxel data text files from desktop
        folderPath = ".\\Assets\\Init_Voxel_Data\\";
        pgo = transform.parent.gameObject;
        parentNewName = Regex.Replace(pgo.name, "[^0-9.]", "");
        
        Regex regex = new Regex(@"[-+]?[0-9]+ *\.?[0-9]+");

        //add orig voxel data from text files to 'entries' if object has been spawned
        foreach (Transform child in pgo.transform)
        {
            //print("Is Object??");
            if (child.gameObject.name.Equals("ground"))
            {
                ground_Object = child.gameObject;
            }
            if (child.gameObject.tag == "Item")
            {
                //print("Yes!");
                goName = child.gameObject.name;

                int index1 = goName.IndexOf("(Clone)");
                goNameNew = goName.Remove(index1);
                for (int i = 0; i < name_list.Length; i++)
                {
                    //print("goNameNew : " + goNameNew);
                    //print("Name : " + name);
                    if (goNameNew == name_list[i])
                    {
                        string filename = folderPath + name_list[i] + ".txt";
                        theReader = new StreamReader(filename);
                        using (theReader)
                        {
                            
                            voxelLines = theReader.ReadToEnd();

                            //print("Reading File : " + filename);
                            if (voxelLines != null)
                            {
                                //print("Not Null");
                                MatchCollection digits = regex.Matches(voxelLines);
                                
                                Vector3[] transformVal_arr = new Vector3[digits.Count/3];
                                int[] voxelval_arr = new int[digits.Count / 3];
                                //print(digits.Count);
                                for (int idx = 0, vec_idx = 0; idx < digits.Count; idx=idx+3, vec_idx++)
                                {
                                    transformVal_arr[vec_idx] = new Vector3(float.Parse(digits[idx].Value), float.Parse(digits[idx+1].Value), float.Parse(digits[idx + 2].Value));
                                    voxelval_arr[vec_idx] = 1;
                                }

                                transformVals.Add(transformVal_arr);
                                voxelVals.Add(voxelval_arr);


                            }
                            
                            // Done reading, close the reader and return true to broadcast success    
                            theReader.Close();
                            
                        }

                        string filename_trans = folderPath + name_list[i] + "_transform.txt";
                        theReader = new StreamReader(filename_trans);
                        using (theReader)
                        {
                            string voxelLine1 = theReader.ReadLine();

                            MatchCollection digits = regex.Matches(voxelLine1);

                            init_pos.Add(new Vector3(float.Parse(digits[0].Value), float.Parse(digits[1].Value), float.Parse(digits[2].Value)));

                            string voxelLine2 = theReader.ReadLine();
                            MatchCollection digits2 = regex.Matches(voxelLine2);
                            init_rot.Add(new Vector3(float.Parse(digits2[0].Value), float.Parse(digits2[1].Value), float.Parse(digits2[2].Value)));
                            init_name.Add(name_list[i]);
                            
                            theReader.Close();
                        }

                        break;
                    }
                }
            }
        } 
    }
    

    public void VoxelGen(List<GameObject> Obj_generated, List<Vector3[]> transformValsRotated, List<int> currentObjectTypes, List<float> objectVisibility, bool collect_data)
    {
        if (collect_data)
        {
            
            //voxelValues passes voxel 0/1 data to ActionDataComp everytime VoxelGen is called
            
            int counter = 0;
            int goInstanceID = 0;
            List<int> gameobjectIDs = new List<int>();
            List<Vector3> childLocalPositions = new List<Vector3>();
            List<Vector3> eulAngs = new List<Vector3>();

            List<Vector3> initialPositions = new List<Vector3>();
            List<int> transform_val_idx = new List<int>();



            //for each spawned object, collect its rotation and translation 
            foreach (GameObject child in Obj_generated)
            {

                if (child.tag == "Item")
                {
                    if (ground_Object.GetComponent<Ground_SC>().Is_grounded(child) || child.transform.localPosition.y < -0.2)
                        continue;

                    goInstanceID = child.GetInstanceID();
                    gameobjectIDs.Add(goInstanceID);
                    childLocalPositions.Add(child.transform.localPosition);
                    eulAngs.Add(child.transform.eulerAngles);

                    goName = child.name;
                                        

                    tempindex1 = goName.IndexOf("(Clone)");
                    tempGoNameNew = goName.Remove(tempindex1);
                    currentObjectTypes.Add(Array.IndexOf(name_list, tempGoNameNew) + 1);
                    int name_count = init_name.Count;

                    for (int ii = 0; ii < name_count; ii++)
                    {
                        //print("tempGoNameNew : " + tempGoNameNew);
                        //print("init_name     : " + init_name[ii]);
                        if (init_name[ii].Equals(tempGoNameNew))
                        {
                            transform_val_idx.Add(ii);
                            initialPositions.Add(init_pos[ii]);
                            //Vector3 rotationVector = new Vector3(eulAngs[counter].x - init_rot[ii].x, eulAngs[counter].y - init_rot[ii].y, eulAngs[counter].z - init_rot[ii].z);
                            Vector3 rotationVector = new Vector3(eulAngs[counter].x - init_rot[ii].x, eulAngs[counter].y - init_rot[ii].y, eulAngs[counter].z - init_rot[ii].z);
                            //print(eulAngs[counter].ToString("F3"));

                            rotation.Add(Quaternion.Euler(rotationVector));
                            //rotation.Add(rotationVector);
                            counter++;
                            break;

                            /*
                            if (counter < generated.Count)
                            {
                                distances.Add(child.transform.localPosition - init_pos[ii]);

                                Vector3 rotationVector = new Vector3(eulAngs[counter].x - init_rot[ii].x, eulAngs[counter].y - init_rot[ii].y, eulAngs[counter].z - init_rot[ii].z);
                                //Vector3 rotationVector = new Vector3(10, 10, 10);
                                rotation.Add(Quaternion.Euler(rotationVector));
                                //print(rotation[counter]);
                                counter++;
                            }
                            else
                            {
                                counter = 0;
                            }
                            */

                        }
                    }
                }
            }


            for (int i = 0; i < transform_val_idx.Count; i++)   // initial pos(metric)
            {
                transformValsRotated.Add(new Vector3[transformVals[transform_val_idx[i]].Length]);
            }
            //create rotation matrices for each spawned object 
            for (int i = 0; i < rotation.Count; i++)
            {
                matrices.Add(Matrix4x4.Rotate(rotation[i]));
                //matrices.Add(Rotate(rotation[i]));
                //print(rotation[i]);
                //print(matrices[i]);
            }
            //rotate and translate orig voxel data  
            for (int i = 0; i < transformValsRotated.Count; i++)
            {
                for (int j = 0; j < transformValsRotated[i].Length; j++)
                {
                    //move to zero point
                    try { 
                        transformValsRotated[i][j] = transformVals[transform_val_idx[i]][j] - initialPositions[i];
                    }
                    catch
                    {
                        print("Hello");
                    }
                    Vector3 globalVoxelPos = pgo.transform.TransformPoint(transformValsRotated[i][j]);

                    //rotate
                    transformValsRotated[i][j] = matrices[i].MultiplyPoint(transformValsRotated[i][j]);
                    globalVoxelPos = pgo.transform.TransformPoint(transformValsRotated[i][j]);

                    //then back to correct place
                    transformValsRotated[i][j] = transformValsRotated[i][j] + childLocalPositions[i];
                }
            }

            /*
            foreach (Vector3[] voxel_set in transformValsRotated)
            {
                foreach(Vector3 point in voxel_set)
                {
                    Debug.DrawLine(pgo.transform.position + point, pgo.transform.position + point + new Vector3(0.05f, 0.05f, 0.05f));
                }
            }*/
            // calculate whether each object on the table is visible or not using raycasts
            CalculateObjectVisibility(Obj_generated, childLocalPositions, gameobjectIDs, transformValsRotated, objectVisibility);
            
                      

            distances.Clear();
            matrices.Clear();
            rotation.Clear();
            

            /*
            //print("div: " + div); 
            if (voxelTransform.Count % 10 == 0)    
            {
                
                List<string> linesToWriteTotal = new List<string>();
                for (int i = 0; i < voxelTransform.Count; i++)         /// for every episode (total voxel matrix)
                {
                    //int idx = voxelValues.Count / voxelVals.Count;
                    for (int j = 0; j < voxelTransform[i].Count; j++)    /// for every voxel
                    {
                        // val - the type of object (i.e cup, box, can...)
                        //StringBuilder lineTotal = new StringBuilder();
                        //lineTotal.Append("" + voxelTransform[i][j].ToString("F5")).Append(" val:" + ((i % voxelVals.Count + 1) * voxelValues[i][j])).Append(";");
                        //lineTotal.Append("" + voxelTransform[i][j].ToString("F5")).Append(" val:" + objectTypes[i]).Append(";");
                        
                        for (int kk = 0; kk < voxelTransform[i][j].Length; kk++)
                        { 
                            String lineTotal = voxelTransform[i][j][kk].ToString("F5") + " val:" + objectTypes[i][j] + " vis:" + objectVisibility[i][j];
                            linesToWriteTotal.Add(lineTotal.ToString());
                        }
                    }
                    linesToWriteTotal.Add("-----");

                }

                //System.IO.File.WriteAllLines("C:/Users/User/AppData/Roaming/Microsoft/Windows/Network Shortcuts/bml (143.248.32.61)/temp/RoboticArmData/ActionDataComp_VOX-env" + parentNewName + "_action" + vox_write_counter + ".txt", linesToWriteTotal.ToArray());
                //System.IO.File.WriteAllLines("C:/Users/User/Desktop/bml (143.248.32.61)/ActionDataComp_VOX-env" + parentNewName + "_action" + vox_write_counter + ".txt", linesToWriteTotal.ToArray());
                System.IO.File.WriteAllLines(writePath + "ActionDataComp_VOX-env" + parentNewName + "_action" + vox_write_counter + ".txt", linesToWriteTotal.ToArray());

                vox_write_counter++;
                //print("voxwritecounter" + parentNewName + ": " + vox_write_counter);
                
                objectVisibility.Clear();
                objectTypes.Clear();
                voxelTransform.Clear();
                
            }
            */


            //print("time: " + Time.realtimeSinceStartup);

        }
    }

    void CalculateObjectVisibility(List<GameObject> generated, List<Vector3> childLocalPositions, List<int> gameobjectIDs, List<Vector3[]> voxelTransform, List<float> ObjectVisible_degree)
    {
        //for each object 
        //raycast to lots of points to check which can be seen by the camera 
        //and from that data calculate how 'visible' it is
        Vector3 cameraPos = agentCamera.transform.localPosition;
        

        // for each object raycast to its points 
        for (int i = 0; i < voxelTransform.Count; i++)
        {
            int visible = 0;
            int invisible = 0;
            int raycastMisses = 0;
            Vector3 actualPosition = Vector3.zero;
            Vector3 actualRotation = Vector3.zero;

            //raycast every 2nd point
            for (int j = 0; j < voxelTransform[i].Length; j ++)
            {
                //if (!generated[i].transform.GetChild(0).GetComponent<Collider>().bounds.Contains(voxelTransform[i][j]))
                //  continue;
                
                //check first if point even in camera view - if it isn't, it's invisible
                Vector3 globalVoxelPos = pgo.transform.TransformPoint(voxelTransform[i][j]);
                Vector3 viewPos = agentCamera.WorldToViewportPoint(globalVoxelPos);
                //if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 && viewPos.z > 0)
                //{
                    // Ray ray = agentCamera.ViewportPointToRay(viewPos);
                    Ray ray = new Ray(agentCamera.transform.position, globalVoxelPos - agentCamera.transform.position);
                    //Ray ray = new Ray(agentCamera.transform.position, globalVoxelPos);
                    Vector3 rayOrigin = ray.origin;
                    Vector3 rayDirection = ray.direction;

                    RaycastHit hit;


                    //Debug.DrawLine(rayOrigin, rayOrigin + 100.0f * rayDirection, Color.green, 100.0f);


                    if (Physics.Raycast(ray, out hit, 100f, layerMask))
                    {
                        int hitInstanceID = hit.collider.transform.parent.gameObject.GetInstanceID();
                        if (hitInstanceID == gameobjectIDs[i])
                        {
                            visible++;
                            actualPosition = hit.collider.gameObject.transform.localPosition;
                            actualRotation = hit.collider.gameObject.transform.eulerAngles;

                        }
                        else
                        {
                            invisible++;
                        }
                    }
                    else
                    {
                        //Debug.DrawLine(rayOrigin, rayOrigin + 100.0f * rayDirection, Color.red, 100.0f);
                        raycastMisses++;
                    }
                //}
               
               
            }

            //int total = visibile + invisible + raycastMisses;
            int total = visible + invisible;
            if (total == 0)
            {
                print("no raycast hits");
                print(raycastMisses);
                ObjectVisible_degree.Add(0);
            }
            else 
            {
                if (raycastMisses > 50)
                {
                    //print("more than 50 points missed - miss count: " + raycastMisses + ", object name: " + generated[i].name + ", position: " + generated[i].transform.localPosition.ToString("F2"));
                    //print("more than 50 points missed, object name: " + generated[i].name + ", position: " + generated[i].transform.localPosition.ToString("F2"));
                    //Debug.Break();
                }
                else
                {
                    // print("Okay~");
                }

                float visibility = (float)visible / (float)total;

                ObjectVisible_degree.Add(visibility);

                
            }

        }

    }

}

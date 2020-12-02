using System.Collections;
using System.Collections.Generic;
using UnityEngine;  
using System; 
using System.Text;

public class ObjectVoxelDetection : MonoBehaviour
{
    public GameObject objectBound;
    private Transform pgo;
    private List<Transform> gos = new List<Transform>();

    private int scale_x = 48;
    private int scale_y = 12;
    private int scale_z = 60;

    private float interval = -1; 
    private int[,,] amn;
    private string childName;
    private int tempindex;  

    // Start is called before the first frame update 
    void Start()
    {
        /**cube gameObject in Environment2Voxel is used to create an x,y,z boundary
         * a 1 is added everytime an object's collider is detected within the boundary 
         * all object locations with 1's are written to a text file 
         * */
        pgo = transform.parent;
        Vector3 cubePos = new Vector3(objectBound.transform.position.x, objectBound.transform.position.y, objectBound.transform.position.z);
        Vector3 cubePosLoc = new Vector3(objectBound.transform.localPosition.x, objectBound.transform.localPosition.y, objectBound.transform.localPosition.z);
        //print(cubePosLoc);
        float xMin = (cubePos[0] - (objectBound.transform.lossyScale.x / 2));
        float xMax = (cubePos[0] + (objectBound.transform.lossyScale.x / 2));
        float yMin = (cubePos[1] - (objectBound.transform.lossyScale.y / 2));
        float yMax = (cubePos[1] + (objectBound.transform.lossyScale.y / 2));
        float zMin = (cubePos[2] - (objectBound.transform.lossyScale.z / 2));
        float zMax = (cubePos[2] + (objectBound.transform.lossyScale.z / 2));
        float xMinLoc = (cubePosLoc[0] - (objectBound.transform.localScale.x / 2));
        float xMaxLoc = (cubePosLoc[0] + (objectBound.transform.localScale.x / 2));
        float yMinLoc = (cubePosLoc[1] - (objectBound.transform.localScale.y / 2));
        float yMaxLoc = (cubePosLoc[1] + (objectBound.transform.localScale.y / 2));
        float zMinLoc = (cubePosLoc[2] - (objectBound.transform.localScale.z / 2));
        float zMaxLoc = (cubePosLoc[2] + (objectBound.transform.localScale.z / 2));

        //assuming here xTwo is always bigger
        int xLen = scale_x;
        int yLen = scale_y;
        int zLen = scale_z;

        int xLenLoc = scale_x;
        int yLenLoc = scale_y;
        int zLenLoc = scale_z;

        float interval_x = (xMax - xMin) / scale_x;
        float interval_y = (yMax - yMin) / scale_y;
        float interval_z = (zMax - zMin) / scale_z;


        int[,,] amn = new int[xLenLoc, yLenLoc, zLenLoc];
        //StartCoroutine(Example());
        MeshCollider col = new MeshCollider();
        GameObject target_child = new GameObject();

        foreach (Transform child in pgo)
        {
            if (child.gameObject.tag == "Item" && child.gameObject.activeInHierarchy)
            {
                childName = child.name;
                target_child = child.gameObject;
                col = child.GetChild(0).GetComponent<MeshCollider>();
                break;
            }
        }

        List<string> linesToWrite = new List<string>();
        for (int i = 0; i < xLen; i++)
            for (int j = 0; j < yLen; j++)
                for (int k = 0; k < zLen; k++)
                {
                    StringBuilder line = new StringBuilder();
                    Vector3 target_pos = new Vector3(xMin + (interval_x * i), yMin + (interval_y * j), zMin + (interval_z * k));
                    Vector3 target_pos_loc = new Vector3(xMinLoc + (interval_x * i), yMinLoc + (interval_y * j), zMinLoc + (interval_z * k));
                    if (col.bounds.Contains(target_pos))
                    {
                        amn[i, j, k] = 1;
                        line.Append("" + target_pos_loc.ToString("F4")).Append(" val:" + amn[i, j, k]).Append(";");
                        linesToWrite.Add(line.ToString());
                    }
                    
                }

        //D:\\Github_\\robot_hand_data\\VoxelData\\
        System.IO.File.WriteAllLines("D:\\Github_\\robot_hand_data\\Init_Voxel_Data\\" + childName + ".txt", linesToWrite.ToArray());

        List<string> linesToWrite2 = new List<string>();

        StringBuilder line_transform = new StringBuilder();

        line_transform.Append("" + target_child.transform.localPosition.ToString("F4"));
        linesToWrite2.Add(line_transform.ToString());
        line_transform.Clear();
        
        line_transform.Append("" + target_child.transform.eulerAngles.ToString("F4"));
        linesToWrite2.Add(line_transform.ToString());
                          
        System.IO.File.WriteAllLines("D:\\Github_\\robot_hand_data\\Init_Voxel_Data\\" + childName + "_transform.txt", linesToWrite2.ToArray());
        print("time: " + Time.realtimeSinceStartup);

        
    }

    // Update is called once per frame
    void Update()
    {

    }
     
}
     
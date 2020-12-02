using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class robotic_arm : MonoBehaviour
{


    /*
    private HingeJoint base_to_part0;
    private HingeJoint part0_to_part1;
    private HingeJoint part1_to_part2;
    private HingeJoint part2_to_part3;
    private HingeJoint part3_to_Rgrip;
    private HingeJoint part3_to_Lgrip;
    */

    private List<HingeJoint> hinge_joints = new List<HingeJoint>();

    private GameObject Parent_Object;

    private GameObject Item_Object;

    private List<string> joint_name_list = new List<string>();

    private List<float> force_ratio = new List<float>();


    // Use this for initialization
    void Start()
    {
        string[] name_list = { "Base", "part1", "part2", "part3", "leftGrip", "RigthGrip" };
        float[] force_rate_arr = {40,60,60,40,40,40};

        Parent_Object = this.transform.parent.gameObject;
        foreach (Transform child in transform)
        {
            for (int i = 0; i < 5; i++)
            {
                string object_name = name_list[i];
                
                if (child.gameObject.name.Equals(object_name))
                {
                    hinge_joints.Add(child.gameObject.GetComponent(typeof(HingeJoint)) as HingeJoint);
                    joint_name_list.Add(object_name);
                    break;
                }
            }
        }

        foreach(Transform child in transform.parent)
        {
            if (child.gameObject.name.Equals("Item"))
            {
                Item_Object = child.gameObject;
            }
        }

        for (int i = 0; i < hinge_joints.Count; i++)
        {
            HingeJoint joint = hinge_joints[i];

            if (joint_name_list[i] == "leftGrip")
            {
                JointMotor motor = joint.motor;

                motor.force = -40;
                motor.targetVelocity = 80;
                motor.freeSpin = false;

                joint.motor = motor;
                joint.useMotor = true;
                //motor.targetVelocity = 0;
                //motor.force = 0;
                //Parent_Object.GetComponent<SpawnObject>().ActivateEarthquakeAuto();
            }

        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        //Item_Object.GetComponent<SpawnObject>().ActivateEarthquakeAuto();
        //FindObjectOfType<SpawnObject>().ActivateEarthquakeAuto();
        //foreach (HingeJoint joint in hinge_joints)

        /*
        for(int i =0; i< hinge_joints.Count; i++)
        {
            HingeJoint joint = hinge_joints[i];

            if(joint_name_list[i] == "Base")
            { 
                JointMotor motor = joint.motor;

                motor.force = 20;
                motor.targetVelocity = 80;
                motor.freeSpin = false;

                joint.motor = motor;
                joint.useMotor = true;
                //motor.targetVelocity = 0;
                //motor.force = 0;
                //Parent_Object.GetComponent<SpawnObject>().ActivateEarthquakeAuto();
            }

        }*/
    }
}

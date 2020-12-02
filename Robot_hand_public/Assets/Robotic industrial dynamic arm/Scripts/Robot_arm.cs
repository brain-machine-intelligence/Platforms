using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Robotic_arm : MonoBehaviour
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

    // Use this for initialization
    void Start()
    {
        string[] name_list = { "Base", "part1", "part2", "part3", "leftGrip", "RigthGrip" };

        foreach (Transform child in transform)
        {
            for (int i = 0; i < 5; i++)
            {
                string object_name = name_list[i];
                print(child.gameObject.name);
                if (child.gameObject.name.Equals(object_name))
                {
                    hinge_joints.Add(child.gameObject.GetComponent(typeof(HingeJoint)) as HingeJoint);
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        foreach (HingeJoint joint in hinge_joints)
        {
            var motor = joint.motor;

            joint.useMotor = true;

            // motor.targetVelocity = 100;
            //motor.force = 50;
        }
    }
}

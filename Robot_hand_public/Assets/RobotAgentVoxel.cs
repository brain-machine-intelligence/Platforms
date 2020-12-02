using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;


public class RobotAgentVoxel : Agent
{

    public struct transform_info
    {
        public string _obj_name;
        public float _position_x;
        public float _position_y;
        public float _position_z;
        public float _rot_x;
        public float _rot_y;
        public float _rot_z;
        public float _rot_w;

        public transform_info(string name, float position_x, float position_y, float position_z, float rot_x, float rot_y, float rot_z, float rot_w)
        {
            this._obj_name = name;
            this._position_x = position_x;
            this._position_y = position_y;
            this._position_z = position_z;
            this._rot_x = rot_x;
            this._rot_y = rot_y;
            this._rot_z = rot_z;
            this._rot_w = rot_w;
        }

    }
    public bool terminate_flag = false;
    private List<transform_info> initial_transform_list = new List<transform_info>();
    private List<HingeJoint> hinge_joints = new List<HingeJoint>();
    private List<float> hinge_rate = new List<float>();

    private GameObject Parent_Object;

    private List<Transform> robot_component_list = new List<Transform>();

    private int action_count = 0;
    private Camera i_camera;
    public bool useVectorObs;
    RayPerception rayPer;
    private GameObject Item_Object;
    private GameObject ground_Object;

    private bool doing_action = false;

    private const int action_num = 4;
    private int[] joint_idx = new int[action_num];

    Rigidbody rBody;
    // Start is called before the first frame update
    public override void InitializeAgent()
    {
        base.InitializeAgent();
        rBody = GetComponent<Rigidbody>();
        string[] name_list = { "Base", "part1", "part2", "part3", "leftGrip", "RightGrip" };   // name_list for joint
        string[] add_name_list = { "part0", "leftGrip", "RightGrip" };  // doesn't has hinge_joint
        float[] force_rate_arr = { 240, 320, 320, 240, 240, 240 };
        Parent_Object = this.transform.parent.gameObject;
        int hinge_idx = 0;

        foreach (Transform child in transform)
        {

            for (int i = 0; i < 6; i++)
            {
                string object_name = name_list[i];

                if (child.gameObject.name.Equals(object_name))
                {

                    if (object_name.Equals(add_name_list[1]) || object_name.Equals(add_name_list[2]))
                    {

                        Transform t = child.gameObject.transform;
                        transform_info t_info = new transform_info(object_name, t.localPosition.x, t.localPosition.y, t.localPosition.z, t.localRotation.x, t.localRotation.y, t.localRotation.z, t.localRotation.w);

                        robot_component_list.Add(child);
                        initial_transform_list.Add(t_info);
                        break;


                    }
                    else
                    {
                        Transform t = child.gameObject.transform;
                        transform_info t_info = new transform_info(object_name, t.localPosition.x, t.localPosition.y, t.localPosition.z, t.localRotation.x, t.localRotation.y, t.localRotation.z, t.localRotation.w);

                        robot_component_list.Add(child);
                        initial_transform_list.Add(t_info);
                        hinge_joints.Add(child.gameObject.GetComponent(typeof(HingeJoint)) as HingeJoint);
                        hinge_rate.Add(force_rate_arr[i]);
                        joint_idx[i] = hinge_idx;
                        hinge_idx += 1;
                        break;
                    }
                }
                else if (child.gameObject.name.Equals(add_name_list[0]))
                {

                    Transform t = child.gameObject.transform;
                    transform_info t_info = new transform_info(object_name, t.localPosition.x, t.localPosition.y, t.localPosition.z, t.localRotation.x, t.localRotation.y, t.localRotation.z, t.localRotation.w);

                    robot_component_list.Add(child);
                    initial_transform_list.Add(t_info);
                    break;

                }
            }
        }

        foreach (Transform child in transform.parent)
        {
            if (child.gameObject.name.Equals("Item"))
            {
                Item_Object = child.gameObject;
            }
            else if (child.gameObject.name.Equals("ground"))
            {
                ground_Object = child.gameObject;
            }

        }

        Item_Object.GetComponent<ObjectVoxelDetection>();
    }


    public override void CollectObservations()  // when to end
    {
        if (useVectorObs)
        {

            AddVectorObs(GetStepCount() / (float)agentParameters.maxStep);

            foreach (Transform component in robot_component_list)
            {
                AddVectorObs(component.localPosition);
                AddVectorObs(component.localRotation);

            }
            //AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));

        }

    }


    private void FixedUpdate()
    { 
        //if (!doing_action)
        //{

        //    if (action_count == 0 && !Item_Object.GetComponent<SpawnObject>().get_fixed_check())
        //    {
        //        // print("Am I stuck??");
        //        // print(doing_action);
        //        // print(Item_Object.GetComponent<SpawnObject>().get_spawn_finish());
        //        Item_Object.GetComponent<SpawnObject>().Do_check();
        //        return;
        //    }


        //    if (action_count > 5)
        //    {
        //        doing_action = true;
        //        int reward = ground_Object.GetComponent<Ground_SC>().Get_count();
        //        Item_Object.GetComponent<SpawnObject>().Reset();
        //        ground_Object.GetComponent<Ground_SC>().Reset();

        //        //float reward = Item_Object.GetComponent<SpawnObject>().Evaluate_Action(); 
        //        Done();
        //        SetReward(reward * 0.1f);
        //        print(reward);

        //    }
        //    else
        //    {
        //        if (action_count == 0)
        //        {
        //            ground_Object.GetComponent<Ground_SC>().Reset();
        //        }
        //        action_count += 1;
        //        doing_action = true;
        //        Item_Object.GetComponent<SpawnObject>().Estimate_voxel();
        //        RequestDecision();

        //    }
        //}

    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //StartCoroutine(Doing_Action(4f, vectorAction));
    }


    IEnumerator Doing_Action(float sec, float[] vectorAction)
    {

        for (int i = 0; i < action_num; i++)
        {
            float force_val = (vectorAction[i] - 0.5f) * 2f;
            //float target_val = vectorAction[2 * i + 1];

            JointMotor motor = hinge_joints[joint_idx[i]].motor;


            motor.force = Math.Abs(force_val) * hinge_rate[joint_idx[i]] + 0.1f * hinge_rate[joint_idx[i]];
            if (force_val >= 0)
                motor.targetVelocity = 100;
            else
                motor.targetVelocity = -100;

            motor.freeSpin = false;

            hinge_joints[joint_idx[i]].motor = motor;
            hinge_joints[joint_idx[i]].useMotor = true;

        }

        yield return new WaitForSeconds(sec);

        SetReward(0.0f);

        doing_action = false;

        foreach (HingeJoint joint in hinge_joints)
        {
            JointMotor motor = joint.motor;

            motor.force = 0;
            motor.targetVelocity = 0;
            motor.freeSpin = false;

            joint.motor = motor;
            joint.useMotor = false;
        }





    }

    public override void AgentOnDone()
    {
        print("Done?");

    }

    IEnumerator Earth_Quake_n_Evaluate(float sec)
    {

        Item_Object.GetComponent<SpawnObject>().ActivateEarthquakeAuto();
        yield return new WaitForSeconds(sec);

        //float reward = Item_Object.GetComponent<SpawnObject>().Evaluate_Action();
        int reward = ground_Object.GetComponent<Ground_SC>().Get_count();
        Item_Object.GetComponent<SpawnObject>().Reset();
        ground_Object.GetComponent<Ground_SC>().Reset();

        SetReward(-1f * reward);
        Done();

    }


    // Update is called once per frame
    public override void AgentReset()
    {
        for (int i = 0; i < 6; i++)   // initialize robot component
        {
            transform_info component_tr = initial_transform_list[i];

            robot_component_list[i].localPosition = new Vector3(component_tr._position_x, component_tr._position_y, component_tr._position_z);
            robot_component_list[i].localRotation = new Quaternion(component_tr._rot_x, component_tr._rot_y, component_tr._rot_z, component_tr._rot_w);

            //robot_component_list[i].SetPositionAndRotation(new Vector3(component_tr._position_x, component_tr._position_y, component_tr._position_z),
            //                                               new Quaternion(component_tr._rot_x, component_tr._rot_y, component_tr._rot_z, component_tr._rot_w)) ;
        }


        //Item_Object.GetComponent<SpawnObject>().Reset();

        doing_action = false;
        action_count = 0;

    }
}

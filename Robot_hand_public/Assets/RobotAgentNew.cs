using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using UnityEditor;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
//using System.Management;

public class RobotAgentNew : Agent
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
    public bool action_set = false;
    public bool grab_key_down = false;

    private List<transform_info> initial_transform_list = new List<transform_info>();
    private List<HingeJoint> hinge_joints = new List<HingeJoint>();
    

    private GameObject Parent_Object;

    private List<Transform> robot_component_list = new List<Transform>();
    private int Reward_MAX = 6;
    private int action_count = 0;

    private Camera i_camera;
    public bool useVectorObs;
    RayPerception rayPer;
    private GameObject Item_Object;
    private GameObject ground_Object;
    private float[] MaxArmVelocity = { 10.0f, 30.0f, 30.0f, 30.0f };
    private float MaxTime = 4f;

    private bool doing_action = false;

    private const int action_num = 4;
    private int[] joint_idx = new int[action_num];

    private double prev_kl_val = Double.PositiveInfinity;

    Rigidbody rBody;
    private List<float[]> motorForce = new List<float[]>();
    private MultiDimDictList<string, float[]> motorForceVel = new MultiDimDictList<string, float[]>();
    private List<MultiDimDictList<string, float[]>> actionForceVel = new List<MultiDimDictList<string, float[]>>();

    private List<List<Vector3[]>> voxelTransform = new List<List<Vector3[]>>();
    private List<List<int>> objectTypes = new List<List<int>>();
    private List<List<float>> objectVisiblity = new List<List<float>>();


    private List<int[]> voxelValues = new List<int[]>();
    private List<int> cmlRewards = new List<int>();

    private List<Vector3[]> jointPositions = new List<Vector3[]>();
    private List<Vector3[]> jointRotations = new List<Vector3[]>();

    private List<AllData> ActionDataComp = new List<AllData>();
    private AllData actVoxRew;
    private StreamReader theReader;
    string parentNewName;
    private float randProb;
    //private float threshold;
    private const float threshold_value = 0.05f;
    private List<float> threshold_list = new List<float>();
    private List<float> reward_count_list = new List<float>();

    //private float[] threshold = new float[Reward_MAX];
    //private float[] reward_count = new float[Reward_MAX];

    private float epsilon;
    private bool collect_data = false;
    private int trials_collected = 0;
    private int prev_rew = -1;
    private int cur_rew = -1;
    private bool trials_completed = true;
    private float curr_prob;


    public GameObject GrabPoint;

    private GameObject TargetObject; // TargetObject는 로봇 손과 가장 가까운 물체, GrabPoint는 잡힌 물체가 붙어있을 포인트

    private bool IsGrab = false; // 물체를 잡았는지 확인하는 값


    private float AttractSpeed = 10.0f; // 물체를 잡았을때 물체가 따라가는 속도

    private float GrabDistance = 0.9f; // 물체 잡기를 허용하는 거리
    

    //private string writePath = "C:/Users/User/AppData/Roaming/Microsoft/Windows/Network Shortcuts/bml (143.248.32.61)/RobotArmData/";
    //string writePath = "//143.248.32.61/bml/RobotArmData ";
    //string writePath = "D:/Github_/robot_hand_data/RobotTrainingVARData/";
    string writePath = ".\\data\\data12\\RobotAgentData\\";

    int pathCounter = 0;

    int act_rw_count = 0;

    // Start is called before the first frame update
    public override void InitializeAgent()
    {
        base.InitializeAgent();

        rBody = GetComponent<Rigidbody>();
        string[] name_list = { "Base", "part1", "part2", "part3", "leftGrip", "RightGrip" };   // name_list for joint
        string[] add_name_list = { "part0", "leftGrip", "RightGrip" };  // doesn't has hinge_joint
        Parent_Object = this.transform.parent.gameObject;
        parentNewName = Regex.Replace(Parent_Object.name, "[^0-9.]", "");
        epsilon = 1e-4f;

        
        for (int i = 0; i <= Reward_MAX; i++)
        {
            reward_count_list.Add(0.0f);
            threshold_list.Add(threshold_value);
           
        }

        //below method works, its just REALLY slow
        //DirectoryInfo driveInfo = new DirectoryInfo("//143.248.32.61/bml/RobotArmData/");
        //FileInfo[] files = driveInfo.GetFiles("*.*", SearchOption.AllDirectories);
        ////solution to storage problem but very very time consuming 
        //long totalByteSize = files.Sum(f => f.Length);
        //print(totalByteSize);
        ////bigger than 75 gb
        //if (totalByteSize > 80530636800)
        //{
        //    pathCounter++; 
        //    bool exists = System.IO.Directory.Exists(writePath + pathCounter);

        //    if (!exists)
        //    { 
        //        System.IO.Directory.CreateDirectory(writePath + pathCounter);
        //        writePath = writePath + pathCounter;
        //        print("newPath: " + writePath);
        //    } 

        //} 
        //randProb = UnityEngine.Random.Range(0.00f, 1.00f);
        //print(randProb);
        //curr_prob = randProb;  

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
            AddVectorObs(new Vector3((float)action_count / 15.0f, (float)action_count / 15.0f, (float)action_count / 15.0f));

            foreach (Transform component in robot_component_list)
            {
                AddVectorObs(component.localPosition);
                AddVectorObs(component.localRotation);
            }


        }

    }


    private void TargetSet() // 로봇 손으로부터 가장 가까운 물체를 계산하고 지정하는 함수
    {
        float distanceToCloestItem = Mathf.Infinity;
        GameObject closestItem = null;
        GameObject[] allItems = GameObject.FindGameObjectsWithTag("Item");

        foreach (GameObject currentItem in allItems)
        {
            float distanceToItem = (currentItem.transform.position - GrabPoint.transform.position).sqrMagnitude;
            if (distanceToItem < distanceToCloestItem)
            {
                distanceToCloestItem = distanceToItem;
                closestItem = currentItem;
            }
        }
        // Debug.DrawLine(GrabPoint.transform.position, closestItem.transform.position);
        if (IsGrab == false)
        {
            TargetObject = closestItem;
        }
    }

    void GrabObject() // 물체를 잡는 함수
    {

        if (Vector3.Distance(GrabPoint.transform.position, TargetObject.transform.position) < GrabDistance)
        {
            if (grab_key_down)
            {
                IsGrab = !IsGrab;
                grab_key_down = false;
            }
            if (IsGrab == true)
            {
                TargetObject.transform.position = Vector3.MoveTowards(TargetObject.transform.position, GrabPoint.transform.position, AttractSpeed * Time.deltaTime);
                TargetObject.GetComponent<Rigidbody>().useGravity = false;
                TargetObject.GetComponent<Rigidbody>().freezeRotation = true;
                TargetObject.transform.rotation = GrabPoint.transform.rotation;
            }
            if (IsGrab == false)
            {
                TargetObject.GetComponent<Rigidbody>().useGravity = true;
                TargetObject.GetComponent<Rigidbody>().freezeRotation = false;
            }

        }

    }


    private void FixedUpdate()
    {
        TargetSet();
        GrabObject();
        if (!doing_action)
        {
            if (action_count == 0 && !Item_Object.GetComponent<SpawnObject>().get_fixed_check())
            {
                // print("Am I stuck??");
                // print(doing_action);
                // print(Item_Object.GetComponent<SpawnObject>().get_spawn_finish());
                Item_Object.GetComponent<SpawnObject>().Do_check();
                return;
            }

            if (action_count == 0)
            {
                ground_Object.GetComponent<Ground_SC>().Reset();
            }


            if (action_count >= 11)
            {
                doing_action = true;
                int reward = ground_Object.GetComponent<Ground_SC>().Get_count() + 1;
                Item_Object.GetComponent<SpawnObject>().Reset();
                ground_Object.GetComponent<Ground_SC>().Reset();
                //float reward = Item_Object.GetComponent<SpawnObject>().Evaluate_Action(); 
                Done();
                SetReward(reward);
                //print(reward);

            }
            else
            {
                
                action_count += 1;
                actVoxRew = new AllData();

                randProb = UnityEngine.Random.Range(0.0f, 1.0f);
                cur_rew = ground_Object.GetComponent<Ground_SC>().Get_count() + 1 - prev_rew;
                prev_rew = ground_Object.GetComponent<Ground_SC>().Get_count() + 1;

                if (cur_rew > Reward_MAX)
                {
                    
                    for(int iii= Reward_MAX; iii <= cur_rew; iii++)
                    {
                        reward_count_list.Add(0.0f);
                        threshold_list.Add(threshold_value);
                    }
                                      
                    Reward_MAX = cur_rew;
                }
              


                float sum_val = reward_count_list.Sum() + 1;

                double[] reward_distb = new double[Reward_MAX+1];
                double kl_val = .0f;
                for (int c = 0; c <= Reward_MAX; c++)
                {
                    if (c == cur_rew)
                        reward_distb[c] = (reward_count_list[c] + 1) / (sum_val);
                    else
                        reward_distb[c] = reward_count_list[c] / (sum_val);
                    kl_val += reward_distb[c] * Math.Log(reward_distb[c] * Reward_MAX + 0.0000001);

                }

                if (prev_kl_val > kl_val)
                {
                    prev_kl_val = kl_val;

                    reward_count_list[cur_rew] += 1.0f;
                    collect_data = true;

                }
                else if (randProb < threshold_list[cur_rew])
                {
                    prev_kl_val = kl_val;
                    reward_count_list[cur_rew] += 1.0f;
                    collect_data = true;
                }
                else

                {
                    collect_data = false;
                }



                //print(randProb);

                //print("entering coroutine");
                //print("collect_data: " + collect_data);
                doing_action = true;
                Item_Object.GetComponent<SpawnObject>().GetEulers(action_count, voxelTransform, objectTypes, objectVisiblity, collect_data);

                RequestDecision();
            }
        }

    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        StartCoroutine(Doing_Action(MaxTime, vectorAction));
    }


    IEnumerator Doing_Action(float sec, float[] vectorAction)
    {
        float[] actionMotorVelocities = new float[action_num];

        for (int i = 0; i < action_num; i++)
        {
            //float target_val = vectorAction[2 * i + 1];
            float action_val = vectorAction[i];

            JointMotor motor = hinge_joints[joint_idx[i]].motor;

            if (action_val >= 0)
                motor.targetVelocity = MaxArmVelocity[i];// * force_val;
            else
                motor.targetVelocity = (-1) * MaxArmVelocity[i];// * force_val;

            //motor.force = Math.Abs(force_val) * hinge_rate[joint_idx[i]] + 0.1f * hinge_rate[joint_idx[i]];
            //actionMotorForces[i] = motor.force;  
            //print(vectorAction[i]);

            actionMotorVelocities[i] = action_val;
            hinge_joints[joint_idx[i]].useMotor = true;
            hinge_joints[joint_idx[i]].motor = motor;

            yield return new WaitForSeconds(sec * Math.Abs(action_val));

            motor.targetVelocity = 0.0f;
            hinge_joints[joint_idx[i]].useMotor = false;

        }

        float grab_prob = vectorAction.Last();

        if (grab_prob > 0.5f)
        {
            grab_key_down = true;
        }

        yield return new WaitForSeconds(0.1f);

        grab_key_down = false;



        if (collect_data)
        {

            motorForceVel.Add("motor force", actionMotorVelocities);
            cmlRewards.Add(cur_rew);
            // prev_rew = tempReward;
            actVoxRew.ActionReward = cmlRewards;

            //record joint data
            Vector3[] currJointPos = new Vector3[robot_component_list.Count];
            Vector3[] currJointRot = new Vector3[robot_component_list.Count];
            for (int i = 0; i < robot_component_list.Count; i++)
            {
                currJointPos[i] = robot_component_list[i].localPosition;
                currJointRot[i] = robot_component_list[i].localEulerAngles;
            }

            jointPositions.Add(currJointPos);
            jointRotations.Add(currJointRot);

            ActionDataComp.Add(actVoxRew);
            //print(actVoxRew.ActionReward.Count); 
            if (actVoxRew.ActionReward.Count % 10 == 0)
            {
                //print("finally writing action"); 
                List<string> linesToWrite = new List<string>();
                for (int i = 0; i < motorForceVel["motor force"].Count; i++)
                {
                    for (int j = 0; j < motorForceVel["motor force"][i].Length; j++)
                    {
                        StringBuilder line2 = new StringBuilder();
                        line2.Append(string.Format(" ACTION MOTOR DATA {0}: {1},", i, motorForceVel["motor force"][i][j].ToString("F5")));
                        linesToWrite.Add(line2.ToString());
                    }

                    for (int k = 0; k < robot_component_list.Count; k++)
                    {
                        String line = "JOINT POSITION DATA " + i + " [" + k + "]: " + jointPositions[i][k] + ",";
                        linesToWrite.Add(line);

                        line = "JOINT ROTATION DATA " + i + "[" + k + "]: " + jointRotations[i][k] + ",";
                        linesToWrite.Add(line);
                    }

                    StringBuilder line3 = new StringBuilder();
                    line3.Append(" REWARD: " + actVoxRew.ActionReward[i]).Append(";");
                    linesToWrite.Add(line3.ToString());
                }
                //System.IO.File.WriteAllLines("C:/Users/User/AppData/Roaming/Microsoft/Windows/Network Shortcuts/bml (143.248.32.61)/temp/RoboticArmData/ActionDataComp_ACTRW-env" + parentNewName + "_action" + act_rw_count + ".txt", linesToWrite.ToArray());
                System.IO.File.WriteAllLines(writePath + "ActionDataComp_ACTRW-env" + parentNewName + "_action" + act_rw_count + ".txt", linesToWrite.ToArray());
                //System.IO.File.WriteAllLines("C:/Users/User/Desktop/bml (143.248.32.61)/ActionDataComp_ACTRW-env" + parentNewName + "_action" + act_rw_count + ".txt", linesToWrite.ToArray());
                //System.IO.File.WriteAllLines("\\143.248.32.61/bml/temp/RoboticArmData/ActionDataComp_ACTRW-env" + parentNewName + "_action" + act_rw_count + ".txt", linesToWrite.ToArray());
                //print("ACTS Written");

                act_rw_count++;
                motorForceVel.Clear();
                actionForceVel.Clear();
                jointPositions.Clear();
                jointRotations.Clear();
                actVoxRew.ActionReward.Clear();
                cmlRewards.Clear();

                //print(trials_collected); 
            }

        }

        doing_action = false;

        //randProb = curr_prob;
        //print("randProb: " + randProb); 
        SetReward(0.0f);


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
        IsGrab = false;
        action_count = 0;
        prev_rew = 0;
        cur_rew = 0;

    }


}

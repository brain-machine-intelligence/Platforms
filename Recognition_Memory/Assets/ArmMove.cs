using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArmMove : MonoBehaviour
{
    public GameObject joint1, joint2, joint3, joint4; // 로봇 팔 관절

    public GameObject Object1, Object2, Object3, Object4, Object5, Object6, Object7; // 테이블 위에 물체

    public GameObject TargetObject, GrabPoint; // TargetObject는 로봇 손과 가장 가까운 물체, GrabPoint는 잡힌 물체가 붙어있을 포인트

    public bool IsGrab; // 물체를 잡았는지 확인하는 값

    public int ArmVelocity; // 로봇 움직이는 속도

    public float AttractSpeed; // 물체를 잡았을때 물체가 따라가는 속도

    public float GrabDistance; // 물체 잡기를 허용하는 거리

    // Start is called before the first frame update
    void Start()
    {
        IsGrab = false;
    }

    // Update is called once per frame
    void Update()
    {
        inputKey(); // 조작 키에 대한 함수
        TargetSet(); // 로봇 손으로 부터 가장 가까운 물체를 계산하고 지정하는 함수
        GrabObject(); // 물체를 잡는 함수
        if (Input.GetKeyDown(KeyCode.R)) // 재시작
        {
            SceneManager.LoadScene(0);
        }
    }

    void TargetSet() // 로봇 손으로부터 가장 가까운 물체를 계산하고 지정하는 함수
    {
        float distanceToCloestItem = Mathf.Infinity;
        GameObject closestItem = null;
        GameObject[] allItems = GameObject.FindGameObjectsWithTag("Item");

        foreach (GameObject currentItem in allItems)
        {
            float distanceToItem = (currentItem.transform.position - GrabPoint.transform.position).sqrMagnitude;
            if(distanceToItem < distanceToCloestItem)
            {
                distanceToCloestItem = distanceToItem;
                closestItem = currentItem;
            }
        }
        Debug.DrawLine(GrabPoint.transform.position, closestItem.transform.position);
        if(IsGrab == false)
        {
            TargetObject = closestItem;
        }        
    }

    void GrabObject() // 물체를 잡는 함수
    {

        if (Vector3.Distance(GrabPoint.transform.position, TargetObject.transform.position) < GrabDistance)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                IsGrab = !IsGrab;
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

    void inputKey() // 조작 키에 대한 함수
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            HingeJoint hinge = joint1.GetComponent<HingeJoint>();
            JointMotor motor = hinge.motor;
            motor.targetVelocity = -ArmVelocity;
            hinge.motor = motor;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            HingeJoint hinge = joint1.GetComponent<HingeJoint>();
            JointMotor motor = hinge.motor;
            motor.targetVelocity = ArmVelocity;
            hinge.motor = motor;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            HingeJoint hinge = joint1.GetComponent<HingeJoint>();
            JointMotor motor = hinge.motor;
            motor.targetVelocity = 0;
            hinge.motor = motor;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            HingeJoint hinge = joint1.GetComponent<HingeJoint>();
            JointMotor motor = hinge.motor;
            motor.targetVelocity = 0;
            hinge.motor = motor;
        }
        if (Input.GetKey(KeyCode.A))
        {
            HingeJoint hinge = joint2.GetComponent<HingeJoint>();
            JointMotor motor = hinge.motor;
            motor.targetVelocity = -ArmVelocity;
            hinge.motor = motor;
        }
        if ((Input.GetKey(KeyCode.Z)))
        {
            HingeJoint hinge = joint2.GetComponent<HingeJoint>();
            JointMotor motor = hinge.motor;
            motor.targetVelocity = ArmVelocity;
            hinge.motor = motor;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            HingeJoint hinge = joint2.GetComponent<HingeJoint>();
            JointMotor motor = hinge.motor;
            motor.targetVelocity =0;
            hinge.motor = motor;
        }
        if ((Input.GetKeyUp(KeyCode.Z)))
        {
            HingeJoint hinge = joint2.GetComponent<HingeJoint>();
            JointMotor motor = hinge.motor;
            motor.targetVelocity =0;
            hinge.motor = motor;
        }

        if (Input.GetKey(KeyCode.S))
        {
            HingeJoint hinge = joint3.GetComponent<HingeJoint>();
            JointMotor motor = hinge.motor;
            motor.targetVelocity = ArmVelocity;
            hinge.motor = motor;
        }
        if ((Input.GetKey(KeyCode.X)))
        {
            HingeJoint hinge = joint3.GetComponent<HingeJoint>();
            JointMotor motor = hinge.motor;
            motor.targetVelocity = -ArmVelocity;
            hinge.motor = motor;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            HingeJoint hinge = joint3.GetComponent<HingeJoint>();
            JointMotor motor = hinge.motor;
            motor.targetVelocity = 0;
            hinge.motor = motor;
        }
        if ((Input.GetKeyUp(KeyCode.X)))
        {
            HingeJoint hinge = joint3.GetComponent<HingeJoint>();
            JointMotor motor = hinge.motor;
            motor.targetVelocity = 0;
            hinge.motor = motor;
        }

        if (Input.GetKey(KeyCode.D))
        {
            HingeJoint hinge = joint4.GetComponent<HingeJoint>();
            JointMotor motor = hinge.motor;
            motor.targetVelocity = -ArmVelocity;
            hinge.motor = motor;
        }
        if ((Input.GetKey(KeyCode.C)))
        {
            HingeJoint hinge = joint4.GetComponent<HingeJoint>();
            JointMotor motor = hinge.motor;
            motor.targetVelocity = ArmVelocity;
            hinge.motor = motor;
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            HingeJoint hinge = joint4.GetComponent<HingeJoint>();
            JointMotor motor = hinge.motor;
            motor.targetVelocity = 0;
            hinge.motor = motor;
        }
        if ((Input.GetKeyUp(KeyCode.C)))
        {
            HingeJoint hinge = joint4.GetComponent<HingeJoint>();
            JointMotor motor = hinge.motor;
            motor.targetVelocity = 0;
            hinge.motor = motor;
        }
    }
}

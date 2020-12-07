using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{

    public static int BallSpeed = 1; // 공 속도
    public static int PlayerLength = 1; // 플레이어 길이

    public GameObject BallSpeedObject; // 공 속도 텍스트
    public GameObject PlayerLengthObject; // 플레이어 길이 텍스트
    public GameObject Player1, Player2; // 플레이어 1,2

    Text BallSpeedText;
    Text PlayerLengthText;
    // Start is called before the first frame update
    void Start()
    {
        BallSpeedText = BallSpeedObject.GetComponent<Text>(); // 컴포넌트에 텍스트 불러옴
        PlayerLengthText = PlayerLengthObject.GetComponent<Text>(); // 컴포넌트에 텍스트 불러옴
    }

    // Update is called once per frame
    void Update()
    {
        BallSpeedText.text = "" + BallSpeed; // 공 속도 텍스트 업데이트
        PlayerLengthText.text = "" + PlayerLength; // 플레이어 길이 텍스트 업데이트
        Player1.transform.localScale = new Vector3(0.2f, 0.3f+0.7f *PlayerLength, 1); // 플레이어 스케일 y값 기본 0.3 , 길이 1씩 추가할때마다 스케일값 0.7 증가
        Player2.transform.localScale = new Vector3(0.2f, 0.3f+0.7f *PlayerLength, 1);
    }

    public void Mode1()
    {
        SceneManager.LoadScene("Mode1"); // 모드 1 씬 시작
    }
    public void Mode2()
    {
        SceneManager.LoadScene("Mode2"); // 모드 2 씬 시작
    }
    public void BallUP() // 공 속도 텍스트 증가
    {
        if (BallSpeed < 10)
        {
            BallSpeed++;
        }
    }

    public void BallDOWN() // 공 속도 텍스트 감소
    {
        if (BallSpeed > 1)
        {
            BallSpeed--;
        }
    }

    public void PlayerUP() // 플레이어 길이 텍스트 증가
    {
        if (PlayerLength < 10)
        {
            PlayerLength++;
        }
    }

    public void PlayerDOWN() // 플레이어 길이 텍스트 감소
    {
        if (PlayerLength > 1)
        {
            PlayerLength--;
        }
    }
}

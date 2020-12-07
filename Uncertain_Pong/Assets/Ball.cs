using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public GameObject Player1, Player2;
    public GameObject ScoreUI;
    float BallSpeedX,BallSpeedY; // 공 속도 x,y
    int random; // 랜덤한 방향으로 공이 시작하기 위해 랜덤 값을 받을 수
    // Start is called before the first frame update
    void Start()
    {
        
        BallHold();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(BallSpeedX, 0,BallSpeedY);  // 공 움직임
    }

    void BallHold() // 공 정지
    {
        //Player1.transform.position = new Vector3(-8,0,-1);
        //Player2.transform.position = new Vector3(8,0,-1);
        BallSpeedX = 0;
        BallSpeedY = 0;
        Invoke("BallGo", 2f); // 2초가 공 정지됌
        random = Random.Range(1, 3);
    }

    void BallGo() // 공 발사
    {
        transform.position = new Vector3(0, 0, -1);
         if(random == 1) // 오른쪽으로 발사
         {
             BallSpeedX = (2*LobbyUI.BallSpeed+1) * Time.deltaTime;
         }
         if(random == 2) // 왼쪽으로 발사
         {
             BallSpeedX = (-2*LobbyUI.BallSpeed-1) * Time.deltaTime;
         }
        BallSpeedY = Random.Range(-5, 5); // y축 속도 랜덤 값 계산
        if (BallSpeedY < 3 && BallSpeedY >=0)
        {
            BallSpeedY = BallSpeedY + 3;
        }
        if (BallSpeedY > -3 && BallSpeedY < 0)
        {
            BallSpeedY = BallSpeedY - 3;
        }
        BallSpeedY = BallSpeedY * Time.deltaTime;
        

    }


   
    private void OnCollisionEnter(Collision collision) // 벽이나 플레이어에 부딪혔을때 계산
    {
        if (collision.gameObject.tag == "Player1")
        {
            BallSpeedX = -BallSpeedX;
        }
        if (collision.gameObject.tag == "Player2")
        {
            BallSpeedX = -BallSpeedX;
        }
        if (collision.gameObject.tag == "UpWall")
        {
            BallSpeedY = -BallSpeedY;
        }
        if (collision.gameObject.tag == "DownWall")
        {
            BallSpeedY = -BallSpeedY;
        }
        if (collision.gameObject.tag == "LeftWall")
        {
            BallHold();
            ScoreUI.GetComponent<ScoreUI>().Point2up(); // 점수 올리기
            if(ScoreUI.GetComponent<ScoreUI>().Point2 == 11) // 게임 끝
            {
                ScoreUI.GetComponent<ScoreUI>().ballkill();
            }
        }
        if (collision.gameObject.tag == "RightWall")
        {
            BallHold();
            ScoreUI.GetComponent<ScoreUI>().Point1up(); // 점수 올리기
            if (ScoreUI.GetComponent<ScoreUI>().Point1 == 11) // 게임 끝
            {
                ScoreUI.GetComponent<ScoreUI>().ballkill();
            }
        }
    }
}

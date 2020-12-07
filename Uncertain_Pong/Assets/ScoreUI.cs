using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreUI : MonoBehaviour
{
    public GameObject ball;
    public GameObject Win1, Win2;
    public GameObject Score1, Score2;
    Text Score1text, Score2text;

    public int Point1, Point2;
    // Start is called before the first frame update
    void Start()
    {
        Point1 = 0;
        Point2 = 0;
        Score1text = Score1.GetComponent<Text>();
        Score2text = Score2.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Score1text.text = "" + Point1 ;
        Score2text.text = "" + Point2 ;
        if(Point1 == 11)
        {
            Win1.SetActive(true);
            
        }
        if (Point2 == 11)
        {
            Win2.SetActive(true);
            
        }
    }

    public void ballkill()
    {
        ball.SetActive(false);
        Invoke("GoLobby", 3f);
    }

    public void GoLobby()
    {
        SceneManager.LoadScene("Lobby"); // 로비 씬 불러오기
    }

    public void Point1up()
    {
        Point1++;
    }
    public void Point2up()
    {
        Point2++;
    }
}

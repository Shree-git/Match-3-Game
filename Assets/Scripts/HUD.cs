using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Level level;
    public GameOver gameOver;
    public Text remainingText, targetText, scoreText, remainingSubtext, targetSubtext;
    public Image[] stars;
   

    private int starsIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            if (i == starsIndex)
            {
                stars[i].enabled = true;
            }
            else
            {
                stars[i].enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTarget(int tarText)
    {
        targetText.text = tarText.ToString();
    }
    public void SetRemaining(int remText)
    {
        remainingText.text = remText.ToString();
    }

    public void SetRemaining(string remText)
    {
        remainingText.text = remText;
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();

        int visibleStars = 0;

        if(score >= level.score1Star && score < level.score2Star)
        {
            visibleStars = 1;
        } else if (score >= level.score2Star && score < level.score3Star)
        {
            visibleStars = 2;
        }else if(score >= level.score3Star)
        {
            visibleStars = 3;
        }

        for (int i = 0; i < stars.Length; i++)
        {
            if(i == visibleStars)
            {
                stars[i].enabled = true;
            }
            else
            {
                stars[i].enabled = false;
            }
        }

        starsIndex = visibleStars;
    }

   

    public void SetLevelType(Level.LevelType type)
    {
        if(type == Level.LevelType.MOVES)
        {
            remainingSubtext.text = "Moves\nRemaining";
            targetSubtext.text = "Target\nScore";
        } else if (type == Level.LevelType.OBSTACLE)
        {
            remainingSubtext.text = "Moves\nRemaining";
            targetSubtext.text = "Bubbles\nRemaining";
        }else if(type == Level.LevelType.TIMER)
        {
            remainingSubtext.text = "Time\nRemaining";
            targetSubtext.text = "Target\nScore";
        }
    }

    public void OnGameWin(int finalScore)
    {
        gameOver.GameWin(finalScore, starsIndex);
        if (starsIndex > PlayerPrefs.GetInt(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 0))
        {
            PlayerPrefs.SetInt(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, starsIndex);
        }
    }

    public void OnGameLose()
    {
        gameOver.GameLose();
       
    }
}

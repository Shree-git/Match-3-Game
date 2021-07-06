using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public HUD hud;
    public enum LevelType
    {
        TIMER,
        MOVES,
        OBSTACLE
    }

    public Grid grid;

    protected LevelType type;
    public LevelType Type
    {
        get { return type; }
    }

    public int score1Star;
    public int score2Star;
    public int score3Star;

    protected int currentScore;

    protected bool didWin;
    // Start is called before the first frame update
    void Start()
    {
        hud.SetScore(currentScore);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void GameWin()
    {
        
        grid.GameOver();
        didWin = true;
        StartCoroutine(WaitForGridFill());
    }

    public virtual void GameLose()
    {
       
        grid.GameOver();
        didWin = false;
        StartCoroutine(WaitForGridFill());
    }

    public virtual void OnMove()
    {
      
    }

    public virtual void OnPieceCleared(GamePiece piece)
    {
        // Update score
        currentScore += piece.score;
        hud.SetScore(currentScore);
    }

    protected virtual IEnumerator WaitForGridFill()
    {
        while (grid.IsFilling)
        {
            yield return 0;
        }

        if (didWin)
        {
            hud.OnGameWin(currentScore);
        }
        else
        {
            hud.OnGameLose();
        }
    }
}

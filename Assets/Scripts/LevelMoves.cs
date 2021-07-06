using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMoves : Level
{
    public int numOfMoves;
    public int targetScore;

    private int movesUsed;
    // Start is called before the first frame update
    void Start()
    {
        type = LevelType.MOVES;

        hud.SetLevelType(type);
        hud.SetRemaining(numOfMoves);
        hud.SetTarget(targetScore);
        hud.SetScore(currentScore);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnMove()
    {
    

        movesUsed++;

        hud.SetRemaining(numOfMoves - movesUsed);

        if(numOfMoves-movesUsed <= 0)
        {
            if (currentScore > targetScore)
            {
                GameWin();
            }
            else
            {
                GameLose();
            }
        }

    }
}

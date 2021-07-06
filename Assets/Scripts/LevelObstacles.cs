using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObstacles : Level
{
    public int numOfMoves;
    private int numOfObstacles;
    private int movesUsed;

    public Grid.PieceType[] obstacleTypes;
    // Start is called before the first frame update
    void Start()
    {
        type = LevelType.OBSTACLE;

        for (int i = 0; i < obstacleTypes.Length; i++)
        {
            numOfObstacles += grid.GetPiecesOfType(obstacleTypes[i]).Count;
        }

        hud.SetLevelType(type);
        hud.SetRemaining(numOfMoves);
        hud.SetTarget(numOfObstacles);
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
        if (numOfMoves - movesUsed == 0 && numOfObstacles > 0)
        {
            GameLose();
        }
    }

    public override void OnPieceCleared(GamePiece piece)
    {
        base.OnPieceCleared(piece);

        for (int i = 0; i < obstacleTypes.Length; i++)
        {
            if(piece.Type == obstacleTypes[i])
            {
                numOfObstacles--;
                hud.SetTarget(numOfObstacles);

                if(numOfObstacles <= 0)
                {
                    currentScore = 1000 * (numOfMoves - movesUsed);
                    hud.SetScore(currentScore);
                    GameWin();
                }
            }
        }
    }
}

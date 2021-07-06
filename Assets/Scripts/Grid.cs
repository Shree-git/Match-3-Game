using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	public enum PieceType
    {
        NORMAL,
        EMPTY,
        BUBBLE,
        ROW,
        COLUMN,
        RAINBOW,
        COUNT
    }

    [System.Serializable]
    public struct PiecePrefab
    {
        public PieceType pieceType;
        public GameObject piecePrefab;
    }

    public PiecePrefab[] piecePrefabs;
    public GameObject backgroundPrefab;

    private Dictionary<PieceType, GameObject> piecesDict;

    [System.Serializable]
    public struct PiecesPosition
    {
        public PieceType type;
        public int x;
        public int y;
    }

    public PiecesPosition[] initialPieces;

    public int xDim;
    public int yDim;
    public float fillTime;

    public Level level;

    public GamePiece[,] pieces;

    private bool inverse = false;

    private GamePiece pressedPiece;
    private GamePiece enteredPiece;

    private bool gameOver = false;

    private bool isFilling = false;
    public bool IsFilling
    {
        get { return isFilling; }
    }
    void Awake()
    {
        piecesDict = new Dictionary<PieceType, GameObject>();
        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            if (!piecesDict.ContainsKey(piecePrefabs[i].pieceType))
            {
                piecesDict.Add(piecePrefabs[i].pieceType, piecePrefabs[i].piecePrefab);
            }
        }

        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                GameObject background = (GameObject)Instantiate(backgroundPrefab, SetWorldPosition(x, y), Quaternion.identity);
                background.transform.parent = transform;
            }
        }

        pieces = new GamePiece[xDim, yDim];

        for (int i = 0; i < initialPieces.Length; i++)
        {
            if(initialPieces[i].x >= 0 && initialPieces[i].x < xDim
                && initialPieces[i].y >=0 && initialPieces[i].y < yDim)
            {
                SpawnNewPiece(initialPieces[i].x, initialPieces[i].y, initialPieces[i].type);
            }
        }

        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                if (pieces[x, y] == null)
                {
                    SpawnNewPiece(x, y, PieceType.EMPTY);
                }
            }
        }

        StartCoroutine(Fill());
        
    }

    public IEnumerator Fill()
    {
        bool needsRefill = true;
        isFilling = true;

        while (needsRefill)
        {
            yield return new WaitForSeconds(fillTime);
            while (FillStep())
            {
                inverse = !inverse;
                yield return new WaitForSeconds(fillTime);
            }

            needsRefill = ClearAllValidMatches();
        }

        isFilling = false;
    }

    public bool FillStep()
    {
        bool movedPiece = false;

        for (int y = yDim-2; y >= 0; y--)
        {
            for (int loopX = 0; loopX < xDim; loopX++)
            {
                int x = loopX;

                if (inverse)
                {
                    x = xDim - 1 - loopX;
                }
                GamePiece piece = pieces[x, y];

                if (piece.IsMovable())
                {
                    GamePiece pieceBelow = pieces[x, y + 1];
                    if (pieceBelow.Type == PieceType.EMPTY)
                    {
                        Destroy(pieceBelow.gameObject);
                        piece.MovableComponent.Move(x, y + 1, fillTime);
                        pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y, PieceType.EMPTY);
                        movedPiece = true;
                    }
                    else
                    {
                      
                        for (int diag = -1; diag <= 1; diag++)
                        {
                            if(diag != 0)
                            {
                                int diagX = x + diag;

                                if (inverse)
                                {
                                    diagX = x - diag;
                                
                                }

                                if(diagX >= 0 && diagX < xDim)
                                {
                                   
                                    GamePiece diagonalPiece = pieces[diagX, y + 1];

                                    if(diagonalPiece.Type == PieceType.EMPTY)
                                    {
                                        
                                        bool hasPieceAbove = true;

                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            GamePiece pieceAbove = pieces[diagX, aboveY];

                                            if (pieceAbove.IsMovable())
                                            {
                                               
                                                break;
                                            }
                                            else if(!pieceAbove.IsMovable() && pieceAbove.Type != PieceType.EMPTY)
                                            {
                                               
                                                hasPieceAbove = false;
                                                break;
                                            }

                        
                                        }
                                        if (!hasPieceAbove)
                                        {
                                            Destroy(diagonalPiece.gameObject);
                                            piece.MovableComponent.Move(diagX, y + 1, fillTime);
                                            pieces[diagX, y + 1] = piece;
                                            SpawnNewPiece(x, y, PieceType.EMPTY);
                                            movedPiece = true;
                                            
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        for (int x = 0; x < xDim; x++)
        {
            GamePiece pieceBelow = pieces[x, 0]; 
            if (pieceBelow.Type == PieceType.EMPTY) 
            {
                Destroy(pieceBelow.gameObject);
                GameObject newPiece = (GameObject)Instantiate(piecesDict[PieceType.NORMAL], SetWorldPosition(x, -1), Quaternion.identity);
                newPiece.transform.parent = transform;

                pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                pieces[x, 0].Init(x, -1, PieceType.NORMAL, this);
                pieces[x, 0].MovableComponent.Move(x, 0, fillTime);
                pieces[x, 0].ColorComponent.ChangeColor((ColorPiece.ColorName)Random.Range(0, pieces[x, 0].ColorComponent.NumColors));
                movedPiece = true;
            }
        }

        return movedPiece;
    }

    public GamePiece SpawnNewPiece(int x, int y, PieceType type)
    {
        GameObject newPiece = (GameObject)Instantiate(piecesDict[type], SetWorldPosition(x, y), Quaternion.identity);
        newPiece.name = "Piece(" + x + "," + y + ")";
        newPiece.transform.parent = transform;

        pieces[x, y] = newPiece.GetComponent<GamePiece>();
        pieces[x, y].Init(x, y, type, this);

        return pieces[x, y];
    }

    public Vector2 SetWorldPosition(int x, int y)
    {
        return new Vector2(transform.position.x - xDim / 2 + x,
            transform.position.y + yDim / 2 - y);
    }

    public bool IsAdjacent(GamePiece piece1, GamePiece piece2)
    {
        return (piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1) 
            || (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X-piece2.X)==1);
    }

    public void SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        if (gameOver)
        {
            return;
        }
        if (piece1.IsMovable() && piece2.IsMovable())
        {
            pieces[piece1.X, piece1.Y] = piece2;
            pieces[piece2.X, piece2.Y] = piece1;

            if (GetMatch(piece1, piece2.X, piece2.Y) != null || GetMatch(piece2, piece1.X, piece1.Y) != null
                || piece1.Type == PieceType.RAINBOW || piece2.Type == PieceType.RAINBOW)
            {
                Debug.Log("There is a match");
                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                piece1.MovableComponent.Move(piece2.X, piece2.Y, fillTime);
                piece2.MovableComponent.Move(piece1X, piece1Y, fillTime);

                if(piece1.Type == PieceType.RAINBOW && piece1.IsClearable() && piece2.IsColorable())
                {
                    ClearAnyPiece clearColor = piece1.GetComponent<ClearAnyPiece>();

                    if (clearColor)
                    {
                        clearColor.Color = piece2.ColorComponent.Color;
                    }

                    ClearPiece(piece1.X, piece1.Y);
                }

                if (piece2.Type == PieceType.RAINBOW && piece2.IsClearable() && piece1.IsColorable())
                {
                    ClearAnyPiece clearColor = piece2.GetComponent<ClearAnyPiece>();

                    if (clearColor)
                    {
                        clearColor.Color = piece1.ColorComponent.Color;
                    }

                    ClearPiece(piece2.X, piece2.Y);
                }

                ClearAllValidMatches();

                if(piece1.Type == PieceType.ROW || piece1.Type == PieceType.COLUMN)
                {
                    ClearPiece(piece1.X, piece2.X);
                }

                if (piece2.Type == PieceType.ROW || piece2.Type == PieceType.COLUMN)
                {
                    ClearPiece(piece2.X, piece2.X);
                }

                pressedPiece = null;
                enteredPiece = null;

                StartCoroutine(Fill());

                level.OnMove();
            }
            else
            {
                pieces[piece1.X, piece1.Y] = piece1;
                pieces[piece2.X, piece2.Y] = piece2;
            } 
        }
    }

    public void PressPiece(GamePiece piece)
    {
        pressedPiece = piece;
    }

    public void EnterPiece(GamePiece piece)
    {
        enteredPiece = piece;
    }

    public void ReleasePiece()
    {
        if(IsAdjacent(pressedPiece, enteredPiece))
        {
            SwapPieces(pressedPiece, enteredPiece);
        }
    }

    public List<GamePiece> GetMatch(GamePiece piece, int newX, int newY)
    {
        if (piece.IsColorable())
        {
            ColorPiece.ColorName color = piece.ColorComponent.Color;
            List<GamePiece> horizontalPieces = new List<GamePiece>();
            List<GamePiece> verticalPieces = new List<GamePiece>();
            List<GamePiece> matchingPieces = new List<GamePiece>();

            //Checking horizontally first

            horizontalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xOffset = 1; xOffset < xDim; xOffset++)
                {
                    int x;

                    if (dir == 0) //Left
                    {
                        x = newX - xOffset;
                    }
                    else
                    {
                        x = newX + xOffset;
                    }

                    if (x < 0 || x >= xDim)
                    {
                        break;
                    }

                    if(pieces[x,newY].IsColorable() && pieces[x, newY].ColorComponent.Color == color)
                    {
                        horizontalPieces.Add(pieces[x, newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if(horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    matchingPieces.Add(horizontalPieces[i]);
                }
            }

            //Traverse vertically if we found a match (for L and T shape)
            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int yOffset = 1; yOffset < yDim; yOffset++)
                        {
                            int y;

                            if (dir == 0) //Up
                            {
                                y = newY - yOffset;
                            }
                            else //Down
                            {
                                y = newY + yOffset;
                            }

                            if (y < 0 || y >= yDim)
                            {
                                break;
                            }

                            if (pieces[horizontalPieces[i].X, y].IsColorable() && pieces[horizontalPieces[i].X, y].ColorComponent.Color == color)
                            {
                                verticalPieces.Add(pieces[horizontalPieces[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if(verticalPieces.Count < 2)
                    {
                        verticalPieces.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < verticalPieces.Count; j++)
                        {
                            matchingPieces.Add(verticalPieces[j]);
                        }
                    }
                }
            }

            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }


            //No horizontal pieces found. Now checking vertically
            horizontalPieces.Clear();
            verticalPieces.Clear();
            verticalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < yDim; yOffset++)
                {
                    int y;

                    if (dir == 0) //Up
                    {
                        y = newY - yOffset;
                    }
                    else //Down
                    {
                        y = newY + yOffset;
                    }

                    if (y < 0 || y >= yDim)
                    {
                        break;
                    }

                    if (pieces[newX, y].IsColorable() && pieces[newX, y].ColorComponent.Color == color)
                    {
                        verticalPieces.Add(pieces[newX, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    matchingPieces.Add(verticalPieces[i]);
                }
            }

            //Traverse vertically if we found a match (for L and T shape)
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < xDim; xOffset++)
                        {
                            int x;

                            if (dir == 0) //Left
                            {
                                x = newX - xOffset;
                            }
                            else //Right
                            {
                                x = newX + xOffset;
                            }

                            if (x < 0 || x >= yDim)
                            {
                                break;
                            }

                            if (pieces[x, verticalPieces[i].Y].IsColorable() && pieces[x, verticalPieces[i].Y].ColorComponent.Color == color)
                            {
                                horizontalPieces.Add(pieces[x, verticalPieces[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (horizontalPieces.Count < 2)
                    {
                        horizontalPieces.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < horizontalPieces.Count; j++)
                        {
                            matchingPieces.Add(horizontalPieces[j]);
                        }
                    }
                }
            }

            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }
        }

        return null;
    }

    public bool ClearAllValidMatches()
    {
        bool needsRefill = false;

        for (int y = 0; y < yDim; y++)
        {
            for (int x = 0; x < xDim; x++)
            {
                if (pieces[x, y].IsClearable())
                {
                    List<GamePiece> match = GetMatch(pieces[x, y], x, y);

                    if(match != null)
                    {
                        PieceType specialPieceType = PieceType.COUNT;
                        GamePiece randomPiece = match[Random.Range(0, match.Count)];
                        int specialPieceX = randomPiece.X;
                        int specialPieceY = randomPiece.Y;

                        if(match.Count == 4)
                        {
                            if(pressedPiece == null || enteredPiece == null)
                            {
                                specialPieceType = (PieceType)Random.Range((int)PieceType.ROW, (int)PieceType.COLUMN);

                            } else if(pressedPiece.Y == enteredPiece.Y)
                            {
                                specialPieceType = PieceType.ROW;
                            }
                            else
                            {
                                specialPieceType = PieceType.COLUMN;
                            }
                        } else if (match.Count >= 5)
                        {
                            specialPieceType = PieceType.RAINBOW;
                        }

                        for (int i = 0; i < match.Count; i++)
                        {
                            if(ClearPiece(match[i].X, match[i].Y))
                            {
                                needsRefill = true;

                                if(match[i] == pressedPiece || match[i] == enteredPiece)
                                {
                                    specialPieceX = match[i].X;
                                    specialPieceY = match[i].Y;
                                }
                            }
                        }

                        if(specialPieceType != PieceType.COUNT)
                        {
                            Destroy(pieces[specialPieceX, specialPieceY]);
                            GamePiece newPiece = SpawnNewPiece(specialPieceX, specialPieceY, specialPieceType);

                            if((specialPieceType == PieceType.ROW || specialPieceType == PieceType.COLUMN)
                                && newPiece.IsColorable() && match[0].IsColorable())
                            {
                                newPiece.ColorComponent.Color = match[0].ColorComponent.Color;
                            } else if(specialPieceType == PieceType.RAINBOW && newPiece.IsColorable())
                            {
                                newPiece.ColorComponent.Color = ColorPiece.ColorName.ANY;
                            }
                        }
                    }
                }
            }
        }

        return needsRefill;
    }
    public bool ClearPiece(int x, int y)
    {
        if (pieces[x, y].IsClearable() && !pieces[x, y].ClearableComponent.IsBeingCleared)
        {
            pieces[x, y].ClearableComponent.Clear();
            SpawnNewPiece(x, y, PieceType.EMPTY);
            ClearBubble(x,y);
            return true;
        }

        return false;
    }

    public void ClearBubble(int x, int y)
    {
        for (int adjX = x-1; adjX <= x+1; adjX++)
        {
            if (adjX != x && adjX >= 0 && adjX < xDim)
            {
                if (pieces[adjX, y].IsClearable() && pieces[adjX, y].Type == PieceType.BUBBLE)
                {
                    pieces[adjX, y].ClearableComponent.Clear();
                    SpawnNewPiece(adjX, y, PieceType.EMPTY);
                }
            }
        }
        for (int adjY = y - 1; adjY <= y + 1; adjY++)
        {
            if (adjY != y && adjY >= 0 && adjY < yDim)
            {
                if (pieces[x,adjY].IsClearable() && pieces[x, adjY].Type == PieceType.BUBBLE)
                {
                    pieces[x, adjY].ClearableComponent.Clear();
                    SpawnNewPiece(x, adjY, PieceType.EMPTY);
                }
            }
        }

    }

    public void ClearRow(int row)
    {
        for (int x = 0; x < xDim; x++)
        {
            ClearPiece(x, row);
        }
    }

    public void ClearColumn(int column)
    {
        for (int y = 0; y < yDim; y++)
        {
            ClearPiece(column, y);
        }
    }

    public void ClearRainbowAll(ColorPiece.ColorName color)
    {
        for (int i = 0; i < xDim; i++)
        {
            for (int j = 0; j < yDim; j++)
            {
                if(pieces[i,j].IsColorable() && (pieces[i,j].ColorComponent.Color == color || color == ColorPiece.ColorName.ANY))
                {
                    ClearPiece(i, j);
                }
            }
        }
    }

    public void GameOver()
    {
        gameOver = true;
    }

    public List<GamePiece> GetPiecesOfType(PieceType piece)
    {
        List<GamePiece> gamePieces = new List<GamePiece>();

        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                if(pieces[x,y].Type == piece)
                {
                    gamePieces.Add(pieces[x, y]);
                }
            }
        }

        return gamePieces;
    }
	// Update is called once per frame
	void Update () {
	
	}
}

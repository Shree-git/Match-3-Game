using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    public int score;
   
    private int x, y;

    public int X
    {
        get { return x; }
        set
        {
            if (IsMovable())
            {
                x = value;
            }
        }
    }

    public int Y
    {
        get { return y; }
        set
        {
            if (IsMovable())
            {
                y = value;
            }
        }
    }

    private Grid.PieceType type;
    public Grid.PieceType Type
    {
        get { return type; }
    }

    private Grid gridRef;
    public Grid GridRef
    {
        get { return gridRef; }
    }

    private MovablePiece movableComponent;
    public MovablePiece MovableComponent
    {
        get { return movableComponent; }
    }

    private ColorPiece colorComponent;
    public ColorPiece ColorComponent
    {
        get { return colorComponent; }
    }

    private ClearablePiece clearableComponent;
    public ClearablePiece ClearableComponent
    {
        get { return clearableComponent; }
    }

    void Awake()
    {
        movableComponent = GetComponent<MovablePiece>();
        colorComponent = GetComponent<ColorPiece>();
        clearableComponent = GetComponent<ClearablePiece>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(int _x, int _y, Grid.PieceType _type, Grid _gridRef)
    {
        x = _x;
        y = _y;
        type = _type;
        gridRef = _gridRef;
    }

    void OnMouseEnter()
    {
        gridRef.EnterPiece(this);
    }

    void OnMouseDown()
    {
        gridRef.PressPiece(this);
    }

    void OnMouseUp()
    {
        gridRef.ReleasePiece();
    }

    public bool IsMovable()
    {
        return movableComponent != null;
    }

    public bool IsColorable()
    {
        return colorComponent != null;
    }

    public bool IsClearable()
    {
        return clearableComponent != null;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

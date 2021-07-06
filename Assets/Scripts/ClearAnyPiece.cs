using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearAnyPiece : ClearablePiece
{
    private ColorPiece.ColorName color;
    public ColorPiece.ColorName Color
    {
        get { return color; }
        set { color = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Clear()
    {
        base.Clear();

        piece.GridRef.ClearRainbowAll(color);

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePiece : MonoBehaviour
{
    private GamePiece piece;
    private IEnumerator moveCoroutine;

    void Awake()
    {
        piece = GetComponent<GamePiece>();
    }

    public void Move(int setX, int setY, float time)
    {
        if(moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = MoveCoroutine(setX, setY, time);
        StartCoroutine(moveCoroutine);
    }

    private IEnumerator MoveCoroutine(int setX, int setY, float time)
    {
        piece.X = setX;
        piece.Y = setY;

        Vector3 startPos = transform.position;
        Vector3 endPos = piece.GridRef.SetWorldPosition(setX, setY);

        //InterPolation
        for (float t = 0; t <= 1 * time; t += Time.deltaTime)
        {
            piece.transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return 0;
        }

        piece.transform.position = endPos;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

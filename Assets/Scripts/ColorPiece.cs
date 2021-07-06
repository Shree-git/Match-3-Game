using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPiece : MonoBehaviour
{
    public enum ColorName
    {
        YELLOW,
        PURPLE,
        RED,
        BLUE,
        GREEN,
        PINK,
        ANY,
        COUNT
    };

    [System.Serializable]
    public struct ColorSprite
    {
        public ColorName colorName;
        public Sprite sprite;
    }

    public ColorSprite[] colorSprite;

    private ColorName color;
    public ColorName Color
    {
        get { return color; }
        set { ChangeColor(value); }
    }

    private Dictionary<ColorName, Sprite> colorSpriteDict;

    private SpriteRenderer sprite;
    void Awake()
    {
        colorSpriteDict = new Dictionary<ColorName, Sprite>();
        for (int i = 0; i < colorSprite.Length; i++)
        {
            if (!colorSpriteDict.ContainsKey(colorSprite[i].colorName))
            {
                colorSpriteDict.Add(colorSprite[i].colorName, colorSprite[i].sprite);
            }
        }
        sprite = transform.Find("piece").GetComponent<SpriteRenderer>();
    }

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeColor(ColorName newColor)
    {
        color = newColor;

        if (colorSpriteDict.ContainsKey(newColor))
        {
            sprite.sprite = colorSpriteDict[newColor];
        }
    }

    public int NumColors
    {
        get { return colorSprite.Length; }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileComponent : MonoBehaviour
{
    public TileData TileData;
    SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateTileData(ref TileData tileData)
    {
        TileData = tileData;
        spriteRenderer.sprite = TileData.sprite;
    }

    public  string GetText()
    {
        return TileData.ToString();
    }
}

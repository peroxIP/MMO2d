using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileData
{
    public string Type;
    public string Name;
    public Sprite sprite;
    public int Level;
    public bool IsOccupied = false;
    public TileData(string type, string name, Sprite sprite)
    {
        Type = type;
        Name = name;
        this.sprite = sprite;
    }
    
    public override string ToString()
    {
        return string.Format("Name: {0}\nType:{1}", Name, Type);
    }
}

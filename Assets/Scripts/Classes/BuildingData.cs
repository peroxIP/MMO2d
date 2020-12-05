using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData : TileData
{

    public BuildingData(string type, string name, Sprite sprite, int level) : base(type, name, sprite)
    {
        Level = level;
    }

    public BuildingData Clone()
    {
        return new BuildingData(this.Type, this.Name, this.sprite, this.Level);
    }

    public override string ToString()
    {
        return string.Format("{0}\nLevel:{1}", base.ToString(), Level) ;
    }
}

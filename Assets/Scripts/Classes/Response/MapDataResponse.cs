using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapDataResponse
{
    public int map_width;
    public int map_height;
    public int number_of_houses;
    public List<TileDataResponse> tiles;
}


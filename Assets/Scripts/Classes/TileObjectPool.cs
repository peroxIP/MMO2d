using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObjectPool : MonoBehaviour
{
    public GameObject Original;

    private GameObject[,] Pool;
    private int Range;
    private int Offset;
    private Dictionary<GameObject, TileComponent> MapGOToComponent;
    public void Setup(int range)
    {
        Range = range;
        Pool = new GameObject[Range, Range];
        MapGOToComponent = new Dictionary<GameObject, TileComponent>();
        Offset = Range / 2;
        InitPool();
    }

    public void UpdatePoolObjects(ref Vector2Int cameraPosition, ref TileData[,] tileDatas)
    {
        this.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, 0);
        for (int col = 0; col < Range; col++)
        {
            for (int row = 0; row < Range; row++)
            {
                TileData tileData = tileDatas[row, col];

                GameObject currentPoolObject = Pool[row, col];

                if (tileData == null)
                {
                    currentPoolObject.SetActive(false);
                }
                else
                {
                    currentPoolObject.SetActive(true);
                    TileComponent component = MapGOToComponent[currentPoolObject];
                    component.UpdateTileData(ref tileData);
                }
            }
        }
    }

    private void InitPool()
    {
        for (int col = 0; col < Range; col++)
        {
            for (int row = 0; row < Range; row++)
            {
                GameObject gameObject = Instantiate(Original, new Vector3(row - Offset, col - Offset, 0), Quaternion.identity);
                gameObject.transform.parent = this.transform;
                Pool[row, col] = gameObject;

                MapGOToComponent[gameObject] = gameObject.GetComponent<TileComponent>();
            }
        }
    }

    
}

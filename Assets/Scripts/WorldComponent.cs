using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Linq;

public class WorldComponent : MonoBehaviour
{
    [Header("WorldGeneration")]
    public string WorldSpecificationURL;
    public GameObject PoolGO;
    public int PoolRange;
    public bool isDebug;

    [Header("Art")]
    public List<Sprite> sprites;
    private Dictionary<string, Sprite> NameToSprite;

    [Header("UI info")]
    public Text textNumverOfHouses;
    private string formatNumberOfHouses = "Number of houses: {0}";



    private int WorldWidth;
    private int WorldHeight;
    private int MaxNumberOfHouses;
    private int CurrentNumberOfBuilding;

    private TileObjectPool tileObjectPool;

    private TileData[,] WorldMap;

    private int HalfRange;
    private Vector2Int CameraOffset = Vector2Int.zero;
    private TileData[,] ViewField;

    private List<TileData> EmptyTiles;
    private List<BuildingData> BuildingTiles;

    private void Awake()
    {
        Random.InitState(1);
        EmptyTiles = new List<TileData>();
        BuildingTiles = new List<BuildingData>();
        tileObjectPool = PoolGO.GetComponent<TileObjectPool>();
        NameToSprite = new Dictionary<string, Sprite>();

        tileObjectPool.Setup(PoolRange);

        ViewField = new TileData[PoolRange, PoolRange];
        HalfRange = PoolRange / 2;

        foreach (var item in sprites)
        {
            NameToSprite[item.name] = item;
        }
        StartCoroutine(GetRequest(WorldSpecificationURL));
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Processjson(webRequest.downloadHandler.text);
            }
        }
    }

    private void Processjson(string jsonString)
    {
        MapDataResponse data = JsonUtility.FromJson<MapDataResponse>(jsonString);

        WorldWidth = data.map_width;
        WorldHeight = data.map_height;
        MaxNumberOfHouses = data.number_of_houses;
        if (isDebug)
        {
            WorldWidth = 30;
            WorldHeight = 30;
            MaxNumberOfHouses = 20;
        }


        foreach (var item in data.tiles)
        {
            if (item.level == 0)
            {
                EmptyTiles.Add(new TileData(item.type, item.name, NameToSprite[item.type]));
            }
            else
            {
                BuildingTiles.Add(new BuildingData(item.type, item.name, NameToSprite[item.type], item.level));
            }
        }
        

        WorldMap = new TileData[WorldWidth, WorldHeight];

        for (int col = 0; col < WorldHeight; col++)
        {
            for (int row = 0; row < WorldWidth; row++)
            {
                int a = Random.Range(0, EmptyTiles.Count);
                WorldMap[col, row] = EmptyTiles[a];
            }
        }

        CurrentNumberOfBuilding = MaxNumberOfHouses;
        while (CurrentNumberOfBuilding != 0)
        {
            int col = Random.Range(0, WorldHeight - 1);
            int row = Random.Range(0, WorldWidth - 1);

            var z = WorldMap[col, row];
            if (!z.IsOccupied)
            {
                int a = Random.Range(0, BuildingTiles.Count);
                BuildingData b = BuildingTiles[a].Clone();
                b.IsOccupied = true;

                WorldMap[col, row] = b;

                CurrentNumberOfBuilding -= 1;
            }
        }
        CurrentNumberOfBuilding = MaxNumberOfHouses;
        textNumverOfHouses.text = string.Format(formatNumberOfHouses, CurrentNumberOfBuilding);

        var cm = Camera.main.GetComponent<CameraMovement>();
        cm.SetWorld(this);        
    }

    public bool IsOutOfBounds(int row, int col)
    {
        if (row < 0 || row > WorldWidth - 1 || col < 0 || col > WorldHeight - 1)
        {
            return true;
        }
        return false;
    }

    private TileData GetTileAt(int row, int col)
    {
        if (IsOutOfBounds(row, col))
        {
            return null;
        }
        return WorldMap[row, col];
    }

    public void UpdateViewField()
    {
        for (int col = CameraOffset.y - HalfRange; col < CameraOffset.y + HalfRange; col++)
        {
            for (int row = CameraOffset.x - HalfRange; row < CameraOffset.x + HalfRange; row++)
            {
                ViewField[row - CameraOffset.x + HalfRange, col - CameraOffset.y + HalfRange] = GetTileAt(row, col);
            }
        }
        tileObjectPool.UpdatePoolObjects(ref CameraOffset, ref ViewField);
    }

    public void UpdateCameraPosition(Vector2Int position)
    {
        CameraOffset = position;
        UpdateViewField();
    }

    public void HandleDestroyHouse(int col , int row)
    {
        WorldMap[col, row] = EmptyTiles[0];
        CurrentNumberOfBuilding--;
        textNumverOfHouses.text = string.Format(formatNumberOfHouses, CurrentNumberOfBuilding);
    }
}

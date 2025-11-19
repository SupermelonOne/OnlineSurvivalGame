using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PerlinNoiseMap : MonoBehaviour
{
    [SerializeField] private GameObject emptyTile;
    private GameObject floorTiles;

    Dictionary<int, GameObject> tileSet;
    Dictionary<int, GameObject> tileGroups;
    [SerializeField] private GameObject tree;
    [SerializeField] private GameObject rock;
    [SerializeField] private GameObject flower;

    [SerializeField] int map_width = 16;
    [SerializeField] int map_height = 9;

    List<List<int>> noiseGrid = new List<List<int>>();
    List<List<GameObject>> tileGrid = new List<List<GameObject>>();

    [SerializeField] private float magnification = 5;
    [SerializeField] int x_offset = 0;
    [SerializeField] int y_offset = 0;

    private void Start()
    {
        SetTileset();
        CreateTileGroups();

        GenerateMap();
    }
    private void SetTileset()
    {
        tileSet = new Dictionary<int, GameObject>();
        tileSet.Add(1, tree);
        tileSet.Add(2, rock);
        tileSet.Add(3, flower);
    }
    private void CreateTileGroups()
    {
        floorTiles = new GameObject("floor");
        tileGroups = new Dictionary<int, GameObject>();
        foreach (KeyValuePair<int, GameObject> prefabPair in tileSet)
        {
            GameObject tileGroup = new GameObject(prefabPair.Value.name);
            tileGroup.transform.parent = transform;
            tileGroup.transform.localPosition = Vector3.zero;
            tileGroups.Add(prefabPair.Key, tileGroup);
        }
    }
    private void GenerateMap()
    {
        for (int x = 0; x < map_width; x++)
        {
            noiseGrid.Add(new List<int>());
            tileGrid.Add(new List<GameObject>());

            for (int y = 0; y < map_height; y++)
            {
                int tileId = GetIdUsingPerlin(x, y);
                noiseGrid[x].Add(tileId);
                CreateTile(tileId, x, y);
            }
        }
    }

    private int GetIdUsingPerlin(int x, int y)
    {
        float rawPerlin = Mathf.PerlinNoise((x - x_offset) / magnification, (y - y_offset) / magnification);
        float clampedPerlin = Mathf.Clamp(rawPerlin, 0.0f, 1.0f);
        clampedPerlin *= (tileSet.Count);

        return Mathf.FloorToInt(clampedPerlin);
    }

    private void CreateTile(int tileId, int x, int y)
    {
        GameObject floorTile = Instantiate(emptyTile, floorTiles.transform);
        floorTile.transform.localPosition = new Vector3(x, 0, y);
        if (tileId != 0)
        {
            Debug.Log("creatred tile");

            GameObject tilePrefab = tileSet[tileId];
            GameObject tileGroup = tileGroups[tileId];
            GameObject tile = Instantiate(tilePrefab, tileGroup.transform);

            tile.name = string.Format("tile_x{0}_y{1}", x, y);
            tile.transform.localPosition = new Vector3(x, 0, y);

            tileGrid[x].Add(tile);
        }
    }

}

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
//OMG ALS JE HIERMEE VERDER GAAT IN JE EIGEN TIJD, PLS SPLIT DIT TUSSEN EEN "GENERATION" CODE EN "WORLD BEHAVIOR" CODE
public class TerrainGenerator : NetworkBehaviour
{
    [SerializeField] public GameObject itemPrefab;

    public static TerrainGenerator Instance { get; private set; }
    //you are making the terrainData networklist functional
    //for this  you needed to make a function that queues changes (at the bottom)
    //these changes get checked and performed r mthng by some timer just like in authorative player
    //those changess should get procesed by all games
    //also games that join should copy the current terrainData en spawn de corresponding objects
    List<List<int>> terrainIds = new List<List<int>>();
    NetworkList<int> terrainData = new NetworkList<int>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    NetworkList<Vector4> terrainDamages = new NetworkList<Vector4>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    List<List<GameObject>> terrainObjects = new List<List<GameObject>>();
    List<List<TerrainObjectData>> terrainObjectDatas = new List<List<TerrainObjectData>>();
    Dictionary<int, GameObject> tileGroups;


    [SerializeField] ObjectIds objectIds;

    [SerializeField] private int map_width = 16;
    [SerializeField] private int map_height = 9;
    [SerializeField] private float magnification = 5;

    [SerializeField] Item testItem;
    private struct ObjectChange
    {
        Vector2Int position;
        int id;
        public ObjectChange(Vector2Int pos, int i)
        {
            position = pos;
            id = i;
        }
    }
    Queue<ObjectChange> objectChanges = new Queue<ObjectChange>();

    public override void OnNetworkSpawn()
    {
        Instance = this;
        objectIds = FindFirstObjectByType<ObjectIds>();
        if (objectIds == null)
            return;
        CreateTileGroups();
        GenerateObjectList();
        if (IsHost)
        {

            GenerateTerrainIds();
            terrainData.Clear();

            for (int x = 0; x < map_width; x++)
            {
                for (int y = 0; y < map_height; y++)
                {
                    terrainData.Add(terrainIds[x][y]);
                }
            }
            //terrainIds = ImportTerrain(terrainData);
        }
        else
        {
            // Client joining late
            if (terrainData.Count > 0)
            {
                // Rebuild terrainIds from flat list
                terrainIds = new List<List<int>>();
                int index = 0;

                for (int x = 0; x < map_width; x++)
                {
                    terrainIds.Add(new List<int>());
                    for (int y = 0; y < map_height; y++)
                    {
                        terrainIds[x].Add(terrainData[index++]);
                    }
                }
            }
        }
        terrainData.OnListChanged += HandleTerrainChange;
        terrainDamages.OnListChanged += HandleTerrainDamage;

        GenerateTerrain();

        SpawnItem(new Vector3(5, 0, 0), testItem.id, 1);
        SpawnItem(new Vector3(6, 0, 0), testItem.id, 1);
        SpawnItem(new Vector3(4, 0, 0), testItem.id, 1);
    }


    private void CreateTileGroups()
    {
        //floorTiles = new GameObject("floor");
        tileGroups = new Dictionary<int, GameObject>();
        for (int i = 1; i <= objectIds.GetObjectAmount(); i++)
        {
            GameObject tileGroup = new GameObject(objectIds.GetObjectById(i).name + " Id: " + i);
            tileGroup.transform.parent = transform;
            tileGroup.transform.localPosition = Vector3.zero;
            tileGroups.Add(i, tileGroup);
        }
    }
    private void GenerateObjectList()
    {
        for (int x = 0; x < map_width; x++)
        {
            terrainObjects.Add(new List<GameObject>());
            terrainObjectDatas.Add(new List<TerrainObjectData>());
        }
    }
    private void GenerateTerrainIds()
    {
        List<List<bool>> treeBools = GenerateBoolList(map_width, map_height, 0, 0, magnification * 3, .5f);
        List<List<bool>> flowerBools = GenerateBoolList(map_width, map_height, map_height * 3, map_width * 3, magnification, .2f);
        List<List<bool>> rockBools = GenerateBoolList(map_width, map_height, map_height, map_width, magnification * 2, .2f);
        List<List<bool>> barrelFruitsBools = GenerateBoolList(map_width, map_height, map_height * 2, map_width * 2, magnification/2, .3f);

        for (int x = 0; x < map_width; x++)
        {
            terrainIds.Add(new List<int>());
            for (int y = 0; y < map_height; y++)
            {
                int tileId = 0;
                if (flowerBools[x][y])
                {
                    tileId = 4;
                }
                if (treeBools[x][y])
                {
                    tileId = 1;
                }
                if (rockBools[x][y])
                {
                    tileId = 2;
                }
                if (barrelFruitsBools[x][y])
                {
                    tileId = 3;
                }
                if (tileId > 0)
                {

                }


                //GameObject floorTile = Instantiate(emptyTile, floorTiles.transform);
                //floorTile.transform.localPosition = new Vector3(x, 0, y);
                terrainIds[x].Add(tileId);
            }
        }
    }

    private void GenerateTerrain()
    {
        for (int x = 0; x < terrainIds.Count; x++)
        {
            for (int y = 0; y < terrainIds[0].Count; y++)
            {
                CreateTile(terrainIds[x][y], x, y);
            }
        }
    }

    private List<List<bool>> GenerateBoolList(int width, int height, float xOffset, float yOffset, float magnification, float acceptenceRate)
    {
        List<List<bool>> boolList = new List<List<bool>>();

        for (int x = 0; x < width; x++)
        {
            boolList.Add(new List<bool>());
            for (int y = 0; y < height; y++)
            {
                boolList[x].Add(PlaceTile(acceptenceRate, (x + xOffset) / magnification, (y + yOffset) / magnification));
            }
        }

        return boolList;
    }

    private void CreateTile(int tileId, int x, int y)
    {

        if (tileId != 0)
        {
            GameObject tilePrefab = objectIds.GetObjectById(tileId);
            GameObject tile = Instantiate(tilePrefab, tileGroups[tileId].transform);

            tile.name = string.Format("tile_x{0}_y{1}", x, y);
            tile.transform.localPosition = new Vector3(x, 0, y);

            terrainObjects[x].Add(tile);
            TerrainObjectData objectData = tile.GetComponent<TerrainObjectData>();
            if (objectData == null)
            {
                Debug.LogWarning("no terrainObjectData found on object: " + tile.name);
                terrainObjectDatas[x].Add(null);
            }
            else
            {
                terrainObjectDatas[x].Add(objectData);
            }
        }
        else
        {
            terrainObjects[x].Add(null);
            terrainObjectDatas[x].Add(null);
        }
    }
    private bool PlaceTile(float acceptenceRate, float x, float y)
    {
        return (acceptenceRate > Mathf.PerlinNoise(x, y));
    }
    private int GetIdUsingPerlin(int x, int y)
    {
        throw new NotImplementedException();
    }

    private void Update()
    {
        if (IsHost)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ChangeTile(1, 1, 4);
            }
        }
    }
    public bool HasTile(int x, int y)
    {
        if (CheckTileInRange(x, y))
            Debug.Log("in HasTile");
        return (terrainObjectDatas[x][y] != null);
    }
    public void ChangeTile(int x, int y, int newId)
    {
        if (x >= 0 && y >= 0 && x < map_width && y < map_height)
        {
            //Debug.Log("changed tile at" + x + ", " + y + " Id:" + newId);
            int index = x * map_height + y;
            terrainData[index] = newId;   // this automatically syncs to all clients
            Debug.Log(index);
        }
        else
        {
            Debug.LogWarning("attempted change outside borders");
        }
    }
    public void HitTile(int x, int y, float damage, int typeOfDamage)
    {
        if (CheckTileInRange(x, y))
        {
            Debug.LogWarning("in HitTile()");
            return;
        }
        terrainDamages.Add(new Vector4(x, y, damage, typeOfDamage));
    }

    //bleh functions
    private void UpdateTileDamage(int x, int y, float damage, int damageType)
    {
        if (CheckTileInRange(x, y))
        {
            Debug.LogWarning("in UpdateTileDamage()");
            return;
        }

        if (terrainObjectDatas[x][y] == null) //above this is where you can apply damage to things on top of this field
            return;
        if (IsServer)
        {
            List<Item> itemsToDrop = terrainObjectDatas[x][y].CheckHitDrops(damageType);
            foreach(Item item in itemsToDrop)
            {
                SpawnItem(new Vector3(x, 0.5f, y), item.id, 1);
            }
        }

        if (terrainObjectDatas[x][y].TakeDamage(damage, damageType))
        {
            List<Item> itemsToDrop = terrainObjectDatas[x][y].CheckDestroyDrops(damageType);
            foreach (Item item in itemsToDrop)
            {
                SpawnItem(new Vector3(x, 0.5f, y), item.id, 1);
            }
            ChangeTile(x, y, 0);
        }


    }

    private void UpdateTile(int x, int y, int newId)
    {
        terrainObjectDatas[x][y] = null;
        GameObject oldTile = terrainObjects[x][y];
        if (oldTile != null)
        {
            //Debug.Log("destroying" + oldTile.name); // hier  moet je de tileObjectData ook verwijderen :: wdym??? dat gebeurd toch al want nieuwe players kunnen joinen terwijl de changes blijven
            Destroy(oldTile);
        }
        else
        {
            Debug.Log("oldtile was null");
        }
        terrainObjects[x][y] = null;

        if (newId != 0)
        {
            GameObject newTile = Instantiate(objectIds.GetObjectById(newId), tileGroups[newId].transform);

            newTile.transform.position = new Vector3(x, 0, y);
            terrainObjects[x][y] = newTile;
            TerrainObjectData objectData = newTile.GetComponent<TerrainObjectData>();
            if (objectData != null)
            {
                terrainObjectDatas[x][y] = objectData;
            }
            else
            {
                Debug.LogWarning("no terrainObjectData found on: " + newTile.name);
            }
        }
        terrainIds[x][y] = newId;
    }
    private void HandleTerrainDamage(NetworkListEvent<Vector4> change)
    {
        Vector4 damageData = change.Value;

        int x = (int)damageData.x;
        int y = (int)damageData.y;
        float damage = damageData.z;
        int typeOfDamage = (int)damageData.w;

        UpdateTileDamage(x, y, damage, typeOfDamage);
    }
    private void HandleTerrainChange(NetworkListEvent<int> change)
    {
        if (change.Type == NetworkListEvent<int>.EventType.Value)
        {
            int index = change.Index;

            int x = index / map_height;
            int y = index % map_height;

            UpdateTile(x, y, change.Value);
            //Debug.Log(index); HIER WAS EEN DEBUG LOG?HIER WAS EEN DEBUG LOG?HIER WAS EEN DEBUG LOG?HIER WAS EEN DEBUG LOG?HIER WAS EEN DEBUG LOG?HIER WAS EEN DEBUG LOG?
        }
        else if (change.Type == NetworkListEvent<int>.EventType.Add) //this happens on initial load, or when terrain expands
        {
            
        }
    }

    private bool CheckTileInRange(int x, int y)
    {
        if (!(x >= 0 && y >= 0 && x < map_width && y < map_height))
        {
            Debug.LogWarning("attempted change outside borders");
            return true;
        }
        return false;
    }

    //[ClientRpc]
    int objectNumber = 0;
    private void SpawnItem(Vector3 position, int itemId, int amount)
    {
        if (!IsServer)
            return;
        GameObject spawneditem = Instantiate(itemPrefab, position, quaternion.identity);
        NetworkObject netObj = spawneditem.GetComponent<NetworkObject>();

        Item item = ItemIds.Instance.GetItemById(itemId);
        spawneditem.name = item.itemName;
        //add information to the item
        PickupableItem itemReference = spawneditem.GetComponent<PickupableItem>();
        itemReference.item = item;
        itemReference.priority = objectNumber++;
        itemReference.amount = amount;

        netObj.Spawn();


    }
}

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;



public class ObjectIds : MonoBehaviour 
{
    public static ObjectIds Instance;
    [SerializeField] List<ObjectId> objectIds = new List<ObjectId>();
    Dictionary<int, GameObject> activeObjectIds = new Dictionary<int, GameObject>();
    Dictionary<GameObject, int> activeObjects = new Dictionary<GameObject, int>();
    private void Awake()
    {
        foreach(ObjectId item in objectIds)
        {
            activeObjectIds.Add(item.id, item.obj);
            activeObjects.Add(item.obj, item.id);
        }
        Instance = this;
    }
    public GameObject GetObjectById(int id)
    {
        return activeObjectIds[id];
    }
    public int GetIdByObject(GameObject obj)
    {
        return activeObjects[obj];
    }
    public int GetObjectAmount()
    {
        return activeObjectIds.Count;
    }
}

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;



public class ObjectIds : MonoBehaviour 
{
    [SerializeField] List<ObjectId> objectIds = new List<ObjectId>();
    Dictionary<int, GameObject> activeObjectIds = new Dictionary<int, GameObject>();
    private void Awake()
    {
        foreach(ObjectId item in objectIds)
        {
            activeObjectIds.Add(item.id, item.obj);
        }
    }
    public GameObject GetObjectById(int id)
    {
        return activeObjectIds[id];
    }
    public int GetObjectAmount()
    {
        return activeObjectIds.Count;
    }
}

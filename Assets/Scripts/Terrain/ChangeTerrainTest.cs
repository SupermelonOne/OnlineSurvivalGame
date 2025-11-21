using UnityEngine;

public class ChangeTerrainTest : MonoBehaviour
{
    public void ChangeTerrainFunc()
    {
        TerrainGenerator.Instance.ChangeTile(3, 3, 5);
        Debug.Log("did thing");
    }
}

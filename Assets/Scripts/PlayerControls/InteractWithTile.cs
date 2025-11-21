using UnityEngine;

public class InteractWithTile : MonoBehaviour
{
    [SerializeField] private Transform player;

    public void DestroyTile()
    {
        if (player == null)
            return;
        RaycastHit hit;
        if (Physics.Raycast(player.position, transform.position - player.position, out hit, Vector3.Distance(transform.position, player.position)))
        {
            int x = Mathf.FloorToInt(hit.point.x + .5f);
            int y = Mathf.FloorToInt(hit.point.z + .5f);
            if (TerrainGenerator.Instance.HasTile(x, y))
            {
                TerrainGenerator.Instance.ChangeTile(x, y, 0);
            }
            else
            {
                x = Mathf.FloorToInt(hit.collider.transform.position.x + .5f);
                y = Mathf.FloorToInt(hit.collider.transform.position.z + .5f);
                TerrainGenerator.Instance.ChangeTile(x, y, 0);
            }
        }

    }
    public void DamageTile()
    {
        if (player == null)
            return;
        RaycastHit hit;
        if (Physics.Raycast(player.position, transform.position - player.position, out hit, Vector3.Distance(transform.position, player.position)))
        {
            int x = Mathf.FloorToInt(hit.point.x + .5f);
            int y = Mathf.FloorToInt(hit.point.z + .5f);
            if (!TerrainGenerator.Instance.HasTile(x, y))
            {
                x = Mathf.FloorToInt(hit.collider.transform.position.x + .5f);
                y = Mathf.FloorToInt(hit.collider.transform.position.z + .5f);
            }
            TerrainGenerator.Instance.HitTile(x, y, 1, 0);

        }
        else
        {
            int x = Mathf.FloorToInt(transform.position.x + .5f);
            int y = Mathf.FloorToInt(transform.position.z + .5f);
            TerrainGenerator.Instance.HitTile(x, y, 1, 0);
        }


    }

    public void PlaceTile()
    {
        Vector3 direction = (transform.position - player.position).normalized;
        Vector3 targetPos = player.position + (direction * .4f);
        int x = Mathf.FloorToInt(player.transform.position.x + .5f);
        int y = Mathf.FloorToInt(player.transform.position.z + .5f);
        if (!TerrainGenerator.Instance.HasTile(x,y))
            TerrainGenerator.Instance.ChangeTile(x, y, 5);
    }
}

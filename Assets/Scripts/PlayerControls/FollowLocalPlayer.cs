using TMPro;
using UnityEngine;

public class FollowLocalPlayer : MonoBehaviour
{
    Vector3 offset;
    Transform player;
    [SerializeField] private float moveSpeed = 1;
    public void SetPlayer(Transform player)
    {
        this.player = player;
        offset = transform.position - player.position;
    }
    private int firstFrames = 5;
    void Update()
    {
        if (player == null)
            return;
        Vector3 targetPosition = player.position + offset;
        Vector3 direction = targetPosition - transform.position;
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
        if (firstFrames > 0)
        {
            firstFrames--;
            transform.position = targetPosition;
        }
    }
}

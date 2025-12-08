using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;

    private void Update()
    {
        rectTransform.position = Input.mousePosition;
    }
}

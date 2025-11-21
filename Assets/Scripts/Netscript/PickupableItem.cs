using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PickupableItem : NetworkBehaviour
{
    [SerializeField] private Item item;
    //[SerializeField] private RawImage rawImage;
    MeshRenderer[] materials;
    private void Start()
    {
        materials = GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer meshRenderer in materials)
        {
            meshRenderer.material = item.displaySprite;
        }
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, 90, 0) * Time.deltaTime);
    }
}

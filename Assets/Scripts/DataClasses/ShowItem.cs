using UnityEngine;
using UnityEngine.UI;

public class ShowItem : MonoBehaviour
{
    [SerializeField] private PlayerInventory myInventory;
    [SerializeField] public int myIndex;
    [SerializeField] private RawImage myRawImage;
    [SerializeField] private Item myItem;
    [SerializeField] private Texture emptyTexture;

    [SerializeField] private bool updateEveryClick;

    private void Update()
    {
        if (!updateEveryClick)
            return;
        if (Input.GetMouseButtonUp(0))
        {
            UpdateSprite();
        }
    }
    private void OnEnable()
    {
        UpdateSprite();
        Debug.Log("OnEnable activated");
    }
    //call update sprite op OnEnable (voor als inventory geopent word) en na geklikt te worden
    public void UpdateSprite()
    {
        if (myItem != myInventory.localItems[myIndex])
            myItem = myInventory.localItems[myIndex];
        if (myItem == null)
        {
            myRawImage.texture = emptyTexture;
                Debug.Log("myItem was null");
        }
        else
        {
            myRawImage.texture = myItem.displayTexture;
                Debug.Log("should be done?");
        }
    }
}
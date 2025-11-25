using UnityEngine;
using UnityEngine.UI;

public class ShowItem : MonoBehaviour
{
    [SerializeField] private PlayerInventory myInventory;
    [SerializeField] private int myIndex;
    [SerializeField] private RawImage myRawImage;
    [SerializeField] private Item myItem;
    

    private void Update()
    {
        if (myInventory.localItems[myIndex] != myItem)
        {

        }
    }
    private void UpdateSprite()
    {
        if (myItem == null)
        {
            myRawImage.texture = 
        }
    }
}
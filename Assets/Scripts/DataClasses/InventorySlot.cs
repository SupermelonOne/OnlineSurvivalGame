using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int myIndex;

    [SerializeField] PlayerInventory myInventory;

    [SerializeField] private ShowItem showItem;
    private void Start()
    {
        if (showItem != null)
        {
            showItem.myIndex = myIndex;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("clicked");
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            myInventory.TakeFullSlot(myIndex);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            //myInventory.TakeHalfSlot(mySlot);
        }
        if (showItem != null)
        {
            showItem.UpdateSprite();
        }
    }
}

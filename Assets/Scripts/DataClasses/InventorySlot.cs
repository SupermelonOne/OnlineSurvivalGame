using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int mySlot;

    [SerializeField] PlayerInventory myInventory;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("clicked");
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            myInventory.TakeFullSlot(mySlot);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            //myInventory.TakeHalfSlot(mySlot);
        }
    }
}

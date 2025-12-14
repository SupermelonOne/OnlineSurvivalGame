using UnityEngine;
using UnityEngine.EventSystems;

public class EquipItemClickable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] EquipItem equipitem;
    public void OnPointerClick(PointerEventData eventData) //make this online for security meassures
    {
        if (equipitem != null)
            equipitem.EquipServerRpc();
    }
}

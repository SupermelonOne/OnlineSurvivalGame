using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int myIndex;

    [SerializeField] PlayerInventory myInventory;

    [SerializeField] private ShowItem showItem;

    private float slotSize = 1;
    private bool hover = false;

    RectTransform rectTransform;

    private void Start()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
    }
    private void Awake()
    {
        if (showItem != null)
        {
            showItem.myIndex = myIndex;
        }
    }
    private void OnEnable()
    {
        hover = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("clicked");
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
    public void OnPointerEnter(PointerEventData eventData)
    {
        hover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hover = false;

    }
    private void Update()
    {
        if (rectTransform == null)
            return;
        if (hover && slotSize < 1.1f)
        {
            slotSize += Time.deltaTime;
        }
        else if (hover)
        {
            slotSize = 1.1f;
        }
        else if (slotSize > 1f)
        {
            slotSize -= Time.deltaTime;
        }
        else
        {
            slotSize = 1;
        }
        if (rectTransform.localScale.x != slotSize)
            rectTransform.localScale = new Vector3(slotSize, slotSize, slotSize);
    }
}

using SA.EventBusSystem;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowItem : MonoBehaviour
{
    [SerializeField] private PlayerInventory myInventory;
    [SerializeField] public int myIndex;
    [SerializeField] private RawImage myRawImage;
    [SerializeField] private Item myItem;
    [SerializeField] private Texture emptyTexture;
    [SerializeField] private TextMeshProUGUI itemAmountText;

    [SerializeField] private bool updateEveryClick;
    private void Start()
    {
        OnEnable();
    }

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
        InventoryEventBus<ItemCollected>.OnEvent += HandleItemCollected;
    }
    private void OnDisable()
    {
        InventoryEventBus<ItemCollected>.OnEvent -= HandleItemCollected;
    }
    //call update sprite op OnEnable (voor als inventory geopent word) en na geklikt te worden
    public void UpdateSprite()
    {
        if (myItem != myInventory.localItems[myIndex])
            myItem = myInventory.localItems[myIndex];
        if (myItem == null)
        {
            myRawImage.texture = emptyTexture;
        }
        else
        {
            myRawImage.texture = myItem.displayTexture;
        }
        if (itemAmountText == null)
            return;
        if (myInventory.localAmounts[myIndex] == 0 || myInventory.localAmounts[myIndex] == 1)
        {
            itemAmountText.text = "";
        }
        else
        {
            itemAmountText.text = myInventory.localAmounts[myIndex].ToString();
        }
    }
    private void HandleItemCollected(ItemCollected eventData)
    {
        if (eventData.index == myIndex)
        {
            UpdateSprite();
        }
    }
}
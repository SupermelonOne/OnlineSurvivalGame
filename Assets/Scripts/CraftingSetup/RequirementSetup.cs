using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RequirementSetup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI amountText;
    [SerializeField] RawImage itemImage;

    public void SetTexture(Texture texture)
    {
        if (itemImage == null)
            return;
        itemImage.texture = texture;
    }
    public void SetText(string text)
    {
        if (amountText == null)
            return;
        amountText.text = text;
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickupPopup : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TMP_Text itemName;
    [SerializeField] TMP_Text amount;

    public void SetupPopup(ItemData data)
    {
        image.sprite = data.itemIcon;
        itemName.text = data.itemName;
        if (data is ResourceData rd)
        {
            amount.text = "x" + rd.amount.ToString();
        }
        else
        {
            amount.text = "x1";
        }
    }
}

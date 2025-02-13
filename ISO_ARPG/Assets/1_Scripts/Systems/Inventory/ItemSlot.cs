using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public CellIndex index;
    public Image img;
    public TMP_Text temp;
    public ItemData item;
    public bool HasItem { get { return item != null; } }
}

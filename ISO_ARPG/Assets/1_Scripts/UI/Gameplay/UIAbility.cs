using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAbility : MonoBehaviour
{
    public Image abImage;
    public TMP_Text abName;
    public TMP_Text description;
    public TMP_Text cost;
    public TMP_Text cooldown;
    public Slider cd_slider;
    public void LoadFromAbility(Ability toLoad)
    {
        Debug.Log("[HUD]: Loading " + toLoad.name);
        if (abImage != null)
            abImage.sprite = toLoad.Icon;
        if (abName != null)
            abName.text = toLoad.Name;
        if (description != null)
            description.text = toLoad.Description;
        if (cost != null)
            cost.text = toLoad.Cost.ToString();
        if (cooldown != null)
            cooldown.text = toLoad.Cooldown.ToString();

        cd_slider.maxValue = toLoad.Cooldown;
        cd_slider.value = 0;
    }
}

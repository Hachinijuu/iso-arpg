using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using TMPro;

public class VideoSettings : MonoBehaviour
{
    //public RenderPipelineAsset[] qualityLevels;
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown resolutionDropdown;
    void Start()
    {
        Debug.Log(QualitySettings.count);
        qualityDropdown.value = QualitySettings.GetQualityLevel();
    }
    public void SetQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(level);
        Debug.Log("Set Quality level to: " + QualitySettings.GetQualityLevel());
        //QualitySettings.renderPipeline = qualityLevels[level];
    }

}

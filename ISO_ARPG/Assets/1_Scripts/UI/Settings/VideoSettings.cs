using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class VideoSettings : MonoBehaviour
{
    //public RenderPipelineAsset[] qualityLevels;
    public Slider qualitySlider;

    void Start()
    {
        Debug.Log(QualitySettings.count);
        qualitySlider.value = QualitySettings.GetQualityLevel();
    }
    public void SetQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(level);
        Debug.Log("Set Quality level to: " + QualitySettings.GetQualityLevel());
        //QualitySettings.renderPipeline = qualityLevels[level];
    }
}

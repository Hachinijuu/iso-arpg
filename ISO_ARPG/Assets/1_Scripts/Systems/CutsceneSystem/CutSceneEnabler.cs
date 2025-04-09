using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneEnabler : MonoBehaviour
{
    [SerializeField]
   public GameObject BerserkerCutscene;
    [SerializeField]
   public GameObject HunterCutscene;
    [SerializeField]
   public GameObject ElementalistCutscene;
    // Start is called before the first frame update
    void Start()
    {
        
     // Check current class
     GoX_Class currentClass = PlayerManager.Instance.currentClass;
        switch (currentClass)
        {
            case GoX_Class.BERSERKER:
                BerserkerCutscene.SetActive(true); break;
               
            case GoX_Class.HUNTER:
               HunterCutscene.SetActive(true); break;
            case GoX_Class.ELEMENTALIST:
                ElementalistCutscene.SetActive(true); break;
        }        
     // Depending on the class enable that Cutscene
        
    }
    

   
}

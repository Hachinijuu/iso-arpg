using UnityEngine;

public class PauseMenu : MonoBehaviour 
{
    public bool CanPause {get { return canPause; } set { canPause = true; } }
    private bool canPause = true;    
}
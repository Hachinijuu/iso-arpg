using UnityEngine;

public class GameplayUIController : MonoBehaviour
{
    // This has references to the hud elements and disable or enable accordingly
    [SerializeField] HUDController hud;
    [SerializeField] GameObject statsScreen;
    [SerializeField] GameObject inventoryScreen;
    [SerializeField] GameObject stashScreen;
    [SerializeField] GameObject smithScreen;

    private void OnEnable()
    {
        TurnOnRelevantElements();
    }

    private void OnDisable()
    {
        ShutoffElements();
    }

    // On load cleanup, chut off character screens....?
    public void ShutoffElements()
    {
        //hud.gameObject.SetActive(false);
        statsScreen.SetActive(false);
        inventoryScreen.SetActive(false);
        stashScreen.SetActive(false);
        smithScreen.SetActive(false);
    }

    public void TurnOnRelevantElements()
    {
        hud.gameObject.SetActive(true);
    }
}

using UnityEngine;

public class optionsScript : MonoBehaviour
{
    public GameObject settingsScreen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void enableSettingsMenu()
    {
        settingsScreen.SetActive(true);
    }
    
}

using UnityEngine;

public class exitMenu : MonoBehaviour
{
    public GameObject settingsScreen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void disableSettingsMenu()
    {
        settingsScreen.SetActive(false);
    }
    
}

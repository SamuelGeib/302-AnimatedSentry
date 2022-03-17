using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUI_MainMenu : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Button Functions:

    // Play
    public void playButton()
    {
        SceneManager.LoadScene("Sentry Level");
    }

    // Settings
    public void settingsButton()
    {
        SceneManager.LoadScene("Sentry Level");
    }

    // Quit
    public void quitButton()
    {
        Application.Quit();
    }
}

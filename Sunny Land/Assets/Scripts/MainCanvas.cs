using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject pausePanel;


    bool gameIsPaused = false;



    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // Pause the game
    public void Pause()
    {
        gameIsPaused = true;
        Time.timeScale = 0f;
        mainPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    // Resume the game
    public void Resume()
    {
        gameIsPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void OpenMainMenu()
    {
        gameIsPaused = false;
        Time.timeScale = 1f;
        GameManager.LoadScene(StaticInfo.mainmenuScene);
    }

   
}

using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    private bool gameIsPaused = false;

    public GameObject mainPanel;
    public GameObject pausePanel;


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

    public void Pause()
    {
        gameIsPaused = true;
        Time.timeScale = 0f;
        mainPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void OpenMainMenu()
    {
        gameIsPaused = false;
        Time.timeScale = 1f;
        GameManager.LoadScene(StaticInfo.mainmenuScene);
    }

    public void Resume()
    {
        gameIsPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        mainPanel.SetActive(true);
    }
}

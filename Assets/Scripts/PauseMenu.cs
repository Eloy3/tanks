using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject Panel;
    public GameObject PausePanel;
    public GameObject GameOverScreen;
    public GameObject LevelWonScreen;
    public GameObject StartScreen;
    private bool isPaused = false;

    void Start()
    {
        Time.timeScale = 0;
        isPaused = true;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                Continue();
            }
            else
            {
                Pause();
            }
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GameOver(){
        Panel.SetActive(true);
        GameOverScreen.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
    }

    public void GameWon(){
        Panel.SetActive(true);
        LevelWonScreen.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
    }
    public void Play()
    {
        Panel.SetActive(false);
        StartScreen.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }
    public void Pause()
    {
        Panel.SetActive(true);
        PausePanel.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
    }

    public void Continue()
    {
        Panel.SetActive(false);
        PausePanel.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }

    public bool IsPaused()
    {
        return isPaused;
    }


    public void RestartLevel()
    {
        Time.timeScale = 1; // Ensure timeScale is reset before reloading the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }

    public void NextLevel()
    {
        Time.timeScale = 1; // Ensure timeScale is reset before loading the next scene
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more levels to load.");
        }
    }

    public void Quit(){
        Application.Quit();
        Debug.Log("Quit");
    }

}

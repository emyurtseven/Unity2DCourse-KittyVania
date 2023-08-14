using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages pause menu, attached to InGameMenuCanvas object.
/// </summary>
public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] TextMeshProUGUI stageText;


    bool gamePaused;

    private void Start()
    {
        // display stage number on screen while we're at it
        stageText.text = "Stage " + GameManager.Instance.CurrentLevel.ToString();
    }

    private void Update() 
    {
        // pause/unpause game with player input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gamePaused)
            {
                pauseMenu.SetActive(true);
                PauseGame(true);
                gamePaused = true;
            }
            else
            {
                OnResumePressed();
            }
        }
    }

    public void PauseGame(bool isPaused)
    {
        if (isPaused)
        {
            Time.timeScale = 0;            
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// Callback for UI button resume
    /// </summary>
    public void OnResumePressed()
    {
        AudioManager.PlaySfx(AudioClipName.UISelect);
        PauseGame(false);
        gamePaused = false;
        pauseMenu.SetActive(false);
    }

    /// <summary>
    /// Load start scene.
    /// </summary>
    public void OnQuitLevelPressed()
    {
        AudioManager.PlaySfx(AudioClipName.UISelect);
        Time.timeScale = 1;
        AudioManager.StopMusic(0);
        SceneManager.LoadScene(0);
    }
}

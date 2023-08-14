using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartMenuManager : MonoBehaviour
{
    [SerializeField] int firstLevelSceneIndex = 1;

    [Header("Panel object references")]
    [SerializeField] GameObject startMenuCanvas;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject loadFailedPanel;
    [SerializeField] GameObject continueConfirmPanel;

    [Header("Audio controls")]
    [SerializeField] float volume;
    [SerializeField] float menuMusicStartDelay = 1f;
    [SerializeField] float menuMusicFadeInDuration = 1f;

    [Header("Scene object references")]
    [SerializeField] Slider volumeSlider;
    [SerializeField] TextMeshProUGUI volumeValueText;
    [SerializeField] GameObject musicOnButton;
    [SerializeField] GameObject musicOffButton;

    MusicPlayer musicPlayer;

    int loadedLevel;
    int musicOn = 1;

    private void Start() 
    {
        // LoadSavedGame();
        // LoadSoundSettings();

        // volumeSlider.value = AudioListener.volume;
    }

    private void LoadSavedGame()
    {
        // if (PlayerPrefs.HasKey(PlayerPreferenceKeys.SavedLevel))
        // {
        //     GameManager.Instance.CurrentLevel = PlayerPrefs.GetInt(PlayerPreferenceKeys.SavedLevel);
        // }
        // else
        // {
        //     GameManager.Instance.CurrentLevel = 1;
        // }
    }

    private void LoadSoundSettings()
    {
        if (PlayerPrefs.HasKey(PlayerPreferenceKeys.Volume))
        {
            AudioListener.volume = PlayerPrefs.GetFloat(PlayerPreferenceKeys.Volume);
        }
        else
        {
            AudioListener.volume = 1;
        }

        musicOn = PlayerPrefs.GetInt(PlayerPreferenceKeys.MusicOn, 1);

        if (musicOn == 1)
        {
            SetMusicOn();
        }
        else if (musicOn == 0)
        {
            SetMusicOff();
        }
    }


    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeValueText.text = volume.ToString("P0");
    }

    public void SetMusicOn()
    {
        musicPlayer.Mute(false);
        musicOn = 1;
        musicOnButton.GetComponent<Image>().enabled = true;
        musicOffButton.GetComponent<Image>().enabled = false;
        musicOnButton.GetComponent<Button>().interactable = false;
        musicOffButton.GetComponent<Button>().interactable = true;

        if (!musicPlayer.IsPlaying)
        {
            // AudioManager.PlayMusicFadeIn(AudioClipName.StartMenuMusic, DefaultGameValues.MusicMaxVolume,
            //                                  menuMusicFadeInDuration, menuMusicStartDelay);
        }
    }

    public void SetMusicOff()
    {
        musicPlayer.Mute(true);
        musicOn = 0;
        musicOnButton.GetComponent<Image>().enabled = false;
        musicOffButton.GetComponent<Image>().enabled = true;
        musicOnButton.GetComponent<Button>().interactable = true;
        musicOffButton.GetComponent<Button>().interactable = false; 
    }



    public void ApplySoundSettings()
    {
        PlayerPrefs.SetFloat(PlayerPreferenceKeys.Volume, AudioListener.volume);
        PlayerPrefs.SetInt(PlayerPreferenceKeys.MusicOn, musicOn);
    }

    public void OnNewGameConfirmed()
    {
        GameManager.Instance.CurrentLevel = 1;
        AudioManager.PlaySfx(AudioClipName.UISelect);
        SceneManager.LoadScene(1);
    }

    public void OnContinueClicked()
    {
        AudioManager.PlaySfx(AudioClipName.UISelect);

        if (PlayerPrefs.HasKey(PlayerPreferenceKeys.SavedLevel))
        {
            loadedLevel = PlayerPrefs.GetInt(PlayerPreferenceKeys.SavedLevel, 1);
            string template = $"Continue to level {loadedLevel} ?";
            continueConfirmPanel.SetActive(true);
            continueConfirmPanel.GetComponentInChildren<TextMeshProUGUI>().text = template;
        }
        else
        {
            // loadFailedPanel.SetActive(true);
        }
    }

    /// <summary>
    /// This assumes level scene index is the same as level number.
    /// </summary>
    public void OnContinueConfirmed()
    {
        // AudioManager.FadeOutMusic(0, 1f);
        // GameManager.Instance.LevelStartedFromMainMenu = true;
        // GameManager.Instance.CurrentLevel = loadedLevel;
        // StartCoroutine(StartSceneTransition(loadedLevel));
    }

    public void OnQuitClicked()
    {
        AudioManager.PlaySfx(AudioClipName.UISelect);

        Application.Quit();
    }
}

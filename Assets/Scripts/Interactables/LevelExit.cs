using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] float levelLoadDelay = 2f;

    /// <summary>
    /// Starts the level change procedure if player enters the trigger. 
    /// Only player capsule collider is used so the script runs only once.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && 
            other.GetType() == typeof(UnityEngine.CapsuleCollider2D))
        {
            int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            StartCoroutine(LoadLevel(currentLevelIndex + 1));
        }
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        AudioManager.FadeOutMusic(0, 0.1f, 0.5f);
        AudioManager.PlaySfx(AudioClipName.LevelFinished);
        // If current level is the last, do nothing
        if (levelIndex == SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("Next level not implemented");
            SceneManager.LoadScene(0);
            yield break;
        }

        yield return new WaitForSecondsRealtime(levelLoadDelay);
        SceneManager.LoadScene(levelIndex);
    }
}

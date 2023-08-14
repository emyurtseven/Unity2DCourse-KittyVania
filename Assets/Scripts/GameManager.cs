using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject HUDPrefab;

    [Header("Icons")]
    [SerializeField] GameObject heartIcon;
    [SerializeField] Sprite heartIconFull;
    [SerializeField] Sprite heartIconEmpty;

    GameObject heartIconsPanel;
    TextMeshProUGUI coinsAmountText;
    GameObject HUDObject;

    Player player;
    int currentLevel;

    int coinsAmount = 0;

    public static GameManager Instance { get; private set; }
    public int CurrentLevel { get => currentLevel; set => currentLevel = value; }
    public int CoinsAmount { get => coinsAmount; set => coinsAmount = value; }

    void Awake()
    {
        SingletonThisObject();
        AudioManager.Initialize();
    }

    private void InitializeLevel(Scene prevScene, Scene nextScene) 
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            AudioManager.PlayMusicFadeIn(0, AudioClipName.Music, 1f, 3f, 1);
            return;
        }

        AudioManager.FadeInMusic(0, 0.5f, 1f, 0);

        HUDObject = GameObject.FindGameObjectWithTag(GameObjectTags.HUD.ToString());

        if (HUDObject == null)
        {
            HUDObject = Instantiate(HUDPrefab);
        }

        coinsAmountText = HUDObject.transform.Find("Coin Text").GetComponent<TextMeshProUGUI>();
        heartIconsPanel = HUDObject.transform.Find("Lives Panel").gameObject;
        player = GameObject.FindGameObjectWithTag(GameObjectTags.Player.ToString()).GetComponent<Player>();
        coinsAmountText.text = coinsAmount.ToString();
        InstantiateHeartIcons();
    }

    private void SingletonThisObject()
    {
        int instanceCount = GameObject.FindGameObjectsWithTag(GameObjectTags.GameManager.ToString()).Length;

        if (instanceCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            SceneManager.activeSceneChanged += InitializeLevel;            // Subscribe to scene changed event
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void InstantiateHeartIcons()
    {
        heartIconsPanel = GameObject.Find("Lives Panel");

        while (heartIconsPanel.transform.childCount > 0)
        {
            DestroyImmediate(heartIconsPanel.transform.GetChild(0).gameObject);
        }

        for (int i = 0; i < player.MaxHealth; i++)
        {
            if (i < player.MaxHealth)
            {
                heartIcon.GetComponent<Image>().sprite = heartIconFull;
                Instantiate(heartIcon, heartIconsPanel.transform);
            }
        }
    }

    /// <summary>
    /// Draws heart icons in HUD.
    /// </summary>
    public void ShowHeartIcons()
    {
        int i = 0;

        heartIconsPanel = GameObject.Find("Lives Panel");

        foreach (Transform heart in heartIconsPanel.transform)
        {
            if (i < player.Health)
            {
                heart.GetComponent<Image>().sprite = heartIconFull;  // Full version is drawn for non-damaged health
            }
            else
            {
                heart.GetComponent<Image>().sprite = heartIconEmpty;  // Empty ones are drawn if player has received damage
            }

            i++;
        }
    }

    /// <summary>
    /// Plays a particle effect on the lost heart upon damage received.
    /// </summary>
    /// <param name="damage"> Number of hearts to show effect </param>
    public void PlayHeartLostEffect(float damage)
    {
        int healthIndex = player.Health;
        heartIconsPanel = GameObject.Find("Lives Panel");

        for (int i = healthIndex; i < healthIndex + damage; i++)
        {
            if (i < 0)
            {
                continue;
            }


            heartIconsPanel.transform.GetChild(i).GetComponent<ParticleSystem>().Play();
        }
    }

    public void ModifyCoinAmount(int amount)
    {
        coinsAmount += amount;
        coinsAmountText.text = coinsAmount.ToString();
    }

    public void EnablePlayerWeapon()
    {
        player.hasGun = true;
    }

    public void ResetGameSession()
    {

    }
}

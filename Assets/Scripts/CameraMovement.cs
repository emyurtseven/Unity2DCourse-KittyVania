using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    Cinemachine.CinemachineVirtualCamera deathCam;

    Player player;

    bool increasingDutch = true;
    bool animationFinished = false;

    [SerializeField] float rockingSpeed = 0.4f;
    [SerializeField] float rockingLimit = 10f;
    [SerializeField] float animationLength = 1.5f;
    [SerializeField] UnityEngine.Camera mainCamera;

    void Awake()
    {
        deathCam = GameObject.Find("Death Camera").GetComponent<Cinemachine.CinemachineVirtualCamera>();
        player = GameObject.FindGameObjectWithTag(GameObjectTags.Player.ToString()).GetComponent<Player>();
    }

    private void Start() 
    {
        Canvas canvas = GameObject.FindGameObjectWithTag(GameObjectTags.HUD.ToString()).GetComponent<Canvas>();
        canvas.worldCamera = mainCamera;
    }

    void Update()
    {
        // If player is dead, rotate camera back and forth between + - rockingLimit, with rockingSpeed

        if (animationFinished)
        {
            deathCam.m_Lens.Dutch = 0;
            return;
        }

        if (!player.isAlive)
        {
            RockCamera();
            Invoke("StabilizeCamera", animationLength);
        }
    }

    private void StabilizeCamera()
    {
        rockingLimit = 0;
        deathCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0f;
        animationFinished = true;
    }


    private void RockCamera()
    {
        if (deathCam.m_Lens.Dutch < rockingLimit && increasingDutch)
        {
            deathCam.m_Lens.Dutch += rockingSpeed;
        }
        else
        {
            increasingDutch = false;
        }

        if (deathCam.m_Lens.Dutch > -rockingLimit && !increasingDutch)
        {
            deathCam.m_Lens.Dutch -= rockingSpeed;
        }
        else
        {
            increasingDutch = true;
        }
    }
}

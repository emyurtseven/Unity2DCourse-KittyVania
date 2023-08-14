using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    bool isActive = false;
    bool isInteractable = false;

    [SerializeField] Sprite[] sprites;
    [SerializeField] GameObject connectedObject;

    AudioSource audioSource;
    Door connectedDoor;

    Animator doorAnimator;

    private void Start() 
    {
        audioSource = transform.parent.GetComponent<AudioSource>();
        connectedDoor = connectedObject.GetComponent<Door>();
        doorAnimator = connectedObject.GetComponent<Animator>();
    }

    private void Update() 
    {
        PushSwitch();
    }

    private void PushSwitch()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Get if door animation is still playing
            bool animationPlaying = doorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;

            // Only activate the switch if it is interactable and the connected door is not being opened right now
            if (isInteractable && !animationPlaying)
            {
                AudioManager.PlaySfx(AudioClipName.Switch);
                isActive = !isActive;
                connectedDoor.OnSignalReceived();

                if (isActive)
                {
                    GetComponent<SpriteRenderer>().sprite = sprites[1];
                }
                else
                {
                    GetComponent<SpriteRenderer>().sprite = sprites[0];
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        isInteractable = true;
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        isInteractable = false;
    }

}

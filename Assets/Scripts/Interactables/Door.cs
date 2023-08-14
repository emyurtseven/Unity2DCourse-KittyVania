using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    bool isOpen = false;

    Animator animator;
    AudioSource audioSource;

    private void Start() 
    {
        animator = GetComponent<Animator>();
        audioSource = transform.parent.GetComponent<AudioSource>();
    }

    public void OnSignalReceived()
    {
        AudioManager.PlaySfx(AudioClipName.StoneDoorSliding);
        
        if (!isOpen)
        {
            isOpen = true;
            animator.SetTrigger("Open");
            Invoke("DisableCollider", 1f);
        }
        else
        {
            isOpen = false;
            animator.SetTrigger("Close");
            GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    private void DisableCollider()
    {
        GetComponent<BoxCollider2D>().enabled = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    GameManager gameSession;

    private void Start() 
    {
        gameSession = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")
                && other.GetType() == typeof(UnityEngine.CapsuleCollider2D))
        {
            gameSession.EnablePlayerWeapon();
            AudioManager.PlaySfx(AudioClipName.WeaponPickUp);
            Destroy(gameObject);
        }
    }
}


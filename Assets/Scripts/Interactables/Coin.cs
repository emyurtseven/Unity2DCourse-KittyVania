using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] AudioClip coinPickupSfx;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")
                && other.GetType() == typeof(UnityEngine.CapsuleCollider2D))
        {
            FindObjectOfType<GameManager>().ModifyCoinAmount(1);
            AudioManager.PlaySfx(AudioClipName.CoinPickUp);
            Destroy(gameObject);
        }
    }
}

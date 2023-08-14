using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingPad : MonoBehaviour
{
    Animator springAnimator;

    private void Awake() 
    {
        springAnimator = GetComponent<Animator>();
    }
    private void OnCollisionExit2D(Collision2D other) 
    {
        if (other.otherCollider.GetType() == typeof(UnityEngine.CapsuleCollider2D))
        {
            Rigidbody2D playerRigidbody = other.gameObject.GetComponent<Rigidbody2D>();

            springAnimator.speed = playerRigidbody.velocity.y / 11;
            springAnimator.SetTrigger("Bounce");

            if (playerRigidbody.velocity.y > 22f)
            {
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 22f);
            }
            else if (playerRigidbody.velocity.y < 15f)
            {
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 15f);
            }
        }  
    }
}

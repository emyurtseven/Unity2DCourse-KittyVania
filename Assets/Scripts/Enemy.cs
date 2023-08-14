using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float animationSpeedMultiplier = 1f;
    [SerializeField] int damage;
    [SerializeField] float moveDelay = 1f;

    [Header("Death Effects")]
    [SerializeField] GameObject deathParticles;
    [SerializeField] GameObject shatteredBody;

    Rigidbody2D myRigidbody;
    Transform enemyTransform;
    CapsuleCollider2D enemyCapsuleCollider;
    Animator enemyAnimator;

    int direction = 1;      // Used for death effects

    int groundLayerMask;

    bool hasRecentlyAttacked;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        enemyTransform = GetComponent<Transform>();
        enemyCapsuleCollider = GetComponent<CapsuleCollider2D>();
        enemyAnimator = GetComponent<Animator>();
    }

    private void Start() 
    {
        // Get direction from the sprite. Negative sign added because sprites originally faced left, 
        // but the movement is positive to the right.
        direction = - Math.Sign(enemyTransform.localScale.x);    

        groundLayerMask = LayerMask.GetMask("Ground");
        enemyAnimator.speed = animationSpeedMultiplier;
    }

    private void Update() 
    {
        if (!hasRecentlyAttacked)
        {
            myRigidbody.velocity = new Vector2(direction * moveSpeed, 0f);
        }
    }

    /// <summary>
    /// Flip enemy movement direction when its 'reverse periscope' collider exits the tile border.
    /// Check the prefabs for collider implementation.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.gameObject.name == "Platforms Tilemap")
        {
            myRigidbody.velocity = -myRigidbody.velocity;
            FlipEnemyDirection();
        }
    }

    public int DealDamage()
    {
        StartCoroutine(PauseMovement());
        return damage;
    }

    IEnumerator PauseMovement()
    {
        hasRecentlyAttacked = true;
        yield return new WaitForSecondsRealtime(moveDelay);
        hasRecentlyAttacked = false;
    }

    /// <summary>
    /// Flips enemy sprite.
    /// </summary>
    private void FlipEnemyDirection()
    {
        float enemyDirectionX = enemyTransform.localScale.x;
        float enemyDirectionY = enemyTransform.localScale.y;
        float enemyDirectionZ = enemyTransform.localScale.z;

        enemyDirectionX = -enemyDirectionX;
        direction = -direction;

        enemyTransform.localScale = new Vector3(enemyDirectionX, enemyDirectionY, enemyDirectionZ);
    }

    public void DieBloody(float direction)
    {
        EnemyDeath corpse = Instantiate(shatteredBody, transform.localPosition, Quaternion.identity).GetComponent<EnemyDeath>();
        corpse.SetSprites(gameObject.name, direction);
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        AudioManager.PlaySfx(AudioClipName.EnemyDead);
        Destroy(gameObject);
    }
}

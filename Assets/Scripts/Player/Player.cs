using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages EVERYTHING about the player. I wrote this script when I was learning the basics,
/// this probably needs to be 2-3 separate scripts.
/// </summary>
public class Player : MonoBehaviour
{
    Vector2 inputVector;
    Vector2 smoothedInputVector; 
    Vector2 smoothInputVelocity;  // REQUIRED for SmoothDamp function, unused otherwise.

    Rigidbody2D playerRigidbody;
    CapsuleCollider2D playerCapsuleCollider;
    CircleCollider2D playerCircleCollider;
    Transform playerTransform;
    Animator playerAnimator;

    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] GameObject[] projectiles;

    [SerializeField] int maxHealth = 6;
    int currentHealth;

    public int Health {get { return currentHealth; }}
    public int MaxHealth {get { return maxHealth; }}

    [SerializeField] float playerMoveSpeed = 7f;
    [SerializeField] float playerJumpSpeed = 14f;
    [SerializeField] float playerClimbSpeed = 4f;
    [SerializeField] float JumpClimbDelay = 0.8f;
    [SerializeField] float playerDeathAnimLength = 3f;
    [SerializeField] float damageDelay = 0.3f;

    [Tooltip("How strongly player is pushed back when hurt")]
    [SerializeField] float knockbackStrength = 5f;

    [Tooltip("Smaller number means snappier movement, larger means more 'slidy'")]
    [SerializeField] float smoothInputDelay = 0.2f;    

    int groundLayerMask;
    int climbingLayerMask;
    int interactablesLayerMask;
    float defaultGravityScale;
    bool climbingActive = true;
    bool isJumping;
    bool isClimbing;
    bool isInvulnerable;
    [SerializeField] public bool hasGun = false;

    [SerializeField] float gunHolsterDelay = 1f;
    float gunHolsterTimer = 0f;

    public bool isAlive = true;

    private void Awake() 
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerTransform = GetComponent<Transform>();
        playerAnimator = GetComponent<Animator>();
        playerCapsuleCollider = GetComponent<CapsuleCollider2D>();
        playerCircleCollider = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        groundLayerMask = LayerMask.GetMask("Ground");
        climbingLayerMask = LayerMask.GetMask("Climbing");
        interactablesLayerMask = LayerMask.GetMask("Interactables");
        defaultGravityScale = playerRigidbody.gravityScale;
    }

    private void Update()
    {
        Run();
        Climb();
        AnimateGunFiring();
    }

    /// <summary>
    /// Called when button set in input system is pressed.
    /// </summary>
    private void OnFire()
    {   
        if (!hasGun || isClimbing)
        {
            return;
        }

        AudioManager.PlaySfx(AudioClipName.PlayerWeaponFire);
        FireProjectile(projectiles[0]);
        gunHolsterTimer = gunHolsterDelay;
    }

    /// <summary>
    /// Instantiates a projectile.
    /// </summary>
    /// <param name="projectile"></param>
    void FireProjectile(GameObject projectile)
    {
        GameObject newProjectile = Instantiate(projectile, projectileSpawnPoint.position, transform.rotation);
    }

    /// <summary>
    /// Activates the player firing animation for duration = gunHolsterTimer
    /// </summary>
    private void AnimateGunFiring()
    {
        if (gunHolsterTimer > 0)
        {
            gunHolsterTimer -= Time.deltaTime;
            playerAnimator.SetBool("isFiring", true);
        }
        else
        {
            playerAnimator.SetBool("isFiring", false);
        }
    }

    /// <summary>
    /// Traverse vertically on climbable tiles that are set in Climbing Tilemap in editor.
    /// </summary>
    private void Climb()
    {
        // Disable climbing animation if climbing is disabled
        if (!climbingActive)
        {
            playerAnimator.SetBool("isClimbing", false);
            return;
        }

        // If player is not touching climbable object, return
        if (!playerCircleCollider.IsTouchingLayers(climbingLayerMask))
        {
            playerRigidbody.gravityScale = defaultGravityScale;
            isClimbing = false;
            playerAnimator.SetBool("isClimbing", false);
            return;
        }


        playerRigidbody.gravityScale = 0;   // Set gravity to 0 to 'stick' player to ladder

        // Add vertical velocity
        playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, inputVector.y * playerClimbSpeed);

        bool playerHasVerticalSpeed = Mathf.Abs(playerRigidbody.velocity.y) > Mathf.Epsilon;
        isClimbing = playerHasVerticalSpeed;

        // Only activate climbing animation if moving vertically.
        // If holding still on ladder, idle is played
        playerAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
    }

    /// <summary>
    /// Gets called when the Jump input set in input manager is pressed.
    /// </summary>
    /// <param name="value"> Is not used in this case </param>
    private void OnJump(InputValue value)
    {
        if (playerCircleCollider.IsTouchingLayers(groundLayerMask) || 
            playerCircleCollider.IsTouchingLayers(interactablesLayerMask) ||
            playerCircleCollider.IsTouchingLayers(climbingLayerMask))
        {
            Jump();
        }
    }

    /// <summary>
    /// Jumps player into air
    /// </summary>
    private void Jump()
    {
       
        playerRigidbody.velocity += new Vector2(0f, playerJumpSpeed);  // Add vertical speed to players velocity to simulate a jump
        playerAnimator.SetTrigger("OnJump");    // Play jumping animation
        playerRigidbody.gravityScale = defaultGravityScale;    // Return gravity to normal if it was turned off on a ladder

        AudioManager.PlaySfx(AudioClipName.PlayerJump, 0.5f);

        // If on ladder, deactivate climbing momentarily, before reactivating it later
        if (playerRigidbody.IsTouchingLayers(climbingLayerMask))
        {
            climbingActive = false;
            Invoke("ReactivateClimbing", JumpClimbDelay);
        }
    }

    // This is called with a delay if player jumps while on a ladder
    private void ReactivateClimbing()
    {
        climbingActive = true;
    }

    // This is called when movement keys set in input system are pressed
    private void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
        FlipPlayerDirection();
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Hazard"))
        {
            DamagePlayer(other.gameObject, "Hazard");
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            DamagePlayer(other.gameObject, "Enemy");
        }
    }

    private void OnCollisionStay2D(Collision2D other) 
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Hazard"))
        {
            DamagePlayer(other.gameObject, "Hazard");
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            DamagePlayer(other.gameObject, "Enemy");
        }
    }

   /// <summary>
   ///  Damages and knocks player back depending on the collided object.
   /// </summary>
   /// <param name="collidedObject"> GameObject of the collision, passed by OnCollision callbacks. </param>
   /// <param name="damager"> Accepted arguments: "Hazard" or "Enemy". </param>
    private void DamagePlayer(GameObject collidedObject, string damager)
    {
        if (!isInvulnerable && isAlive)
        {
            int damage = 0;
            Vector2 knockbackDirection = Vector2.zero;

            if (damager == "Hazard")
            {
                knockbackDirection = new Vector2(0, 4f);    // *** 4f is hardcoded here, change maybe? ***
                damage = 2;      // *** Why is hazard damage set here?? Maybe move in the future ***
            }
            else if (damager == "Enemy")
            {
                knockbackDirection = (transform.position - collidedObject.transform.position).normalized;
                damage = collidedObject.GetComponent<Enemy>().DealDamage();
            }

            playerRigidbody.AddForce(knockbackDirection * knockbackStrength, ForceMode2D.Impulse);
            AudioManager.PlaySfx(AudioClipName.PlayerHurt);
            currentHealth = currentHealth - damage;
            GameManager.Instance.ShowHeartIcons();
            GameManager.Instance.PlayHeartLostEffect(damage);

            CheckPlayerDeath();
        }
    }

    /// <summary>
    /// After receiving damage, update hearts in HUD and check if health is zero.
    /// </summary>
    private void CheckPlayerDeath()
    {
        if (currentHealth <= 0)
        {
            KillPlayer();
        }
        else
        {
            playerAnimator.SetTrigger("onDamageReceived");      // Trigger damage animation
            StartCoroutine(MakePlayerInvulnerable());       // Start invulnerable frames
        }
    }

    private void KillPlayer()
    {
        AudioManager.PlaySfx(AudioClipName.PlayerDead);
        isAlive = false;
        playerAnimator.SetBool("isDead", true);
        GetComponent<PlayerInput>().DeactivateInput();
        StartCoroutine(RespawnPlayer());
    }

    /// <summary>
    /// Coroutine that resets game session after delay = playerDeathAnimLength.
    /// </summary>
    /// <returns></returns>
    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(playerDeathAnimLength);  // Wait for death animation
        transform.position = GameObject.Find("PlayerSpawnPoint").transform.position;
        currentHealth = maxHealth;

        isAlive = true;
        playerAnimator.SetBool("isDead", false);
        GetComponent<PlayerInput>().ActivateInput();
        GameManager.Instance.ShowHeartIcons();
    }

    /// <summary>
    /// Coroutine that makes the player temp. invulnerable to damage.
    /// </summary>
    /// <returns></returns>
    IEnumerator MakePlayerInvulnerable()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(damageDelay);
        isInvulnerable = false;
    }

    /// <summary>
    /// Moves player horizontally with some smooth acceleration.
    /// </summary>
    private void Run()
    {
        if (!isInvulnerable)
        {
            // Use SmoothDamp to smooth the input value from 0 to 1 or 0 to -1, instead of getting discrete integer values
            smoothedInputVector = Vector2.SmoothDamp(smoothedInputVector, inputVector, ref smoothInputVelocity, smoothInputDelay);
            
            // Clamp the value to 0 if it's below a threshold
            if (Mathf.Abs(smoothedInputVector.x) < 0.01f)
            {
                smoothedInputVector = Vector2.zero;
            }

            Vector2 playerVelocity = new Vector2(smoothedInputVector.x * playerMoveSpeed, playerRigidbody.velocity.y);
            playerRigidbody.velocity = playerVelocity;

            EnableRunningAnimation();
        }
    }

    private void EnableRunningAnimation()
    {
        // Switch into run animation if velocity > 0.1f and touching ground
        if (Mathf.Abs(playerRigidbody.velocity.x) > 0.1f && 
            (
                playerCapsuleCollider.IsTouchingLayers(groundLayerMask) || 
                playerCapsuleCollider.IsTouchingLayers(interactablesLayerMask)
            ))                                                                             
        {
            playerAnimator.SetBool("isRunning", true);
            playerAnimator.speed = Mathf.Abs(smoothedInputVector.x);  // Scale running animation speed with smooth input
        }
        else
        {
            playerAnimator.SetBool("isRunning", false);
            playerAnimator.speed = 1;   // Set animation speed to normal for other clips
        }
    }

    /// <summary>
    /// Flips player sprite when changing direction.
    /// </summary>
    private void FlipPlayerDirection()
    {
        float playerDirectionY = playerTransform.localScale.y;

        // Player localscale transform x equals to sign of movement input in x axis
        if (Mathf.Abs(inputVector.x) > 0)
        {
            playerTransform.localScale = new Vector2(Mathf.Sign(inputVector.x), playerDirectionY);
        }
    }
}

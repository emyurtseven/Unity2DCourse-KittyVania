using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] float bulletSpeed;
    [SerializeField] float bulletRange;
    [SerializeField] float bulletSpin;

    float timer = 0f;
    float direction;

    Rigidbody2D myRigidbody;
    Player player;

    void Start()
    {
        timer = bulletRange;
        player = FindObjectOfType<Player>();
        direction = Mathf.Sign(player.transform.localScale.x);
        myRigidbody = GetComponent<Rigidbody2D>();
        myRigidbody.velocity = new Vector2(direction * bulletSpeed, 0f);   // Add velocity in players direction
        myRigidbody.rotation = 1f;
    }

    private void Update() 
    {
        // Rotates bullet for visual effect
        transform.Rotate(Vector3.forward * -bulletSpin * Time.deltaTime);

        if (timer > 0)
        {
            timer -= 2f * Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            enemy.DieBloody(direction);
            Destroy(gameObject);
        }

        Destroy(gameObject);
    }

}

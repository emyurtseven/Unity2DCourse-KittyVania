using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    [SerializeField] float persistTime = 2f;
    [SerializeField] Sprite[] mushroomSprites;
    [SerializeField] Sprite[] batSprites;
    [SerializeField] Sprite[] fishSprites;
    [SerializeField] float maxPieceVelocity_X = 2f;
    [SerializeField] float maxPieceVelocity_Y = 2f;


    void Update()
    {
        if (persistTime > 0)
        {
            persistTime -= 1 * Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSprites(string name, float direction)
    {
        Sprite[] sprites;

        if (name.Contains("Mushroom (Enemy)"))
        {
            sprites = mushroomSprites;
        }
        else if (name.Contains("Bat (Enemy)"))
        {
            sprites = batSprites;
        }
        else if (name.Contains("Fish (Enemy)"))
        {
            sprites = fishSprites;
        }
        else
        {
            return;
        }

        int i = 0;
        foreach (Transform child in transform)
        {
            child.GetComponent<SpriteRenderer>().sprite = sprites[i];
            float pieceVelocity_X = Random.Range(direction * 2, direction * maxPieceVelocity_X);
            float pieceVelocity_Y = Random.Range(1, maxPieceVelocity_Y);

            child.GetComponent<Rigidbody2D>().velocity = new Vector2(pieceVelocity_X, pieceVelocity_Y);
            i++;
        }
    }

    private void DeactivateCollision()
    {
        int i = 0;
        foreach (Transform child in transform)
        {
            child.GetComponent<CircleCollider2D>().enabled = false;
            i++;
        }
    }
}

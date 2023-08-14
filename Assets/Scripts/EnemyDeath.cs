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


    // Update is called once per frame
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
        if (name.Contains("Mushroom (Enemy)"))
        {
            int i = 0;
            foreach (Transform child in transform)
            {
                child.GetComponent<SpriteRenderer>().sprite = mushroomSprites[i];
                float pieceVelocity_X = Random.Range(direction * 2, direction * maxPieceVelocity_X);
                float pieceVelocity_Y = Random.Range(1, maxPieceVelocity_Y);

                child.GetComponent<Rigidbody2D>().velocity = new Vector2(pieceVelocity_X, pieceVelocity_Y);
                i++;
            }
        }
        else if (name.Contains("Bat (Enemy)"))
        {
            int i = 0;
            foreach (Transform child in transform)
            {
                child.GetComponent<SpriteRenderer>().sprite = batSprites[i];
                float pieceVelocity_X = Random.Range(direction * 2, direction * maxPieceVelocity_X);
                float pieceVelocity_Y = Random.Range(1,  maxPieceVelocity_Y);

                child.GetComponent<Rigidbody2D>().velocity = new Vector2(pieceVelocity_X, pieceVelocity_Y);
                i++;
            }
        }
        else if (name.Contains("Fish (Enemy)"))
        {
            int i = 0;
            foreach (Transform child in transform)
            {
                child.GetComponent<SpriteRenderer>().sprite = fishSprites[i];
                float pieceVelocity_X = Random.Range(direction * 2, direction * maxPieceVelocity_X);
                float pieceVelocity_Y = Random.Range(1, maxPieceVelocity_Y);

                child.GetComponent<Rigidbody2D>().velocity = new Vector2(pieceVelocity_X, pieceVelocity_Y);
                i++;
            }
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

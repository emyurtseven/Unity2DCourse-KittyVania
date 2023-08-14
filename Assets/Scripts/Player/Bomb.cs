using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    // [SerializeField] float timer = 2f;

    CircleCollider2D myCollider;
    Rigidbody2D myRigidbody;

    void Start()
    {
        myCollider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

}


using UnityEngine;
using System;
using System.Collections;


public class ObjectScroll : MonoBehaviour // Added missing opening brace
{
    public float speed = 2.0f;

    Rigidbody2D rb;
    BoxCollider2D bc;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Ensure this is inside a MonoBehaviour class
        bc = GetComponent<BoxCollider2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public float GetXExtent() => bc.bounds.extents.x;

    public Vector2 GetTopLeftCorner() => new Vector2(bc.bounds.min.x, bc.bounds.max.y);

    public Vector2 GetBottomLeftCorner() => new Vector2(bc.bounds.min.x, bc.bounds.min.y);

    public Vector2 GetTopRightCorner() => new Vector2(bc.bounds.max.x, bc.bounds.max.y);

    public Vector2 GetBottomRightCorner() => new Vector2(bc.bounds.max.x, bc.bounds.min.y);

    void Update()
    {
        rb.MovePosition(transform.position + (Vector3.left * (speed * Time.deltaTime)));

        if (transform.position.x <= -10) CleanUpPiece();
    }

    void CleanUpPiece()
    {
        // spawn next piece through LevelGen
        // remove this piece from LevelGen list
        // destroy game object
    }
}

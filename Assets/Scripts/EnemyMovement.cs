using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    
    [SerializeField] float moveSpeed = 1f;
    Rigidbody2D rb2d;
    BoxCollider2D periscope;

    private float direction = 1;

    void Awake() {
        rb2d = GetComponent<Rigidbody2D>();
        periscope = GetComponent<BoxCollider2D>();
    }
    
    void Start() {
        
    }

    void Update() {
        rb2d.velocity = new Vector2(moveSpeed * direction, 0f);
    }

    void OnTriggerExit2D(Collider2D other) {
        direction = -direction;
        transform.localScale = new Vector2(direction, 1);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] float bulletSpeed = 1;
    Rigidbody2D rb2d;
    PlayerMovement player;
    float xSpeed;

    void Awake() {
        rb2d = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerMovement>();
        xSpeed =  player.transform.localScale.x * bulletSpeed;
    }
    
    void Start() {
        
    }

    
    void Update() {
        rb2d.velocity = new Vector2(xSpeed, 0);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Enemy") {
            Destroy(other.gameObject);
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        Destroy(gameObject);
    }
}

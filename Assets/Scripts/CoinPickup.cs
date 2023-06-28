using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] AudioClip pickupSound;
    [SerializeField] int coinValue = 100;

    bool wasCollected = false;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player" && !wasCollected) {
            wasCollected = true;
            AudioSource.PlayClipAtPoint(pickupSound, Camera.main.transform.position);
            FindObjectOfType<GameSession>().AddToScore(coinValue);
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}

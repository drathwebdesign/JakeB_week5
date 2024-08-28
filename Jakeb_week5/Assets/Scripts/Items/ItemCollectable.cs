using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollectable : MonoBehaviour {
    public Item item;

    void Start() {
    }
    void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Player")) {
            Destroy(gameObject);
        }
    }
}
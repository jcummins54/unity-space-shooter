using UnityEngine;
using System.Collections;

public class AsteroidMover : MonoBehaviour {

    public float topSpeed;

    private Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();
        Vector3 movement = new Vector3(Random.Range(-0.4f, 0.4f), 0.0f, -1);

        float speed = 3;
        if (topSpeed > 3) {
            speed = Random.Range(3, topSpeed);
        }
        rb.velocity = movement * speed;
    }
}

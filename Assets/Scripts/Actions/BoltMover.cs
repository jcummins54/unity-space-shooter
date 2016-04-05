using UnityEngine;
using System.Collections;

public class BoltMover : MonoBehaviour {
    
    public float speed;

    private Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();

        Vector3 movement = transform.forward;

        rb.velocity = movement * speed;
    }
}

using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate;
    public float delay;

    private AudioSource audioSource;
    private Rigidbody rb;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        InvokeRepeating("Fire", delay, fireRate);

        rb = GetComponent<Rigidbody>();
        Vector3 movement = new Vector3(0.0f, 0.0f, -1);

        float speed = Random.Range(3, 10);
        rb.velocity = movement * speed;
    }

    void Fire() {
        Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
        audioSource.Play();
    }
}

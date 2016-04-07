using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class Boundary {
    public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour {
    public float speed;
    public float tilt;
    public float fireRate;
    public float slowFireRate;
    public float fireSlowdownTime;
    public float fireRecoveryTime;
    public Boundary boundary;
    public GameObject shot;
    public GameObject shotSpawnContainer;
    public Transform[] shotSpawns;

    private Rigidbody rb;
    private float nextFire;
    private float nextFireRefresh;
    private bool isFireRefreshing;
    private UniversalInput input;

    void Start() {
        input = UniversalInput.GetInstance();
        rb = GetComponent<Rigidbody>();
        nextFire = 0.0f;
        nextFireRefresh = Time.time + fireSlowdownTime;
        isFireRefreshing = false;
    }

    void Update() {
        if (input.CanFire() && Time.time > nextFire) {
            if (isFireRefreshing) {
                nextFire = Time.time + slowFireRate;
            }
            else {
                nextFire = Time.time + fireRate;
            }

            for (int i = 0; i < shotSpawns.Length; i++) {
                Transform shotSpawn = shotSpawns[i];
                Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
            }

            GetComponent<AudioSource>().Play();

            if (Time.time > nextFireRefresh) {
                isFireRefreshing = true;
                nextFireRefresh = Time.time + fireRecoveryTime;
            }
        }

        if (!input.CanFire()) {
            if (Time.time > nextFireRefresh) {
                isFireRefreshing = false;
            }
            if (!isFireRefreshing) {
                nextFireRefresh = Time.time + fireSlowdownTime;
            }
        }
    }

    void FixedUpdate() {
        Vector2 direction = input.GetDirection ();
        Vector3 movement = new Vector3 (direction.x, 0.0f, direction.y);

        rb.velocity = movement * speed;

        rb.position = new Vector3(
            Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax), 
            0.0f, 
            Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
        );
        
        rb.rotation = Quaternion.Euler(0.0f, 0.0f, rb.velocity.x * -tilt);
        //Rotate shotSpawns opposite to ship rotation so shots do not leave the Y axis of the Boundary
        shotSpawnContainer.transform.rotation = Quaternion.Euler(rb.rotation.x, rb.rotation.y, -rb.rotation.z);
    }
}
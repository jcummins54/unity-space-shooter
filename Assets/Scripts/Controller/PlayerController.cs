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
    public SimpleTouchPad touchPad;
    public SimpleTouchAreaButton touchButton;

    private Rigidbody rb;
    private float nextFire;
    private float nextFireRefresh;
    private bool isFireRefreshing;
    private Quaternion calibrationQuaternion;

    void Start() {
        rb = GetComponent<Rigidbody>();
        nextFire = 0.0f;
        nextFireRefresh = Time.time + fireSlowdownTime;
        isFireRefreshing = false;

        CalibrateAccelerometer();
    }

    void Update() {
        if (touchButton.CanFire() && Time.time > nextFire) {
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

        if (!touchButton.CanFire()) {
            if (Time.time > nextFireRefresh) {
                isFireRefreshing = false;
            }
            if (!isFireRefreshing) {
                nextFireRefresh = Time.time + fireSlowdownTime;
            }
        }
    }

    void FixedUpdate() {
        /*
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        /*
        Vector3 acceleration = FixAcceleration(Input.acceleration);
        Vector3 movement = new Vector3(acceleration.x, 0.0f, acceleration.y);
        */

        Vector2 direction = touchPad.GetDirection ();
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

    Vector3 FixAcceleration(Vector3 acceleration) {
        Vector3 fixedAcceleration = calibrationQuaternion * acceleration;
        return fixedAcceleration;
    }

    void CalibrateAccelerometer() {
        Vector3 accelerationSnapshot = Input.acceleration;
        Quaternion rotateQuaternion = Quaternion.FromToRotation(new Vector3(0.0f, 0.0f, -1.0f), accelerationSnapshot);
        calibrationQuaternion = Quaternion.Inverse(rotateQuaternion);
    }
}
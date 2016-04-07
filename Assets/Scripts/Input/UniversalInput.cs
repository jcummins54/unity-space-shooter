using UnityEngine;
using System.Collections;

public class UniversalInput : MonoBehaviour {

    private const int KEYBOARD = 1;
    private const int TOUCHPAD = 2;
    private const int ACCELEROMETER = 3;

    private int inputType;

    private SimpleTouchPad joyPad;
    private SimpleTouchAreaButton fireButton;
    private Quaternion calibrationQuaternion;

    private static UniversalInput _instance;

    public static UniversalInput GetInstance() {
        if (_instance != null) {
            return _instance;
        }
        GameObject go = GameObject.Find("UniversalInput");
        if (go != null) {
            _instance = go.GetComponent<UniversalInput>();
            return _instance;
        }
        Debug.Log("Cannot find 'UniversalInput' script");
        return null;
    }

    void Awake() {
        if (_instance == null) {
            DontDestroyOnLoad(gameObject);
            inputType = KEYBOARD;
            _instance = this;
        } else if (_instance != this) {
            Destroy(gameObject);
        }
    }

    void Start() {
        DontDestroyOnLoad(gameObject);
    }

    void OnLevelWasLoaded() {
        joyPad = GameObject.Find("UIContainer").GetComponentInChildren<SimpleTouchPad>();
        fireButton = GameObject.Find("UIContainer").GetComponentInChildren<SimpleTouchAreaButton>();
        CalibrateAccelerometer();
    }

    void Update() {
        if (inputType == KEYBOARD && Input.touchCount > 0) {
            inputType = TOUCHPAD;
        }
    }

    public bool CanFire() {
        switch(inputType) {
            case TOUCHPAD:
                return fireButton.CanFire();
            default:
                return Input.GetButton("Fire1");
        }
    }

    public Vector2 GetDirection() {
        Vector2 direction;
        switch (inputType) {
            case TOUCHPAD:
                direction = joyPad.GetDirection();
                break;
            case ACCELEROMETER:
                Vector3 acceleration = FixAcceleration(Input.acceleration);
                direction = new Vector2(acceleration.x, acceleration.y);
                break;
            default:
                float moveHorizontal = Input.GetAxis("Horizontal");
                float moveVertical = Input.GetAxis("Vertical");
                direction = new Vector2(moveHorizontal, moveVertical);
                break;
        }

        return direction;
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

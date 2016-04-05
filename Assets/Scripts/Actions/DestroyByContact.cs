using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour {

    public GameObject explosion;
    public GameObject playerExplosion;

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Boundary") || (CompareTag("Enemy") && other.CompareTag("Enemy"))) {
            return;
        }

        if (CompareTag("Asteroid") && transform.localScale.x > 0.9) {
            GameController.GetInstance().SpawnAsteroidFragments(transform.position);
        }
        if (explosion != null) {
            Instantiate(explosion, transform.position, transform.rotation);
        }
        if (other.CompareTag("Bolt") || other.CompareTag("Player")) {
            GameController.GetInstance().AddScore(tag);
        }
        Destroy(other.gameObject);
        Destroy(gameObject);
        if (other.CompareTag("Player")) {
            Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
            GameController.GetInstance().GameOver();
        }
    }
}

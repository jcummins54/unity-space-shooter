using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public GameObject[] hazards;
    public Vector3 spawnValues;
    public int hazardCount;
    public float spawnWait;
    public float startWait;
    public float waveWait;

    // Offset to spawn asteroid fragments when large asteroids are destroyed
    public float fragOffset;

    public int scoreAsteroid;
    public int scoreEnemy;

    public Text scoreText;
    public Text restartText;
    public Text gameOverText;
    public Text highScoreText;
    public Text highScoreNames;
    public Text highScoreScores;

    private bool gameOver;
    private bool restart;

    private static GameController _instance;

    public static GameController GetInstance() {
        if (_instance != null) {
            return _instance;
        }
        GameObject go = GameObject.Find("GameController");
        if (go != null) {
            _instance = go.GetComponent<GameController>();
            return _instance;
        }
        Debug.Log("Cannot find 'GameController' script");
        return null;
    }

    public void GameOver() {
        gameOverText.text = "Game Over!";
        gameOver = true;
        DynamoDbObject.GetInstance().SavePlayer();
        UpdateHighScores();
    }

    public void AddScore(string tagName) {
        if (gameOver) {
            return;
        }
        int points = 0;
        if (tagName == "Asteroid") {
            points = scoreAsteroid;
        }
        else if (tagName == "Enemy") {
            points = scoreEnemy;
        }
        GameModel.GetInstance().playerScore += points;
        UpdateScore();
    }

    void Start() {
        gameOver = false;
        restart = false;
        restartText.text = "";
        gameOverText.text = "";
        highScoreNames.text = "";
        highScoreScores.text = "";
        GameModel.GetInstance().playerScore = 0;
        UpdateScore();
        highScoreText.text = GameModel.GetInstance().GetHighScore();
        StartCoroutine(SpawnWaves());
    }

    void Update() {
        if (Input.GetKey(KeyCode.Escape)) {
            Application.Quit();
        }

        if (restart) {
            if (Input.GetButton("Fire1")) {
                SceneManager.LoadScene("01 Start");
            }
        }
    }

    void UpdateScore() {
        scoreText.text = GameModel.GetInstance().playerScore.ToString();
    }

    void UpdateHighScores() {
        GameModel.GetInstance().UpdateHighScores();
        highScoreText.text = GameModel.GetInstance().GetHighScore();
        highScoreNames.text = GameModel.GetInstance().GetHighScoreNames();
        highScoreScores.text = GameModel.GetInstance().GetHighScoreScores();
    }

    public void SpawnAsteroidFragments(Vector3 spawnPosition) {
        float spawnOriginX = spawnPosition.x;
        for (int i = 0; i < 3; i++) {
            Quaternion spawnRotation = Quaternion.identity;
            GameObject hazard = hazards[Random.Range(0, hazards.Length)];
            if (hazard.CompareTag("Enemy")) {
                i--;
                continue;
            }
            spawnPosition.x = spawnOriginX + (i * fragOffset) - fragOffset;
            GameObject clone = Instantiate(hazard, spawnPosition, spawnRotation) as GameObject;
            float asteroidScale = Random.Range(0.2f, 0.7f);
            clone.transform.localScale = new Vector3(asteroidScale, asteroidScale, asteroidScale);
        }
    }

    IEnumerator SpawnWaves() {
        yield return new WaitForSeconds(startWait);
        while (true) {
            if (gameOver) {
                restartText.text = "Fire to Play Again";
                restart = true;
                break;
            }
            for (int i = 0; i < hazardCount; i++) {
                Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
                Quaternion spawnRotation = Quaternion.identity;

                GameObject hazard = hazards[Random.Range(0, hazards.Length)];
                GameObject clone = Instantiate(hazard, spawnPosition, spawnRotation) as GameObject;

                if (clone.CompareTag("Asteroid")) {
                    float asteroidScale = Random.Range(0.5f, 1.3f);
                    clone.transform.localScale = new Vector3(asteroidScale, asteroidScale, asteroidScale);
                }

                yield return new WaitForSeconds(Random.Range(0.1f, spawnWait));
            }
            yield return new WaitForSeconds(waveWait);           
        }
    }
}
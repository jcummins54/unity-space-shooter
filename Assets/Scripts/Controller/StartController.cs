using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartController : MonoBehaviour {

    public InputField nameInput;
    public Button startButton;
    public Text scoreText;
    public Text highScoreText;
    public Text highScoreNames;
    public Text highScoreScores;

    void Start() {
        scoreText.text = GameModel.GetInstance().playerScore.ToString();
        nameInput.text = GameModel.GetInstance().playerName;
        RefreshHighScores();
        startButton.onClick.AddListener(delegate {
                BeginGame();
            });
    }

    void OnEnable() {
        DynamoDbObject.OnHighScoresLoaded += RefreshHighScores;
    }

    void Update() {
        if (Input.GetKey(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public void RefreshHighScores() {
        highScoreText.text = GameModel.GetInstance().GetHighScore();
        highScoreNames.text = GameModel.GetInstance().GetHighScoreNames();
        highScoreScores.text = GameModel.GetInstance().GetHighScoreScores();
    }

    public void BeginGame() {
        GameModel.GetInstance().playerName = nameInput.text;
        SceneManager.LoadScene("02 Main");
    }
}

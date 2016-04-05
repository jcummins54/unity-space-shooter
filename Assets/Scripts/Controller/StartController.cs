using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartController : MonoBehaviour {

    public InputField nameInput;
    public Button startButton;
    public Text highScoreNames;
    public Text highScoreScores;

    void Start() {
        UpdateHighScores();
        //SceneManager.LoadScene("02 Main");
    }

    void UpdateHighScores() {
        GameModel.GetInstance().UpdateHighScores();
        highScoreNames.text = GameModel.GetInstance().GetHighScoreNames();
        highScoreScores.text = GameModel.GetInstance().GetHighScoreScores();
    }
}

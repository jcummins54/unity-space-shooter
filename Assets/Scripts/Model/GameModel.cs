﻿using UnityEngine;
using System;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameModel : MonoBehaviour {

    [SerializeField] private PlayerModel player;

    public string playerName {
        get {
            if (player == null) {
                player = PlayerModel.Factory();
            }
            return player.name;
        }
        set {
            if (player == null) {
                player = PlayerModel.Factory();
            }
            player.name = value;
        }
    }

    public float playerScore {
        get {
            if (player == null) {
                player = PlayerModel.Factory();
            }
            return player.score;
        }
        set {
            if (player == null) {
                player = PlayerModel.Factory();
            }
            player.score = value;
        }
    }

    public HighScoresModel highScores;

    private static GameModel _instance;

    public static GameModel GetInstance() {
        if (_instance != null) {
            return _instance;
        }
        GameObject go = GameObject.Find("GameModel");
        if (go != null) {
            _instance = go.GetComponent<GameModel>();
            return _instance;
        }
        Debug.Log("Cannot find 'GameModel' script");
        return null;
    }

    void Awake() {
        if (_instance == null) {
            LoadGameData();           
            DontDestroyOnLoad(gameObject);
            _instance = this;
        } else if (_instance != this) {
            Destroy(gameObject);
        }
    }

    void Start() {
        DontDestroyOnLoad(gameObject);
    }

    public void UpdateHighScores() {
        int i = 0;
        if (highScores == null || highScores.players.Length == 0) {
            highScores = HighScoresModel.Factory();
            highScores.players[0] = player.Clone<PlayerModel>();
            SaveGameData();
            return;
        }
        if (highScores.players.Length < 10) {
            PlayerModel[] tmpPlayers = new PlayerModel[highScores.players.Length + 1];
            for (i = 0; i < highScores.players.Length; i++) {
                tmpPlayers[i] = highScores.players[i];
            }
            tmpPlayers[highScores.players.Length] = PlayerModel.Factory();
            highScores.players = tmpPlayers;
        }
        int j = 0;
        for (i = 0; i < highScores.players.Length; i++) {
            if (highScores.players[i].score < playerScore) {
                j = highScores.players.Length - 1;
                while (j > i) {
                    highScores.players[j] = highScores.players[j - 1];
                    j--;
                }
                highScores.players[i] = player.Clone<PlayerModel>();
                break;
            }
        }
        SaveGameData();
    }

    public string GetHighScoreNames() {
        string text = "";
        for (int i = 0; i < highScores.players.Length; i++) {
            text += highScores.players[i].name + "\n";
        }
        return text;
    }

    public string GetHighScoreScores() {
        string text = "";
        for (int i = 0; i < highScores.players.Length; i++) {
            text += highScores.players[i].score.ToString() + "\n";
        }
        return text;
    }

    public string GetHighScore() {
        string text = "";
        if (highScores.players.Length > 0) {
            text = highScores.players[0].name + "  " + highScores.players[0].score.ToString();
        }
        return text;
    }

    public void SaveGameData() {
        SavePlayer();
        SaveHighScores();
    }

    public void LoadGameData() {
        LoadPlayer();
        //LoadHighScores();
    }

    public void SavePlayer() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Create);
        bf.Serialize(file, player);
        file.Close();
    }

    public void LoadPlayer() {
        if (File.Exists(Application.persistentDataPath + "/player.dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Open);
            player = (PlayerModel)bf.Deserialize(file);
            file.Close();
        }
    }

    public void SaveHighScores() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/highscores.dat", FileMode.Create);
        bf.Serialize(file, highScores);
        file.Close();
    }

    public void LoadHighScores() {
        if (File.Exists(Application.persistentDataPath + "/highscores.dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/highscores.dat", FileMode.Open);
            highScores = (HighScoresModel)bf.Deserialize(file);
            file.Close();
        }
    }
}
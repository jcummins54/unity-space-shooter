[System.Serializable]
public class HighScoresModel : AbstractModel {
    public PlayerModel[] players;

    public static HighScoresModel Factory() {
        HighScoresModel instance = new HighScoresModel();
        instance.players = new PlayerModel[1];
        return instance;
    }
}
using UnityEngine;

[System.Serializable]
public class PlayerModel : AbstractModel {
    
    public string name;
    public float score;

    public static PlayerModel Factory() {
        PlayerModel instance = new PlayerModel();
        instance.name = "Unknown";
        instance.score = 0;
        return instance;
    }
}

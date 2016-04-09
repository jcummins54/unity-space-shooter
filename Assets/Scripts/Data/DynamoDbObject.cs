using UnityEngine;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.CognitoIdentity;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Util;

public class DynamoDbObject : MonoBehaviour {

    public delegate void HighScoresLoaded();
    public static event HighScoresLoaded OnHighScoresLoaded;

    public string IdentityPoolId = "";
    public string CognitoPoolRegion = RegionEndpoint.USEast1.SystemName;
    public string DynamoRegion = RegionEndpoint.USEast1.SystemName;

    private RegionEndpoint _CognitoPoolRegion {
        get { return RegionEndpoint.GetBySystemName(CognitoPoolRegion); }
    }

    private RegionEndpoint _DynamoRegion {
        get { return RegionEndpoint.GetBySystemName(DynamoRegion); }
    }

    private static IAmazonDynamoDB _ddbClient;

    private AWSCredentials _credentials;

    private AWSCredentials Credentials {
        get {
            if (_credentials == null)
                _credentials = new CognitoAWSCredentials(IdentityPoolId, _CognitoPoolRegion);
            return _credentials;
        }
    }

    private IAmazonDynamoDB Client {
        get {
            if (_ddbClient == null) {
                _ddbClient = new AmazonDynamoDBClient(Credentials, _DynamoRegion);
            }

            return _ddbClient;
        }
    }

    private static Table highScoresTable;

    private static DynamoDbObject _instance;

    public static DynamoDbObject GetInstance() {
        if (_instance != null) {
            return _instance;
        }
        GameObject go = GameObject.Find("DynamoDbObject");
        if (go != null) {
            _instance = go.GetComponent<DynamoDbObject>();
            return _instance;
        }
        Debug.Log("Cannot find 'DynamoDbObject' script");
        return null;
    }

    void Awake() {
        if (_instance == null) {
            UnityInitializer.AttachToGameObject(this.gameObject);
            Table.LoadTableAsync(Client, "UnityHighScores", (loadTableResult) => {
                if (loadTableResult.Exception != null) {
                    Debug.Log("Failed to load highScores table");
                } else {
                    highScoresTable = loadTableResult.Result;
                }
            });
            GetHighScores();
            DontDestroyOnLoad(gameObject);
            _instance = this;
        } else if (_instance != this) {
            Destroy(gameObject);
        }
    }

    void Start() {
        DontDestroyOnLoad(gameObject);
    }

    public void SavePlayer() {
        int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        string pId = SystemInfo.deviceUniqueIdentifier.ToString() + "|" + unixTimestamp.ToString();
        var doc = new Document();
        doc["GameTitle"] = "Space Shooter";
        doc["Score"] = GameModel.GetInstance().playerScore;
        doc["Id"] = pId;
        doc["PlayerName"] = GameModel.GetInstance().playerName;
        doc["DeviceId"] = SystemInfo.deviceUniqueIdentifier;
        highScoresTable.PutItemAsync(doc,(r)=>{
            if (r.Exception != null) {
                Debug.Log("Put exception: " + r.Exception.Message);
            }
        });
    }

    void GetHighScores() {
        QueryRequest request = new QueryRequest {
            TableName = "UnityHighScores",
            IndexName = "GameTitle-Score-index",
            KeyConditions = new Dictionary<string, Condition>() {
                {
                    "GameTitle",
                    new Condition {
                        ComparisonOperator = "EQ",
                        AttributeValueList = new List<AttributeValue>() {
                            new AttributeValue { S = "Space Shooter" }
                        }
                    }
                }
            },
            ScanIndexForward = false
        };
        
        request.ReturnConsumedCapacity = "TOTAL";

        Client.QueryAsync(request,(result)=>{
            if (result.Exception != null) {
                Debug.Log("QueryRequest Error: " + result.Exception.Message);
            }
            HighScoresModel highScores = new HighScoresModel();
            highScores.players = new PlayerModel[result.Response.Items.Count];
            int i = 0;
            foreach (Dictionary<string, AttributeValue> item in result.Response.Items) {
                //Debug.Log(PrintItem(item));
                PlayerModel player = new PlayerModel();
                foreach (var kvp in item) {
                    switch(kvp.Key) {
                        case "PlayerName":
                            player.name = kvp.Value.S;
                            break;
                        case "Score":
                            player.score = float.Parse(kvp.Value.N);
                            break;
                    }
                }
                highScores.players[i] = player;
                i++;
            }
            GameModel.GetInstance().highScores = highScores;
            OnHighScoresLoaded();
        });
    }

    private string PrintItem(Dictionary<string, AttributeValue> attributeList) {
        string output = "";
        foreach (var kvp in attributeList) {
            string attributeName = kvp.Key;
            AttributeValue value = kvp.Value;

            output += 
                (
                    attributeName + " " +
                    (value.S == null ? "" : "S=[" + value.S + "]") +
                    (value.N == null ? "" : "N=[" + value.N + "]") +
                    (value.SS == null ? "" : "SS=[" + string.Join(",", value.SS.ToArray()) + "]") +
                    (value.NS == null ? "" : "NS=[" + string.Join(",", value.NS.ToArray()) + "]") + " | "
                );
        }
        return output;
    }
}
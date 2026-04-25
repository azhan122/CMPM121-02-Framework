using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;


// seperates gameplay logic from data handling for EnemySpawner 
public class DataManager : MonoBehaviour
{
    // stores data from levels.json
    public List<JObject> levelData;

    // stores data from enemies.json
    public List<JObject> enemyData;


     // called before start on all objects so data is ready
    void Awake()
    {
        LoadData();
    }

     // reads json files and parses them
    void LoadData()
    {
        // load and parse level definitions
        string levelsText = Resources.Load<TextAsset>("levels").text;
        levelData = JsonConvert.DeserializeObject<List<JObject>>(levelsText);

        // load and parse enemy definitions
        string enemiesText = Resources.Load<TextAsset>("enemies").text;
        enemyData = JsonConvert.DeserializeObject<List<JObject>>(enemiesText);
    }
}
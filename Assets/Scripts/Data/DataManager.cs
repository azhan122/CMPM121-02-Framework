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
        // load level and enemy definitions
        TextAsset levelsFile = Resources.Load<TextAsset>("levels");
        TextAsset enemiesFile = Resources.Load<TextAsset>("enemies");

        // parse level and enemy definitions
        levelData = JsonConvert.DeserializeObject<List<JObject>>(levelsFile.text);
        enemyData = JsonConvert.DeserializeObject<List<JObject>>(enemiesFile.text);
    }
}
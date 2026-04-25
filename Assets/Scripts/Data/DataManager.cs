using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;


// seperates gameplay logic from data handling for EnemySpawner 
public class DataManager : MonoBehaviour
{
    // stores data from levels.json and enemies.js
    public List<JObject> levelData;
    public List<JObject> enemyData;

    // faster lookup of enemies using names
     public Dictionary<string, JObject> enemyMap;

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

        // create dictionary from loaded enemies
        enemyMap = new Dictionary<string, JObject>();
        foreach (var enemy in enemyData)
        {
            string name = enemy["name"].ToString();
            enemyMap[name] = enemy;
        }
    }
}
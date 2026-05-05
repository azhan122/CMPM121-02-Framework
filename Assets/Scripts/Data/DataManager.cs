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

    // stores data from spells.json and makes faster lookup
     public List<JObject> spellData;
    public Dictionary<string, JObject> spellMap;

     // called before start on all objects so data is ready
    void Awake()
    {
        LoadData();
    }

     // reads json files and parses them
    void LoadData()
    {
        // load level enemy and spell definitions
        TextAsset levelsFile = Resources.Load<TextAsset>("levels");
        TextAsset enemiesFile = Resources.Load<TextAsset>("enemies");
        TextAsset spellsFile = Resources.Load<TextAsset>("spells");

        // parse level enemy and spell definitions
        levelData = JsonConvert.DeserializeObject<List<JObject>>(levelsFile.text);
        enemyData = JsonConvert.DeserializeObject<List<JObject>>(enemiesFile.text);

        // create dictionary from loaded enemies
        enemyMap = new Dictionary<string, JObject>();
        foreach (var enemy in enemyData)
        {
            string name = enemy["name"].ToString();
            enemyMap[name] = enemy;
        }

        // parse spells as dictionary
        JObject rawSpells = JObject.Parse(spellsFile.text);
        spellData = new List<JObject>();
        spellMap = new Dictionary<string, JObject>();

        // go through spells.json keys
        foreach (var pair in rawSpells)
        {
            // spell id and data
            string id = pair.Key;
            JObject spell = (JObject)pair.Value;

            // connect the id into the spell object
            spell["id"] = id;

            // add spell to list and dictionary for fast lookup
            spellData.Add(spell);
            spellMap[id] = spell;
        }
    }
}
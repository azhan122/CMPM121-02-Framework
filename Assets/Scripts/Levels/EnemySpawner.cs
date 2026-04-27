using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    public Image level_selector;
    public GameObject button;
    public GameObject enemy;
    public SpawnPoint[] SpawnPoints;    

    // reference to data manager
    DataManager dm;

    // currently selected level data
    JObject currentLevel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        // get reference to DataManager
        dm = FindFirstObjectByType<DataManager>();

        GameObject selector = Instantiate(button, level_selector.transform);
        selector.transform.localPosition = new Vector3(0, 130);
        selector.GetComponent<MenuSelectorController>().spawner = this;
        selector.GetComponent<MenuSelectorController>().SetLevel("Easy");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartLevel(string levelname)
    {
        level_selector.gameObject.SetActive(false);

        // find selected level from json
        currentLevel = dm.levelData.First(level => level["name"].ToString() == levelname);

        GameManager.Instance.player.GetComponent<PlayerController>().StartLevel();
        StartCoroutine(SpawnWave());
    }

    public void NextWave()
    {
        StartCoroutine(SpawnWave());
    }


    IEnumerator SpawnWave()
    {
        GameManager.Instance.state = GameManager.GameState.COUNTDOWN;
        GameManager.Instance.countdown = 3;
        for (int i = 3; i > 0; i--)
        {
            yield return new WaitForSeconds(1);
            GameManager.Instance.countdown--;
        }
        GameManager.Instance.state = GameManager.GameState.INWAVE;

        // get spawn defs from selected level
        var spawns = currentLevel["spawns"] as JArray;

        // loop through enemy types in json
        foreach (var spawn in spawns)
        {
            string enemyName = spawn["enemy"].ToString();

            // spawn 3 of every type for testing
            for (int i = 0; i < 3; i++)
            {
                yield return SpawnEnemy(enemyName);
            }
        }
        yield return new WaitWhile(() => GameManager.Instance.enemy_count > 0);
        GameManager.Instance.state = GameManager.GameState.WAVEEND;
    }

    IEnumerator SpawnEnemy(string enemyName)
    {
        // choose a random spawn point
        SpawnPoint spawn_point = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
        Vector2 offset = Random.insideUnitCircle * 1.8f;
        Vector3 initial_position = spawn_point.transform.position + new Vector3(offset.x, offset.y, 0);

        // create enemy instance
        GameObject new_enemy = Instantiate(enemy, initial_position, Quaternion.identity);

        // get enemy data from DataManager 
        DataManager dm = FindFirstObjectByType<DataManager>();
        var enemyData = dm.enemyMap[enemyName];

        // get stats from json
        int spriteIndex = (int)enemyData["sprite"];
        int hp = (int)enemyData["hp"];
        int speed = (int)enemyData["speed"];
        // damage still handled inside of EnemyController for now

        // apply sprite from sprite manager
        new_enemy.GetComponent<SpriteRenderer>().sprite =
        GameManager.Instance.enemySpriteManager.Get(spriteIndex);

        // apply stats to enemy controller
        EnemyController en = new_enemy.GetComponent<EnemyController>();
        en.hp = new Hittable(hp, Hittable.Team.MONSTERS, new_enemy);
        en.speed = speed;

        // tie enemy to GameManager
        GameManager.Instance.AddEnemy(new_enemy);

        yield return new WaitForSeconds(0.5f);
    }
}

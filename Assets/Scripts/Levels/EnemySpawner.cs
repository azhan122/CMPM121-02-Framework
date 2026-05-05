using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using RPNEvaluator;

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

    // wave counter
    int currentWave = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
            // get reference to DataManager
            dm = FindFirstObjectByType<DataManager>();
            
            // y positioning
            float yOffset = 100f;

            // space between buttons
            float spacing = 30f; 

            // loop through all levels in levels.json
            for (int i = 0; i < dm.levelData.Count; i++)
            {
                var level = dm.levelData[i];

                // get level name from json
                string levelName = level["name"].ToString();

                // create button
                GameObject selector = Instantiate(button, level_selector.transform);

                // position buttons vertically
                selector.transform.localPosition = new Vector3(0, yOffset - i * spacing);

                // connect button to spawner
                var controller = selector.GetComponent<MenuSelectorController>();
                controller.spawner = this;

                // set level name
                controller.SetLevel(levelName);
            }
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

            // get count expression from json
            string countExpr = spawn["count"].ToString();

            // get base value from enemy definition
            int baseHp = (int)dm.enemyMap[enemyName]["hp"];

            // variables for RPN
            Dictionary<string, int> vars = new Dictionary<string, int>()
            {
                { "wave", currentWave },
                { "base", baseHp }
            };

            // evaluate using dll
            int count = RPNEvaluator.RPNEvaluator.Evaluate(countExpr, vars);

            StartCoroutine(SequenceGroup(spawn, enemyName, count));
        }
        yield return new WaitWhile(() => GameManager.Instance.enemy_count > 0);
        if (GameManager.Instance.state == GameManager.GameState.PREGAME)
        {
            yield break;
        } // Alyssa: Prevents game from going to WAVEEND stage when restarting
        GameManager.Instance.state = GameManager.GameState.WAVEEND;

        // count waves
        currentWave++;
    }

    IEnumerator SpawnEnemy(string enemyName)
    {
        // choose a random spawn point
        SpawnPoint spawn_point = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
        Vector2 offset = Random.insideUnitCircle * 1.8f;
        Vector3 initial_position = spawn_point.transform.position + new Vector3(offset.x, offset.y, 0);

        // create enemy instance
        GameObject new_enemy = Instantiate(enemy, initial_position, Quaternion.identity);

        // get enemy data 
        var enemyData = this.dm.enemyMap[enemyName];

        // get stats from json
        int spriteIndex = (int)enemyData["sprite"];
        int hp = (int)enemyData["hp"];
        int speed = (int)enemyData["speed"];
        int damage = (int)enemyData["damage"];

        // read scale
        float scale = 1f;
        if (enemyData["scale"] != null)
        {
            scale = (float)enemyData["scale"];
        }

        // apply scale to entire enemy object
        new_enemy.transform.localScale = Vector3.one * scale;

        // apply sprite from sprite manager
        new_enemy.GetComponent<SpriteRenderer>().sprite =
        GameManager.Instance.enemySpriteManager.Get(spriteIndex);

        // apply stats to enemy controller
        EnemyController en = new_enemy.GetComponent<EnemyController>();
        en.hp = new Hittable(hp, Hittable.Team.MONSTERS, new_enemy);
        en.speed = speed;
        en.damage = damage;

        // if scaling looks weird with current ui, scale the health ui too
        // en.healthui.transform.localScale = Vector3.one;

        // tie enemy to GameManager
        GameManager.Instance.AddEnemy(new_enemy);

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator SequenceGroup(JToken spawn, string enemyName, int count)
    {
        // default sequence is 1 spawn at a time
        List<int> sequence = new List<int>();

        // read sequence from json
        if (spawn["sequence"] != null)
        {
            // convert json array into a list
            foreach (var value in spawn["sequence"])
            {
                sequence.Add((int)value);
            }
        }
        else
        {
            sequence.Add(1);
        }

        // default delay
        float delay = 2f;

        // read delays from json
        if (spawn["delay"] != null)
        {
            delay = float.Parse(spawn["delay"].ToString());
        }

        // enemies spawned already
        int spawned = 0;

        // index for cycling sequence
        int seqIndex = 0;   

        // keep spawning until we reach total count
        while (spawned < count)
        {
            // get current group size from sequence
            int groupSize = sequence[seqIndex];

            // move to next sequence index
            seqIndex++;
            if (seqIndex >= sequence.Count)
            {
                seqIndex = 0;
            }

            // make sure we don't spawn extra
            int remaining = count - spawned;
            if (groupSize > remaining)
            {
                groupSize = remaining;
            }

            // spawn defined group
            for (int i = 0; i < groupSize; i++)
            {
                yield return SpawnEnemy(enemyName);
                spawned++;
            }

            // wait for next group
            yield return new WaitForSeconds(delay);
        }
    }
}

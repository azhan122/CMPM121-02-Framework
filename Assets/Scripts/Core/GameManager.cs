using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

using TMPro;
using UnityEngine.SceneManagement; // Alyssa: Link to slain enemies counter

public class GameManager 
{
    public enum GameState
    {
        PREGAME,
        INWAVE,
        WAVEEND,
        COUNTDOWN,
        GAMEOVER
    }
    public GameState state;

    public int countdown;
    public int wave;
    private static GameManager theInstance;
    public static GameManager Instance {  get
        {
            if (theInstance == null)
                theInstance = new GameManager(); // Make theInstance null after it is no longer null
            return theInstance;
        }
    }

    // Alyssa: Resetting game
    public static void InstanceNull()
    {theInstance = null;}
    public void GameReload()
    {
        // Delete all current enemies on the board
        /*while (enemies.Count != 0)
        {
            EnemyController.DestroyObject();
        }*/
        //foreach(GameObject enemies in EnemyController)

        InstanceNull();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /*public void GameReload()
    {*/
        // Delete all current enemies on the board
        /*while (enemies.Count != 0)
        {
            EnemyController.Die();
        }*/
        /*GameManager.Instance.state = GameManager.GameState.PREGAME;
        InstanceNull();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }*/

    public GameObject player;
    
    public ProjectileManager projectileManager;
    public SpellIconManager spellIconManager;
    public EnemySpriteManager enemySpriteManager;
    public PlayerSpriteManager playerSpriteManager;
    public RelicIconManager relicIconManager;
    public JObject pendingSpellReward;

    private List<GameObject> enemies;
    public int enemy_count { get { return enemies.Count; } }

    // Alyssa: Timer & slain additions
    public int slaincount;
    public int shotnum;

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }
    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
        slaincount += 1; // Alyssa: Add 1 to the counter for every enemy defeated
    }

    public GameObject GetClosestEnemy(Vector3 point)
    {
        if (enemies == null || enemies.Count == 0) return null;
        if (enemies.Count == 1) return enemies[0];
        return enemies.Aggregate((a,b) => (a.transform.position - point).sqrMagnitude < (b.transform.position - point).sqrMagnitude ? a : b);
    }

    private GameManager()
    {
        enemies = new List<GameObject>();
    }
}

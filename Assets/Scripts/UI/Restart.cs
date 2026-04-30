// Alyssa: Found on Unity Discussions, testing to see if this will cause the game to restart

/*using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;*/

/*public class Restart : MonoBehaviour
{
    /*public static void InstanceNull()
    {
        GameManager.theInstance = null;
    }
    public static void GameReload()
    {
        GameManager.Instance.state = GameManager.GameState.PREGAME;
        InstanceNull();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }*/

/*public void GameReload()
{
    GameManager.Instance.state = GameManager.GameState.PREGAME;
    InstanceNull();
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}

}*/

// Alyssa: Game Over screen toggle

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;

public class Restart : MonoBehaviour
{
    //GameManager.GameReload();
}
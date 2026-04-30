// Alyssa: Game Over screen toggle

using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    public GameObject endUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.GAMEOVER)
        {
            endUI.SetActive(true);
        }
        else
        {
            endUI.SetActive(false);
        }
    }
}

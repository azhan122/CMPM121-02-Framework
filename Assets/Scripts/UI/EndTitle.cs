using UnityEngine;

using TMPro;

// Alyssa: Show final stats, Game Over message, and create button to reset all stats & game
public class EndTitle : MonoBehaviour
{

    public TextMeshProUGUI title;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        title = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {
            title.text = string.Format("Wave Passed");
        }
        if (GameManager.Instance.state == GameManager.GameState.GAMEOVER)
        {
            title.text = string.Format("Game Over");
        }
    }
}

using UnityEngine;
using TMPro; // Alyssa: Place info in text mesh

public class RewardScreenManager : MonoBehaviour
{
    public GameObject rewardUI;

    public TextMeshProUGUI spellname; // Alyssa: Display spell name in UI text
    public TextMeshProUGUI spelldesc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {
            var name = GameManager.Instance.pendingSpellReward["name"]; // Alyssa: Get spell name
            spellname.text = string.Format("Modifier: {0}", name);

            var desc = GameManager.Instance.pendingSpellReward["description"];
            spelldesc.text = string.Format("Description: {0}", desc);

            Debug.Log(name);
            rewardUI.SetActive(true);
        }
        else
        {
            rewardUI.SetActive(false);
        }
    }
}

using TMPro; // Alyssa: Place info in text mesh
using UnityEngine;
using System.Collections.Generic;

public class RewardScreenManager : MonoBehaviour
{
    public GameObject rewardUI;

    public TextMeshProUGUI spellname; // Alyssa: Display spell name in UI text
    public TextMeshProUGUI spelldesc;

    public PlayerController player; // Alyssa: Call player for setting post wave stats
    public Hittable statistics; // Alyssa: Call SetMaxHP later

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {}

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {
            var name = GameManager.Instance.pendingSpellReward["name"]; // Alyssa: Get spell name
            spellname.text = string.Format("Modifier: {0}", name);

            var desc = GameManager.Instance.pendingSpellReward["description"];
            spelldesc.text = string.Format("Description: {0}", desc);


            Dictionary<string, int> vars = new Dictionary<string, int>() // Alyssa: Set new stats
            {
                { "wave", GameManager.Instance.wave}
            };
            player.hp.SetMaxHP(RPNEvaluator.RPNEvaluator.Evaluate("95 wave 5 * +", vars));
            
            Debug.Log(player.hp.hp);
            Debug.Log(player.hp.max_hp);


            //Debug.Log(name);
            rewardUI.SetActive(true);
        }
        else
        {
            rewardUI.SetActive(false);
        }
    }
}

using UnityEngine;
using Newtonsoft.Json.Linq;

public class AcceptSpell : MonoBehaviour
{
    public EnemySpawner spawner;

    public void Accept()
    {   
        // ui stored
        PlayerController player = GameManager.Instance.player.GetComponent<PlayerController>();
        JObject reward = GameManager.Instance.pendingSpellReward;

        
        Spell spell = player.spellcaster.spell;

        // applies mod effect
        spell.AddModifier(reward);

        // reset stored mod
        GameManager.Instance.pendingSpellReward = null;
    }
}
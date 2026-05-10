using UnityEngine;
using Newtonsoft.Json.Linq;

public class AcceptSpell : MonoBehaviour
{
    public EnemySpawner spawner;

    public void Accept()
    {   
        // read stored modifier
        PlayerController player = GameManager.Instance.player.GetComponent<PlayerController>();
        JObject reward = GameManager.Instance.pendingSpellReward;
        SpellBuilder builder = new SpellBuilder();

        // apply modifier to player's spell
        player.spellcaster.spell.AddModifier(reward);

        Debug.Log("Accepted spell: " + reward["name"]);

        // clear reward so it can't be reused
        GameManager.Instance.pendingSpellReward = null;
    }
}
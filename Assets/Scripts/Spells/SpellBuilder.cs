using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class SpellBuilder 
{
    DataManager dm;

    public SpellBuilder()
    {
        dm = GameObject.FindFirstObjectByType<DataManager>();
    }

    public Spell Build(SpellCaster owner)
    {
        // arcane bolt only now
        JObject baseSpell = dm.spellMap["arcane_bolt"];

        // create spell
        Spell spell = new Spell(owner);
        spell.SetAttributes(baseSpell);

        // possible modifier ids
        List<string> modifierIDs = new List<string>()
        {
            "damage_amp",
            "speed_amp",
            "doubler",
            "splitter",
            "chaos",
            "homing"
        };

        // random modifier count
        int modifierCount = Random.Range(1, 3);

        for (int i = 0; i < modifierCount; i++)
        {
            // pick random modifier
            string id = modifierIDs[Random.Range(0, modifierIDs.Count)];
            JObject modifier = dm.spellMap[id];

            // attach modifier to spell (for now)
            spell.AddModifier(modifier);

            // debug modifier added
            Debug.Log("Applied modifier: " + modifier["name"]);
        }

        return spell;
    }
}
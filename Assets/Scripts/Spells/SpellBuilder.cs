using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class SpellBuilder 
{
    DataManager dm;

    // modifiers defined from data
    List<string> modifierIDs = new List<string>()
    {
        "damage_amp",
        "speed_amp",
        "doubler",
        "splitter",
        "chaos",
        "homing"
    };

    public SpellBuilder()
    {
        dm = GameObject.FindFirstObjectByType<DataManager>();
    }

    // creates spell itself
    public Spell Build(SpellCaster owner)
    {
        // always use arcane bolt
        JObject baseSpell = dm.spellMap["arcane_bolt"];
        Spell spell = new Spell(owner);
        spell.SetAttributes(baseSpell);

        return spell;
    }

    // applies one random modifier to a spell
    public void ApplyRandomModifier(Spell spell)
    {
        // pick random modifier id
        string id = modifierIDs[Random.Range(0, modifierIDs.Count)];

        // get modifier json
        JObject modifier = dm.spellMap[id];

        // apply modifier to spell 
        spell.AddModifier(modifier);
    }

    // get one random spell
    public JObject GenerateRandomModifier()
    {
        // pick random modifier id
        string id = modifierIDs[Random.Range(0, modifierIDs.Count)];

        // return modifier json
        return dm.spellMap[id];
    }
}
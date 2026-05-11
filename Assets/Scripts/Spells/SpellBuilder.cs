using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class SpellBuilder 
{
    DataManager dm;

    List<string> modifierIDs = new List<string>();

    public SpellBuilder()
    {
        dm = GameObject.FindFirstObjectByType<DataManager>();

        // build modifier list from json
        foreach (var pair in dm.spellMap)
        {
            JObject spell = pair.Value;

            // treat anything without projectile as modifier
            if (spell["projectile"] == null)
            {
                modifierIDs.Add(pair.Key);
            }
        }
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
        string id;

        do
        {
            id = modifierIDs[Random.Range(0, modifierIDs.Count)];
        }
        while (spell.activeBehaviorMods.Contains(
            dm.spellMap[id]["name"]?.ToString()
        ));
        JObject modifier = dm.spellMap[id];
        spell.ApplyModifier(modifier);
    }

    // get one random modifier for reward UI
    public JObject GenerateRandomModifier(Spell spell)
    {
        string id;
        do
        {
            id = modifierIDs[Random.Range(0, modifierIDs.Count)];
        }
        while (spell.activeBehaviorMods.Contains(dm.spellMap[id]["name"]?.ToString()));

        return dm.spellMap[id];
    }
}
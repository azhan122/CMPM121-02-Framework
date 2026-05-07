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
        // create default arcane bolt spell
        Spell spell = new Spell(owner);

        // load arcane bolt data from spells.json
        spell.SetAttributes(dm.spellMap["arcane_bolt"]);

        // list of modifier ids
        List<string> modifiers = new List<string>()
        {
            "damage_amp",
            "speed_amp",
            "splitter",
            "doubler",
            "chaos",
            "homing"
        };

        // random amount of modifiers
        int modifierCount = Random.Range(0, 3);

        // add random modifiers
        for (int i = 0; i < modifierCount; i++)
        {
            string chosen = modifiers[Random.Range(0, modifiers.Count)];

            // add modifier data to spell
            spell.AddModifier(dm.spellMap[chosen]);
        }

        return spell;
    }
}
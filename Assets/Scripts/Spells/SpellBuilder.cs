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
        // collect base spells only (
        List<JObject> baseSpells = new List<JObject>();

        foreach (var spellData in dm.spellData)
        {
            if (spellData["damage"] != null && spellData["projectile"] != null)
            {
                baseSpells.Add(spellData);
            }
        }

        if (baseSpells.Count == 0)
        {
            Debug.LogError("No base spells found in spellData!");
            return new Spell(owner);
        }

        // pick random base spell
        JObject chosen = baseSpells[Random.Range(0, baseSpells.Count)];

        // create spell and attach jSON
        Spell spell = new Spell(owner);
        spell.SetAttributes(chosen);

        return spell;
    }
}
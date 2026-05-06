using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;


public class SpellBuilder 
{

    public Spell Build(SpellCaster owner)
    {
        DataManager dm = GameObject.FindFirstObjectByType<DataManager>();

        // collect base spells 
        List<JObject> baseSpells = new List<JObject>();

        foreach (var spell in dm.spellData)
        {
            // base spells damage
            if (spell["damage"] != null)
            {
                baseSpells.Add(spell);
            }
        }

        // safety check
        if (baseSpells.Count == 0)
        {
            Debug.LogError("No base spells found!");
            return new Spell(owner);
        }

        // pick a random base spell
        int index = Random.Range(0, baseSpells.Count);
        JObject chosen = baseSpells[index];

        // create Spell and inject json
        Spell newSpell = new Spell(owner);
        newSpell.SetAttributes(chosen);

        return newSpell;
    }

   
    public SpellBuilder()
    {        
    }

}

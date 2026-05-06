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

    foreach (var spell in dm.spellData)
    {
        // base spells damage
        if (spell["damage"] != null)
        {
            baseSpells.Add(spell);
        }
    }

    // pick a random base spell

    // create Spell and inject JSON
    Spell newSpell = something;

    return newSpell;
}

   
    public SpellBuilder()
    {        
    }

}

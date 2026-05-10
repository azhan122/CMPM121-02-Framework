using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using RPNEvaluator;


public class Spell 
{
    public float last_cast;
    public SpellCaster owner;
    public Hittable.Team team;

    // json data stored per spell
    protected JObject data;

    // store active modifiers for UI
    public List<ValueModifier> damageMods = new List<ValueModifier>();
    public List<ValueModifier> manaMods = new List<ValueModifier>();
    public List<ValueModifier> speedMods = new List<ValueModifier>();
    public List<ValueModifier> cooldownMods = new List<ValueModifier>();

    // stores modifier spell data
    protected List<JObject> modifiers = new List<JObject>();

    public Spell(SpellCaster owner)
    {
        this.owner = owner;
    }

    // read spell vars through json data
    public virtual string GetName()
    {
        return data?["name"]?.ToString() ?? "Spell";
    }

    public virtual int GetManaCost()
    {
        // evaluate RPN instead of parsing directly
        string expr = data?["mana_cost"]?.ToString() ?? "10";

        Dictionary<string, int> vars = new Dictionary<string, int>()
        {
            { "wave", GameManager.Instance.wave },   
            { "power", owner != null ? owner.spell_power : 0 } 
        };

        int baseMana = RPNEvaluator.RPNEvaluator.Evaluate(expr, vars);
        return (int)ValueModifier.Apply(baseMana, manaMods);
    }

    public virtual float GetCooldown()
    {
        float baseCd = float.Parse(data?["cooldown"]?.ToString() ?? "1");
        return ValueModifier.Apply(baseCd, cooldownMods);
    }

    public virtual int GetIcon()
    {
        return data?["icon"] != null ? (int)data["icon"] : 0;
    }

    // damage hardcoded still for now
    public virtual int GetDamage()
    {
        // RPN damage expressions
        var dmg = data?["damage"];
        string expr = dmg?["amount"]?.ToString() ?? "10";

        Dictionary<string, int> vars = new Dictionary<string, int>()
        {
            { "wave", GameManager.Instance.wave },
            { "power", owner != null ? owner.spell_power : 0 }
        };

        int baseDamage = RPNEvaluator.RPNEvaluator.Evaluate(expr, vars);
        return (int)ValueModifier.Apply(baseDamage, damageMods);
    }

    public bool IsReady()
    {
        return (last_cast + GetCooldown() < Time.time);
    }

    // spawns projectiles and passing behavior
    public virtual IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;

        // take projectile def from json
        var proj = data?["projectile"];

        // read projectile properties with defaults
        string trajectory = proj?["trajectory"]?.ToString() ?? "straight";

        // evaluate speed as RPN
        string speedExpr = proj?["speed"]?.ToString() ?? "10";

        Dictionary<string, int> vars = new Dictionary<string, int>()
        {
            { "wave", GameManager.Instance.wave },
            { "power", owner != null ? owner.spell_power : 0 }
        };

        float baseSpeed = RPNEvaluator.RPNEvaluator.Evaluate(speedExpr, vars);
        float speed = ValueModifier.Apply(baseSpeed, speedMods);
        int sprite = proj?["sprite"] != null ? (int)proj["sprite"] : 0;

        // tell projectile manager to create the projectile
        GameManager.Instance.projectileManager.CreateProjectile(sprite, trajectory, where, target - where, speed, OnHit);

        yield return new WaitForEndOfFrame();
    }

    // assign json data to the spell
    public virtual void SetAttributes(JObject attributes)
    {
        data = attributes;
    }

    // adds a modifier onto the spell
    public void AddModifier(JObject modifier)
    {   
        // ui data
        modifiers.Add(modifier);

        // modification effects
        ApplyModifier(modifier);
    }

    // called when projectiles hit something
    protected virtual void OnHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            other.Damage(new Damage(GetDamage(), Damage.Type.ARCANE));
        }
    }
    public void ApplyModifier(JObject mod)
    {
        // debug for testing
        Debug.Log("=== SPELL MODIFIER APPLIED ===");

        if (mod["name"] != null)
            Debug.Log("Modifier: " + mod["name"]);
        else
            Debug.Log("Modifier: UNKNOWN");

        // damage modifiers
        if (mod["damage_multiplier"] != null)
        {
            float val = float.Parse(mod["damage_multiplier"].ToString());
            damageMods.Add(new ValueModifier(ValueModifier.Type.MULTIPLY, val));
            Debug.Log("-> Damage x" + val);
        }

        // mana modifiers
        if (mod["mana_multiplier"] != null)
        {
            float val = float.Parse(mod["mana_multiplier"].ToString());
            manaMods.Add(new ValueModifier(ValueModifier.Type.MULTIPLY, val));
            Debug.Log("-> Mana x" + val);
        }

        if (mod["mana_adder"] != null)
        {
            float val = float.Parse(mod["mana_adder"].ToString());
            manaMods.Add(new ValueModifier(ValueModifier.Type.ADD, val));
            Debug.Log("-> Mana +" + val);
        }

        // speed modifiers
        if (mod["speed_multiplier"] != null)
        {
            float val = float.Parse(mod["speed_multiplier"].ToString());
            speedMods.Add(new ValueModifier(ValueModifier.Type.MULTIPLY, val));
            Debug.Log("-> Speed x" + val);
        }

        // cooldown modifiers
        if (mod["cooldown_multiplier"] != null)
        {
            float val = float.Parse(mod["cooldown_multiplier"].ToString());
            cooldownMods.Add(new ValueModifier(ValueModifier.Type.MULTIPLY, val));
            Debug.Log("-> Cooldown x" + val);
        }

        // behavior modifiers (trajectory override etc.)
        if (mod["projectile_trajectory"] != null)
        {
            string type = mod["projectile_trajectory"].ToString();
            modifiers.Add(mod);
            Debug.Log("-> Projectile behavior changed: " + type);
        }
    }
}


using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using RPNEvaluator; // ✅ needed for RPN evaluation


public class Spell 
{
    public float last_cast;
    public SpellCaster owner;
    public Hittable.Team team;

    // json data stored per spell
    protected JObject data;

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

        return RPNEvaluator.RPNEvaluator.Evaluate(expr, vars);
    }

    public virtual float GetCooldown()
    {
        // cooldown is usually float
        return float.Parse(data?["cooldown"]?.ToString() ?? "1");
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

        return RPNEvaluator.RPNEvaluator.Evaluate(expr, vars);
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

        float speed = RPNEvaluator.RPNEvaluator.Evaluate(speedExpr, vars);

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

    // called when projectiles hit something
    protected virtual void OnHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            other.Damage(new Damage(GetDamage(), Damage.Type.ARCANE));
        }
    }
}
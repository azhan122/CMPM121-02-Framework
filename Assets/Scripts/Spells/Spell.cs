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
    public HashSet<string> activeBehaviorMods = new HashSet<string>();

    // stores modifier spell data
    protected List<JObject> modifiers = new List<JObject>();


    // projectile behavior overrides
    public string trajectoryOverride = "";

    // multi-shot modifiers
    public bool doubler = false;
    public bool splitter = false;

    // modifier settings
    public float doublerDelay = 0.5f;
    public float splitAngle = 10f;

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

        // modifier can override projectile movement
        if (trajectoryOverride != "")
        {
            trajectory = trajectoryOverride;
        }

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

        // change scale
        float scale = 1f;
        if (proj?["scale"] != null)
        {
            scale = float.Parse(proj["scale"].ToString());
        }

        // tell projectile manager to create the projectile
        GameManager.Instance.projectileManager.CreateProjectile(sprite, trajectory, where, target - where, speed, scale, OnHit);

        // splitter shoots second angled projectile
        if (splitter)
        {
            Vector3 splitDir = Quaternion.Euler(0, 0, splitAngle) * (target - where);
            GameManager.Instance.projectileManager.CreateProjectile(sprite, trajectory, where, splitDir, speed, scale, OnHit);
        }

        // doubler shoots again after delay
        if (doubler)
        {
            yield return new WaitForSeconds(doublerDelay);
            GameManager.Instance.projectileManager.CreateProjectile(sprite, trajectory, where, target - where, speed, scale, OnHit);
        }
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
        string name = modifier["name"]?.ToString();

        // prevent duplicate behavior mods
        if (activeBehaviorMods.Contains(name))
        {
            return;
        }

        modifiers.Add(modifier);
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

    // evaluates modifier expressions from json
   private float EvaluateModifier(string expr)
    {
        // direct float first
        if (float.TryParse(expr, out float result))
        {
            return result;
        }

        // RPN otherwise
        Dictionary<string, int> vars = new Dictionary<string, int>()
        {
            { "wave", GameManager.Instance.wave },
            { "power", owner != null ? owner.spell_power : 0 }
        };

        try
        {
            return RPNEvaluator.RPNEvaluator.Evaluate(expr, vars);
        }
        catch
        {

            // unworking RPN of chaos modifier compensation
            if (expr == "1.5 wave 5 / +")
            {
                return 1.5f + (GameManager.Instance.wave / 5f);
            }
            return 1;
        }
    }

    public void ApplyModifier(JObject mod)
    {
        string name = mod["name"]?.ToString();

        Debug.Log("=== SPELL MODIFIER APPLIED ===");
        Debug.Log("Modifier: " + (name ?? "UNKNOWN"));

        // stats
        if (mod["damage_multiplier"] != null)
        {
            float val = EvaluateModifier(mod["damage_multiplier"].ToString());
            damageMods.Add(new ValueModifier(ValueModifier.Type.MULTIPLY, val));
        }

        if (mod["mana_multiplier"] != null)
        {
            float val = EvaluateModifier(mod["mana_multiplier"].ToString());
            manaMods.Add(new ValueModifier(ValueModifier.Type.MULTIPLY, val));
        }

        if (mod["mana_adder"] != null)
        {
            float val = EvaluateModifier(mod["mana_adder"].ToString());
            manaMods.Add(new ValueModifier(ValueModifier.Type.ADD, val));
        }

        if (mod["speed_multiplier"] != null)
        {
            float val = EvaluateModifier(mod["speed_multiplier"].ToString());
            speedMods.Add(new ValueModifier(ValueModifier.Type.MULTIPLY, val));
        }

        if (mod["cooldown_multiplier"] != null)
        {
            float val = EvaluateModifier(mod["cooldown_multiplier"].ToString());
            cooldownMods.Add(new ValueModifier(ValueModifier.Type.MULTIPLY, val));
        }

        if (mod["projectile_trajectory"] != null)
        {
            // remove old trajectory modifiers
            activeBehaviorMods.Remove("homing");
            activeBehaviorMods.Remove("chaotic");
            activeBehaviorMods.Remove("piercing");

            // apply new trajectory
            trajectoryOverride = mod["projectile_trajectory"].ToString();
        }

        // behavior cases

        if (name == "homing")
        {
            trajectoryOverride = "homing";
            activeBehaviorMods.Add("homing");
        }

        else if (name == "chaotic")
        {
            trajectoryOverride = "spiraling";
            activeBehaviorMods.Add("chaotic");
        }

        else if (name == "doubled")
        {
            doubler = true;
            activeBehaviorMods.Add("doubler");

            if (mod["delay"] != null)
                doublerDelay = EvaluateModifier(mod["delay"].ToString());
        }

        else if (name == "split")
        {
            splitter = true;
            activeBehaviorMods.Add("splitter");

            if (mod["angle"] != null)
                splitAngle = EvaluateModifier(mod["angle"].ToString());
        }

        else if (name == "piercing")
        {   
            //trajectoryOverride = "piercing";   uncomment this after creating PiercingProjectileMovement(speed); and uncommenting in ProjectileManager
            activeBehaviorMods.Add("piercing");
        }
    }
}


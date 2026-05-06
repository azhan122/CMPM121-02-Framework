using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

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
        return int.Parse(data?["mana_cost"]?.ToString() ?? "10");
    }

    public virtual float GetCooldown()
    {
        return float.Parse(data?["cooldown"]?.ToString() ?? "1");
    }

    public virtual int GetIcon()
    {
        return data?["icon"] != null ? (int)data["icon"] : 0;
    }

    // damage hardcoded still for now
    public virtual int GetDamage()
    {
        return 10;
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
        float speed = float.Parse(proj?["speed"]?.ToString() ?? "10");
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

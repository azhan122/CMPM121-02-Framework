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

    public virtual IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;
        GameManager.Instance.projectileManager.CreateProjectile(0, "straight", where, target - where, 15f, OnHit);
        yield return new WaitForEndOfFrame();
    }

    void OnHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            other.Damage(new Damage(GetDamage(), Damage.Type.ARCANE));
        }

    }

}

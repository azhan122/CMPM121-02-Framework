using System.Collections.Generic;

/// <summary>
/// Represents a single modification applied to a numeric spell property
/// (damage, mana cost, speed, cooldown, etc.)
/// 
/// This is NOT a spell. It is just a "rule" that modifies a value.
/// Example:
///     Multiply damage by 1.5
///     Add +10 mana cost
/// </summary>
public class ValueModifier
{
    // multiply or add value
    public enum Type
    {
        MULTIPLY,  
        ADD         
    }

    public Type type;

    public float value;

    // to create a modifier
    public ValueModifier(Type type, float value)
    {
        this.type = type;
        this.value = value;
    }

    // apply spell modifier 
    public static float Apply(float baseValue, List<ValueModifier> mods)
    {
        float result = baseValue;

        // Apply each modifier in order
        foreach (var mod in mods)
        {
            if (mod.type == Type.MULTIPLY)
            {
                result *= mod.value;
            }
            else if (mod.type == Type.ADD)
            {
                result += mod.value;
            }
        }

        return result;
    }
}
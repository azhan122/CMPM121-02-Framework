using UnityEngine;

public class AcceptSpell : MonoBehaviour
{
    public void Accept()
    {
        // get player
        PlayerController player = GameManager.Instance.player.GetComponent<PlayerController>();

        // apply random (for now) modifier 
        SpellBuilder builder = new SpellBuilder();
        builder.ApplyRandomModifier(player.spellcaster.spell);
    }
}
using UnityEngine;

using TMPro;

// Alyssa: Displays total enemies slain after wave ends
public class Slaintxt : MonoBehaviour
{

    public TextMeshProUGUI slain;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slain = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        slain.text = string.Format("Enemies slain: {0}", (GameManager.Instance.slaincount));
    }
}

using UnityEngine;

using TMPro;

// Alyssa: Not actually time elapsed but rather total shots fired, except changing the file name will break everything
public class timelapsed : MonoBehaviour
{

    public TextMeshProUGUI shot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shot = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        shot.text = string.Format("Shots fired: {0}", (GameManager.Instance.shotnum));
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToggleButtonSetup : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro text;
    [SerializeField]
    private ButtonInteractions buttonInteractions;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        switch (tag)
        {
            case "debug":
                text.text = $"Set {tag} to: {!buttonInteractions.state["showDebug"]}";
                break;
            case "gravity":
                text.text = $"Set {tag} to: {!buttonInteractions.state["gravityOn"]}";
                break;
            default:
                text.text = $"Undefined button tag";
                break;
        }
    }
}

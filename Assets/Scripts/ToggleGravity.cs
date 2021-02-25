using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class ToggleGravity : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void toggleGravity()
    {
        var creattedShapes = GameObject.FindGameObjectsWithTag("Floatable");

        foreach (var shape in creattedShapes)
        {
            shape.GetComponent<Rigidbody>().useGravity = GetComponent<Interactable>().IsToggled;
        }
    }
}

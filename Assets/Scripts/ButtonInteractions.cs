using OculusSampleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ButtonInteractions : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshPro debugText;
    public Dictionary<string, bool> state = new Dictionary<string, bool>()
    {
        { "showDebug" , true},
        { "gravityOn" , true},
    };
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //public void setShape(InteractableStateArgs obj)
    //{
    //    GetComponent<CreateShape>().shape = GetComponent<ShapeButtonSetup>().getShape();

    //}

    public void toggleGravity(InteractableStateArgs obj)
    {
        if (obj.NewInteractableState == InteractableState.ActionState)
        {
            state["gravityOn"] = !state["gravityOn"];
            var creattedShapes = GameObject.FindGameObjectsWithTag("Floatable");

            foreach (var shape in creattedShapes)
            {
                shape.GetComponent<Rigidbody>().useGravity = state["gravityOn"];
            }
        }
    }

    public void toggleDebug(InteractableStateArgs obj)
    {
        if (obj.NewInteractableState == InteractableState.ActionState)
        {
            state["showDebug"] = !state["showDebug"];
            debugText.enabled = state["showDebug"];
        }
    }
}

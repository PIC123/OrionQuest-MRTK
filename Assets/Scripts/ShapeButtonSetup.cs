using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShapeButtonSetup : MonoBehaviour
{
    [SerializeField]
    private PrimitiveType shape = PrimitiveType.Cube;
    public CreateShape gameLogic;
    // Start is called before the first frame update

    public void setShape()
    {
        gameLogic.shape = shape;
        gameLogic.UpdateDebugText();
    }
}

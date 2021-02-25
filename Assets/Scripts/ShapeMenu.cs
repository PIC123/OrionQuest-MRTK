using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeMenu : MonoBehaviour
{
    [SerializeField]
    private OVRHand menuHand;
    [SerializeField]
    private GameObject menu;
    // Start is called before the first frame update
    void Start()
    { 
    }

    // Update is called once per frame
    void Update()
    {
        menu.SetActive(menuHand.IsSystemGestureInProgress);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class boundaryWall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        OVRManager.boundary.SetVisible(true);
        if (OVRManager.boundary.GetConfigured())
        {
            Debug.Log("Boudnary ready");
            Vector3[] points = OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.OuterBoundary);
            foreach(Vector3 spot in points)
            {
                Debug.Log($"spot: {spot}");
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = spot;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

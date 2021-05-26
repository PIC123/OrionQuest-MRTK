using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CreateShape : MonoBehaviour
{
    [SerializeField]
    public PrimitiveType shape = PrimitiveType.Cube;
    [SerializeField]
    private OVRHand leftHand;
    [SerializeField]
    private OVRSkeleton leftSkeleton;
    [SerializeField]
    private OVRHand rightHand;
    [SerializeField]
    private OVRSkeleton rightSkeleton;
    [SerializeField]
    private Material[] randomMaterials;
    [SerializeField]
    private Material wireframeMaterial;
    [SerializeField]
    private HandTrackingGrabber[] grabbers;
    public float pinchTolerance = 0.6f;
    public bool createMode = false;
    // Soon
    private bool gravityOn = true;
    public TMP_Text debugText;
    public GameObject currentObj;
    public float minSize = 0.01f;
    public float upForce = 1f;


    // Start is called before the first frame update
    void Start()
    {
        //leftHand = leftHandObj.GetComponent<OVRHand>();
        //leftSkeleton = leftHandObj.GetComponent<OVRSkeleton>();
        //rightHand = leftHandObj.GetComponent<OVRHand>();
        //leftSkeleton = leftHandObj.GetComponent<OVRSkeleton>();

    }

    public void UpdateDebugText()
    {
        //shape = newShape;
        debugText.text = "Current shape: " + shape.ToString();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (aFingerComboIsPinching())
        {
            var leftx = leftSkeleton.Bones[19].Transform.position.x;
            var lefty = leftSkeleton.Bones[19].Transform.position.y;
            var leftz = leftSkeleton.Bones[19].Transform.position.z;
            var dx = leftx - rightSkeleton.Bones[19].Transform.position.x;
            var dy = lefty - rightSkeleton.Bones[19].Transform.position.y;
            var dz = leftz - rightSkeleton.Bones[19].Transform.position.z;
            //keep resizing, else create new obj
            if (createMode) 
            {
                if (Math.Abs(dx) > minSize)
                {
                    currentObj.transform.localScale = 0.8f * new Vector3(dx, dy, dz);
                    //Debug.Log("size:" + 0.8f * new Vector3(dx, dy, dz));
                }
                currentObj.transform.position = new Vector3(leftx - 0.5f * dx, lefty - 0.5f * dy, leftz - 0.5f * dz);
                //Debug.Log("position:" + currentObj.transform.position);

            }
            else
            {
                //Debug.Log(dx);
                if (dx < 0.1 && dy < 0.1 && dz < 0.1 && grabbers.Where(grabber=>grabber.grabbedObject != null).Count() == 0 )
                {
                    //Debug.Log("creating!");
                    currentObj = InstantiateShape();
                    currentObj.transform.localScale = new Vector3(dx, dy, dz);
                    currentObj.transform.position = new Vector3(leftx - 0.5f * dx, lefty - 0.5f * dy, leftz - 0.5f * dz);
                    createMode = true;
                }
            }
        } 
        else
        {
            // drop new object and swap modes
            if (createMode)
            {
                //Debug.Log("stop creating");
                currentObj.GetComponent<MeshRenderer>().material = randomMaterials[UnityEngine.Random.Range(0, randomMaterials.Length)];
                currentObj.GetComponent<Rigidbody>().useGravity = gravityOn;
                currentObj.GetComponent<Rigidbody>().detectCollisions = true;
                currentObj.AddComponent<NearInteractionGrabbable>();
                createMode = false;
            }
        }
    }

    private GameObject InstantiateShape()
    {
        GameObject currentShape = GameObject.CreatePrimitive(shape);
        currentShape.AddComponent<Rigidbody>();
        currentShape.GetComponent<Rigidbody>().useGravity = false;
        currentShape.GetComponent<MeshRenderer>().material = wireframeMaterial;
        currentShape.GetComponent<Rigidbody>().detectCollisions = false;
        currentShape.tag = "Floatable";
        return currentShape;
    }

    public bool aFingerComboIsPinching()
    {
        var isLeftHand = leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index) || leftHand.GetFingerIsPinching(OVRHand.HandFinger.Middle) || leftHand.GetFingerIsPinching(OVRHand.HandFinger.Ring) || leftHand.GetFingerIsPinching(OVRHand.HandFinger.Pinky);
        var isRightHand = rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index) || rightHand.GetFingerIsPinching(OVRHand.HandFinger.Middle) || rightHand.GetFingerIsPinching(OVRHand.HandFinger.Ring) || rightHand.GetFingerIsPinching(OVRHand.HandFinger.Pinky);
        return isLeftHand && isRightHand;
    }

    public void toggleGravity()
    {
        gravityOn = !gravityOn;
        var createdShapes = GameObject.FindGameObjectsWithTag("Floatable");
        Debug.Log(createdShapes.Length);
        foreach (var shape in createdShapes)
        {
            var rb = shape.GetComponent<Rigidbody>();
            rb.useGravity = gravityOn;
            if (!gravityOn)
            {
                rb.AddForce(Vector3.up * upForce * shape.GetComponent<Collider>().bounds.Volume());
            }
        }
    }

    // returns whichfinger is piched
    // 0 = none, 1 = index, 2 = middle, 3 = ring, 4 = pinky
    //private int whichFignerPinching()
    //{
    //    int finger = -1;

    //    if(leftHand.GetFingerPinchStrength(OVRHand.HandFinger.Index) > pinchTolerance && rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Index) > pinchTolerance)
    //    {
    //        finger = 3;
    //    }
    //    else if(leftHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle) > pinchTolerance && rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle) > pinchTolerance)
    //    {
    //        finger = 2;
    //    }
    //    else if(leftHand.GetFingerPinchStrength(OVRHand.HandFinger.Ring) > pinchTolerance && rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Ring) > pinchTolerance)
    //    {
    //        finger = 1;
    //    }
    //    else if(leftHand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky) > pinchTolerance && rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky) > pinchTolerance)
    //    {
    //        finger = 0;
    //    }

    //    return finger;
    //}
}

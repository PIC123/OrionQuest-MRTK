using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToObject : MonoBehaviour
{
    private Transform oldParent;
    private Rigidbody rBody;

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //void OnTriggerEnter(Collider collider)
    //{
    //    if (collider.gameObject.tag == "sticky" && collider.gameObject.transform.parent.name == "ShapeCanvas")
    //    {
    //        Debug.Log($"Stuck to: {collider.gameObject.name}");
    //        GameObject other = collider.gameObject;
    //        oldParent = transform.parent;
    //        var oldPositon = transform.position;

    //        this.transform.parent = other.transform.parent;
    //        this.transform.position = oldPositon;
    //        Debug.Log($"Frozen");
    //    }
    //}

    //void OnTriggerExit(Collider collider)
    //{
    //    Debug.Log($"UnStuck from: {collider.gameObject.name}");

    //    this.transform.parent = oldParent;
    //    Debug.Log($"Unfrozen");
    //}

    void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.parent.name == "ShapeCanvas")
        {
            Debug.Log($"Stuck to: {collision.gameObject.name}");
            GameObject other = collision.gameObject;
            oldParent = transform.parent;
            this.transform.SetParent(other.transform.parent);
            //var joint = gameObject.AddComponent<FixedJoint>();
            //joint.connectedBody = collision.rigidbody;
            DisableRagdoll();
            rBody.isKinematic = false;

            // Potential wierd fix
            var canvas = GameObject.Find("ShapeCanvas").transform;
            for (int i = 0; i < canvas.childCount; ++i)
            {
                canvas.GetChild(i).SetSiblingIndex(i);
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if(collision.transform.parent != null)
        {
            if (collision.transform.parent.name != "ShapeCanvas")
                    {
                        Debug.Log($"UnStuck from: {collision.gameObject.name}");

                        this.transform.parent = oldParent;
                        EnableRagdoll();
                        Debug.Log($"Unfrozen");
                    }
        }
    }

    void EnableRagdoll()
    {
        rBody.isKinematic = false;
        rBody.detectCollisions = true;
    }

    void DisableRagdoll()
    {
        rBody.isKinematic = true;
        rBody.detectCollisions = false;
    }
}

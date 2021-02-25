using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;
using System;
using TMPro;

public class HandTrackingGrabber : OVRGrabber
{
    private OVRHand hand;
    [SerializeField]
    private CreateShape createShape;
    [SerializeField]
    private Collider[] fingers;
    [SerializeField]
    private TextMeshPro pinchStrengthText;
    [SerializeField]
    private TextMeshPro gripStrengthText;
    public float pinchTolerance = 0.2f;
    public float gripTolerance = 0.8f;
    public float touchDist = 0.05f;
    public bool isGrasping;
    public bool isPinching;
    public Dictionary<OVRGrabbable, int> grabbables;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        hand = GetComponent<OVRHand>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        grabbables = m_grabCandidates;
        isGrasping = getGripValue() > gripTolerance;
        isPinching = GetComponent<OVRHand>().GetFingerPinchStrength(OVRHand.HandFinger.Index) > pinchTolerance;
        gripStrengthText.text = $"Grip Strength: {getGripValue()}, Grab Candidates: {m_grabCandidates.Count} \n Fingers touching: {maxFingersTouching()}";
        pinchStrengthText.text = $"Pinch Strength: {GetComponent<OVRHand>().GetFingerPinchStrength(OVRHand.HandFinger.Index)}";
        CheckIndexPinched();
    }

    void CheckIndexPinched()
    {
        if(!m_grabbedObj && isGrasping && m_grabCandidates.Count > 0 && !createShape.createMode)
        {
            createShape.currentObj.GetComponent<Rigidbody>().detectCollisions = false;
            GrabBegin();
        } else if (m_grabbedObj && !isGrasping)
        {
            GrabEnd();
            //System.Threading.Thread.Sleep(1000);
            createShape.currentObj.GetComponent<Rigidbody>().detectCollisions = true;
        }
    }

    private float getGripValue()
    {
        var gripTotal = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index) +
            hand.GetFingerPinchStrength(OVRHand.HandFinger.Middle) +
            hand.GetFingerPinchStrength(OVRHand.HandFinger.Ring) +
            hand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky);
        return gripTotal;
    }

    private int maxFingersTouching()
    {
        var max = 0;
        foreach (var candidate in m_grabCandidates.Keys)
        {
            var count = 0;
            foreach (var finger in fingers)
            {
                if (Vector3.Distance(finger.transform.position, candidate.transform.position) < touchDist)
                {
                    count++;
                }
            }
            if (count > max)
            {
                max = count;
            }
        }

        return max;
    }

    //protected override void GrabEnd()
    //{
    //    if (m_grabbedObj)
    //    {

    //        //OVRPose localPose = new OVRPose { position = hand.transform.position, orientation = hand.transform.rotation };
    //        //OVRPose offsetPose = new OVRPose { position = m_anchorOffsetPosition, orientation = m_anchorOffsetRotation };
    //        //localPose = localPose * offsetPose;

    //        //OVRPose trackingSpace = transform.ToOVRPose() * localPose.Inverse();
    //        //Vector3 linearVelocity = hand.GetComponent<Rigidbody>().velocity;
    //        //Vector3 angularVelocity = hand.GetComponent<Rigidbody>().angularVelocity;

    //        var scale = m_grabbedObj.transform.localScale;
    //        var volume = scale.x * scale.y * scale.z;
    //        var mult = 1;// 0.01f / Mathf.Pow(volume, 1f/3f);

    //        Vector3 linearVelocity = mult * (transform.position - m_lastPos) / Time.fixedDeltaTime;
    //        Vector3 angularVelocity = mult * (transform.eulerAngles - m_lastRot.eulerAngles) / Time.fixedDeltaTime;

    //        GrabbableRelease(linearVelocity, angularVelocity);
    //    }

    //    GrabVolumeEnable(true);
    //}
}

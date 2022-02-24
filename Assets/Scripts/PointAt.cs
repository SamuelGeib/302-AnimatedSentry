using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PointAt : MonoBehaviour
{
    // This is the target object of the object that has the script on it.
    public Transform target;

    // locks rotation in different axes
    public bool lockAxisX = false;
    public bool lockAxisY = false;
    public bool lockAxisZ = false;


    private Quaternion startRotation;
    private Quaternion goalRotation; // the position we ease TOWARDS

    //private PlayerTargeting playerTargeting;

    void Start()
    {
        startRotation = transform.localRotation;
    }


    void Update()
    {
        TurnTowardsTarget();
    }

    private void TurnTowardsTarget()
    {
        if(target != null) {

            Vector3 vToTarget = target.position - transform.position;
            vToTarget.Normalize();

            Quaternion worldRot = Quaternion.LookRotation(vToTarget, Vector3.up); // Calculate the world rotation
            Quaternion localRot = worldRot; // Set the localrotation to the world rotation when there is no parent
            if (transform.parent)
            {
                //  convert to local space:
                localRot = Quaternion.Inverse(transform.parent.rotation) * worldRot;
            }
            Vector3 euler = localRot.eulerAngles;

            // Do not allow locked axiix to rotate by keeping them equal to their starting position.
            if (lockAxisX) euler.x = startRotation.eulerAngles.x;
            if (lockAxisX) euler.y = startRotation.eulerAngles.y;
            if (lockAxisX) euler.z = startRotation.eulerAngles.z;


            localRot.eulerAngles = euler;

            goalRotation = localRot;
        } else
        {
            goalRotation = startRotation; // Resets arm
        }

        transform.localRotation = AnimMath.Ease(transform.localRotation, goalRotation, .001f);
    }
}

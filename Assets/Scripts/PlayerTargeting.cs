using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    public float visionDistance = 10;

    public TargetableObject target { get; private set; } // "{ get; private set; }" makes this publicly readable but not writeable
    public bool playerWantsToAim { get; private set; }  = false; 

private List<TargetableObject> validTargets = new List<TargetableObject>();

    private float cooldownScan = 0; // How much time until we scan for targetable objects
    private float cooldownPickTarget = 0;



    void Update()
    {
        playerWantsToAim = Input.GetButton("Fire2"); // Returns a boolean, true if Right Mouse Button, false if not


        cooldownScan -= Time.deltaTime;
        cooldownPickTarget -= Time.deltaTime;


        if (playerWantsToAim)
        {
            if (cooldownScan <= 0) ScanForTargets();
            if (cooldownPickTarget <= 0) PickATarget();

        }
        else
        {
            target = null;
        }

        print(target);
    }

    // Look for all valid targets, and add them to a list
    void ScanForTargets()
    {
        cooldownScan = 0.5f;
        validTargets.Clear();

        TargetableObject[] things = GameObject.FindObjectsOfType<TargetableObject>();
        foreach (TargetableObject thing in things)
        {
            Vector3 vToThing = thing.transform.position - transform.position; // Draw a vector between the thing to lookat and the player

            // Tests to see if the target is close enough to see
            if (vToThing.magnitude < visionDistance * visionDistance) {

                float alignment = Vector3.Dot(transform.forward, vToThing.normalized); 

                // is within 180 degrees of forward
                if(alignment > .4f) { // .4f is 40% of 180 degrees
                    validTargets.Add(thing);
                }
            }
        }
    }

    // Look through list of valid targets, and selects one to target
    void PickATarget()
    {
        if (target) return;

        float closestDistanceSoFar = visionDistance;

        foreach (TargetableObject thing in validTargets)
        {
            Vector3 vToThing = thing.transform.position - transform.position; // Draw a vector between the thing to lookat and the player

            float dis = vToThing.sqrMagnitude; // distance squared

            if (dis < closestDistanceSoFar || target == null)
            {

                closestDistanceSoFar = dis;
                target = thing;
            }
        }

    }
}
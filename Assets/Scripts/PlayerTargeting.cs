using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    public float visionDistance = 10;
    [Range(1, 20)]
    public int roundsPerSecond = 5;

    public Transform boneShoulderRight;
    public Transform boneShoulderLeft;

    public TargetableObject target { get; private set; } // "{ get; private set; }" makes this publicly readable but not writeable
    public bool playerWantsToAim { get; private set; }  = false; 
    public bool playerWantsToAttack { get; private set; }  = false;

    private List<TargetableObject> validTargets = new List<TargetableObject>();

    private float cooldownScan = 0; // How much time until we scan for targetable objects
    private float cooldownPickTarget = 0;
    private float cooldownAttack = 0; 



    void Update()
    {
        playerWantsToAttack = Input.GetButton("Fire1"); // LMB
        playerWantsToAim = Input.GetButton("Fire2"); // Returns a boolean, true if Right Mouse Button, false if not


        cooldownScan -= Time.deltaTime;
        cooldownPickTarget -= Time.deltaTime;
        cooldownAttack -= Time.deltaTime;


        if (playerWantsToAim)
        {

            if (target != null)
            {
                if(!CanSeeThing(target))
                {
                    target = null;
                }
            }
            
            if (cooldownScan <= 0) ScanForTargets();
            if (cooldownPickTarget <= 0) PickATarget();

        }
        else
        {
            target = null;
        }

         DoAttack();
    }

    void DoAttack()
    {
        if (cooldownAttack > 0) return;
        if (!playerWantsToAim) return;
        if (!playerWantsToAttack) return;
        if (target == null) return;
        if (!CanSeeThing(target)) return;

        cooldownAttack = 1f / roundsPerSecond;

        // spawn Projectiles..
        // Or subtract health from target...

        // Animate Arm Kickback
        boneShoulderLeft.localEulerAngles += new Vector3(-30, 0, 0);
        boneShoulderRight.localEulerAngles += new Vector3(-30, 0, 0);
    }

    // Look for all valid targets, and add them to a list
    void ScanForTargets()
    {
        cooldownScan = 0.5f;
        validTargets.Clear();

        TargetableObject[] things = GameObject.FindObjectsOfType<TargetableObject>();
        foreach (TargetableObject thing in things)
        {
            if (CanSeeThing(thing))
            {
                validTargets.Add(thing);
            }
           }
    }

    private bool CanSeeThing(TargetableObject thing)
    {
        Vector3 vToThing = thing.transform.position - transform.position; // Draw a vector between the thing to lookat and the player

        // Is too far to see
        if (vToThing.sqrMagnitude > visionDistance * visionDistance) return false;
        
        // How much is in-front of player?
        float alignment = Vector3.Dot(transform.forward, vToThing.normalized);

        // is within 40% of 180 degrees of forward
        if (alignment < .4f) return false;
        
        return true;
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
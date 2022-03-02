using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TurretController : MonoBehaviour
{


    // Bone References
    public Transform boneBase;
    public Transform boneDrum;


    private currentState state = currentState.idle;

    private float distanceToPlayer;
    
    public float activationRadius = 25; // The distance to player that determines when the turret activates.


    PlayerTargeting player; // Reference to the player

    public enum currentState
    {
        idle,
        scanning,
        targetLocking,
        targetLocked,
        firing,
        disabled,
        dead
    };

    void Start()
    {
        player = FindObjectOfType<PlayerTargeting>();




    }

    void Update()
    {
        // Control the state of the turret
        switch (state)
        {
            case currentState.idle:
                StateIdleUpdate();
                

                // Switch out of Idle State
                if(Vector3.Distance(transform.position, player.transform.position) <= activationRadius) {
                    state = currentState.scanning;
                }
                break;
            case currentState.scanning:
                StateScanningUpdate();

                break;
            case currentState.targetLocking:
                StateTargetLockingUpdate();

                break;
            case currentState.targetLocked:
                StateTargetLockedUpdate();

                break;
            case currentState.firing:
                StateFiringUpdate();

                break;
            case currentState.disabled:
                StateDisabledUpdate();

                break;
            case currentState.dead:
                StateDeadUpdate();

                break;

        }

        // Debug: Current State
        Debug.Log("Current State: " + state);
    }
    /// <summary>
    /// Controls which state the turret is currently in.
    /// </summary>

    // Idle State Update Function
    private void StateIdleUpdate() {

    }

    // Scanning State Update Function
    private void StateScanningUpdate()
    {

    }

    // Target-Locking State Update Function
    private void StateTargetLockingUpdate()
    {

    }

    // Target-Locked State Update Function
    private void StateTargetLockedUpdate()
    {

    }
    // Firing State Update Function
    private void StateFiringUpdate()
    {

    }
    // Disabled State Update Function
    private void StateDisabledUpdate()
    {

    }
    // Dead State Update Function
    private void StateDeadUpdate()
    {

    }
}

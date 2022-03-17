using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TurretController : MonoBehaviour
{


    // Bone References
    public Transform boneBase;
    public Transform boneDrum;


    

    // All Possible States
    const int IDLE_STATE = 0;
    const int SCAN_STATE = 1;
    const int ATTACK_STATE = 2;
    const int DEAD_STATE = 3;

    [Range(0, 3)]
    public int currentState = IDLE_STATE;

    private float distanceToPlayer;
    
    public float activationRadius = 25; // The distance to player that determines when the turret activates.


    PlayerTargeting player; // Reference to the player

    void Start()
    {
        player = FindObjectOfType<PlayerTargeting>();
    }

    void Update()
    {
        // Turret States:
        switch (currentState)
        {
            case IDLE_STATE:
                

                // Switch out of Idle State
                if(Vector3.Distance(transform.position, player.transform.position) <= activationRadius) {
                    currentState = SCAN_STATE;
                }
                break;

            case SCAN_STATE:

                break;


            case ATTACK_STATE:
                

                break;

            case DEAD_STATE:

                break;

        }

    }
}

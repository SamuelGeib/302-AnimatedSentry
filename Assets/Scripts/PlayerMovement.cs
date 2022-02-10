using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    // controls the speed at which the player walks
    public float walkSpeed = 5;

    public Camera cam;

    CharacterController pawn; // Reference to the character's physics enchine controller
    void Start()
    {
        pawn = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Input
        float v = Input.GetAxisRaw("Vertical"); // Vertical Input "W" and "S" Range: [-1,1]
        float h = Input.GetAxisRaw("Horizontal"); // Horizontal Input "A" and "D" Range: [-1,1]

        bool playerWantsToMove = (v != 0 || h != 0);

        // Set rotation
        if (cam && playerWantsToMove)
        {
            float playerYaw = transform.eulerAngles.y; // Player's Y rotation
            float camYaw = cam.transform.eulerAngles.y; // Camera's Y Rotation

            /* Bug Fix Attempt: Camera player Rotation Angle weirdness
            while (camYaw > playerYaw + 180) camYaw -= 360;
            while (camYaw < playerYaw - 180) camYaw += 360;
            */
            print($"camYaw: {camYaw} playerYaw: {playerYaw}");


            Quaternion targetRotation = Quaternion.Euler(0, camYaw, 0); // Set target rotation for player
            transform.rotation = AnimMath.Ease(transform.rotation, targetRotation, .01f); // Ease towards target rotation
        }
        // Movement

        Vector3 moveDir = transform.forward * v + transform.right * h; // Calculates the direction we want to move
        if (moveDir.magnitude > 1) moveDir.Normalize(); // Clamp the movement vector to 1 if it is greater than 1 (consistent speed in any direction) would we still accelerate up to out movement speed faster in the diagonal directions?
        
        pawn.SimpleMove(moveDir * walkSpeed); // SimpleMove() Automatically calculates fot Delta Time, and calculates Gravity 

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    public Transform boneLegLeft;
    public Transform boneLegRight;
    public Transform boneHip;
    public Transform boneSpine;

    public float walkSpeed = 5;
    [Range(-15, -5)]
    public float gravity = -10;

    public Camera cam;

    CharacterController pawn; // Reference to the character's physics enchine controller
    PlayerTargeting targetingScript;

    private Vector3 inputDir;
    private float velocityVertical; // meters/second

    private float cooldownJumpWindow = 0;
    public bool IsGrounded {
        get{
            return pawn.isGrounded || cooldownJumpWindow > 0;
        }
    }


    void Start()
    {
        pawn = GetComponent<CharacterController>();
        targetingScript = GetComponent<PlayerTargeting>();
    }

    void Update()
    {

        if (cooldownJumpWindow > 0) cooldownJumpWindow -= Time.deltaTime;

        // Lateral Movement:

        // Input
        float v = Input.GetAxisRaw("Vertical"); // Vertical Input "W" and "S" Range: [-1,1]
        float h = Input.GetAxisRaw("Horizontal"); // Horizontal Input "A" and "D" Range: [-1,1]
        
        bool playerWantsToMove = (v != 0 || h != 0);

        bool playerisAiming = (targetingScript && targetingScript.playerWantsToAim && targetingScript.target);

        if (playerisAiming) {

            Vector3 totarget = targetingScript.target.transform.position - transform.position;
            totarget.Normalize();
            
            Quaternion worldRot = Quaternion.LookRotation(totarget);
            Vector3 euler = worldRot.eulerAngles;
            euler.x = 0;
            euler.y = 0;
            worldRot.eulerAngles = euler;
            transform.rotation = AnimMath.Ease(transform.rotation, worldRot, .01f);

        } 
        else if (cam && playerWantsToMove) {
            float playerYaw = transform.eulerAngles.y; // Player's Y rotation
            float camYaw = cam.transform.eulerAngles.y; // Camera's Y Rotation

            
            while (camYaw > playerYaw + 180) camYaw -= 360;
            while (camYaw < playerYaw - 180) camYaw += 360;
            
            print($"camYaw: {camYaw} playerYaw: {playerYaw}");

            Quaternion playerRotation = Quaternion.Euler(0, playerYaw, 0); // Player's current rotation
            Quaternion targetRotation = Quaternion.Euler(0, camYaw, 0); // Set target rotation for player
            
            transform.rotation = AnimMath.Ease(playerRotation, targetRotation, .01f); // Ease towards target rotation
        }
        // Movement

        inputDir = transform.forward * v + transform.right * h; // Calculates the direction we want to move
        if (inputDir.magnitude > 1) inputDir.Normalize(); // Clamp the movement vector to 1 if it is greater than 1 (consistent speed in any direction) would we still accelerate up to out movement speed faster in the diagonal directions?


        // Vertical Movement:
        bool wantsToJump = Input.GetButtonDown("Jump"); // Space
        if(pawn.isGrounded) {
            if (wantsToJump)
            {
                cooldownJumpWindow = 0;
                velocityVertical = 5;
            }
        }
        velocityVertical += gravity * Time.deltaTime;
        

        // Total Movement of player:
        Vector3 moveAmount = inputDir * walkSpeed + Vector3.up * velocityVertical;
        pawn.Move(moveAmount * Time.deltaTime); // SimpleMove() Automatically calculates fot Delta Time, and calculates Gravity 

        if (pawn.isGrounded)
        {
            cooldownJumpWindow = .5f;
            velocityVertical = 0;
            WalkAnimation();
        } else {
            AirAnimation();
        }

    }

    void WalkAnimation() {



        Vector3 inputDirLocal = transform.InverseTransformDirection(inputDir);
        Vector3 axis = Vector3.Cross(Vector3.up, inputDirLocal);


        float alignment = Vector3.Dot(inputDirLocal, Vector3.forward);
        alignment = Mathf.Abs(alignment); // absolute value

        float degrees = AnimMath.Lerp(10, 40, alignment);
        float speed = 10;
        float wave = Mathf.Sin(Time.time * speed) * degrees; // Oscilates between -degrees and degrees

        
        boneLegLeft.localRotation = Quaternion.AngleAxis(wave, axis);
        boneLegRight.localRotation = Quaternion.AngleAxis(-wave, axis);

        if (boneHip) {
            float walkBobAmount = axis.magnitude; // The amount that the player bobs while they walk
            float offsetY = Mathf.Cos(Time.time * speed) * walkBobAmount * .05f;
            boneHip.localPosition = new Vector3(0, offsetY, 0);
        }
    }
    
    void AirAnimation() {

    }
}

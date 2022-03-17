using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // Bones
    public Transform boneHip;
    public Transform boneLegLeft;
    public Transform boneLegRight;
    // Body Parts
    public Rigidbody Head;
    public Rigidbody Chest;
    public Rigidbody Midsection;
    public Rigidbody RightArm;
    public Rigidbody RightLeg;
    public Rigidbody LeftArm;
    public Rigidbody LeftLeg;


    // Movement States
    const int IDLE_STATE = 0;
    const int WALK_STATE = 1;
    const int RUN_STATE = 2;
    const int JUMP_STATE = 3;
    const int ATTACK_STATE = 4;
    const int DEAD_STATE = 5;


    [Range(0, 4)]
    public int currentState = IDLE_STATE;

    // STATS / VARIABLES
    public float maxHealth = 100;
    public float currHealth;
    public float moveSpeed = 5;
    public float speedMultiplier;
    bool isDead = false;

    [Range(-15, -5)]
    public float gravity = -10;

    public Camera cam;

    CharacterController pawn; // Reference to the character's physics enchine controller
    PlayerTargeting targetingScript;

    private Vector3 inputDir;
    private float velocityVertical; // meters/second

    private float cooldownJumpWindow = 0;
    public bool IsGrounded {
        get {
            return pawn.isGrounded || cooldownJumpWindow > 0;
        }
    }


    void Start()
    {
        pawn = GetComponent<CharacterController>();
        targetingScript = GetComponent<PlayerTargeting>();
        currHealth = maxHealth;
    }

    void Update() {

        switch (currentState) {
            //  Idle State Update Function
            case IDLE_STATE:

                // Switch Cases:
                if (playerWantsToMove()) currentState = WALK_STATE;
                if (canJump() && wantsToJump()) currentState = JUMP_STATE;
                break;

            //  Walk State Update Function
            case WALK_STATE:
                speedMultiplier = 1;
                WalkAnimation();
                // Switch Cases:
                if (!playerWantsToMove()) currentState = IDLE_STATE;
                if (playerWantsToRun()) currentState = RUN_STATE;
                if (canJump() && wantsToJump()) currentState = JUMP_STATE;
                break;

            //  Jump State Update Function
            case JUMP_STATE:
                jump();
                JumpAnimation();

                // Switch Cases:
                if (pawn.isGrounded) currentState = IDLE_STATE;

                break;

            //  Run State Update Function
            case RUN_STATE:
                speedMultiplier = 2;
                WalkAnimation();
                // Switch Cases:
                if (!playerWantsToRun()) currentState = IDLE_STATE;
                if (canJump() && wantsToJump()) currentState = JUMP_STATE;
                break;

            //  Attack State Update Function
            case ATTACK_STATE:

                break;

            //  Dead State Update Function
            case DEAD_STATE:
                DeathAnimation();

                break;
        } // State Machine for player states

        // Anything that is consistent in all or most states
        calcPlayerMovement();
        calcJumpCooldown();
        calcInputDirection();
        shouldDie();

        
        if (isPlayerAiming()) {
            aimCameraOverShoulder();
        }
        else if (cam && playerWantsToMove()) {
            aimCameraBehindPlayer();
        }

        

        if (pawn.isGrounded)
        {
            cooldownJumpWindow = .5f;
            velocityVertical = 0;
            
        }
        else
        {
            JumpAnimation();
        }

    }


    void calcInputDirection() {

        // Get Input X, Y
        float v = Input.GetAxisRaw("Vertical"); // Vertical Input "W" and "S" Range: [-1,1]
        float h = Input.GetAxisRaw("Horizontal"); // Horizontal Input "A" and "D" Range: [-1,1]

        // Calculate and Clamp the inputDir vector
        inputDir = transform.forward * v + transform.right * h; // Calculates the direction we want to move as a 3D Vector
        if (inputDir.magnitude > 1) inputDir.Normalize(); // Clamp the movement vector to 1 if it is greater than 1 (consistent speed in any direction) would we still accelerate up to out movement speed faster in the diagonal directions?
    } 
    void calcJumpCooldown() {
        if (cooldownJumpWindow > 0) cooldownJumpWindow -= Time.deltaTime;
        } 
    void calcPlayerMovement()
    {
        if (isDead) return;

        // Calculate Movement
        Vector3 moveAmount = inputDir * moveSpeed * speedMultiplier + Vector3.up * velocityVertical;
        pawn.Move(moveAmount * Time.deltaTime);

        // Calculate Gravity
        velocityVertical += gravity * Time.deltaTime;
    } 
    void jump()
    {
        if (pawn.isGrounded)
        {
            if (wantsToJump())
            {
                cooldownJumpWindow = 0;
                velocityVertical = 5;
            }
        }
    }
    void aimCameraOverShoulder()
    {
        Vector3 totarget = targetingScript.target.transform.position - transform.position;
        totarget.Normalize();

        Quaternion worldRot = Quaternion.LookRotation(totarget);
        Vector3 euler = worldRot.eulerAngles;
        euler.x = 0;
        euler.y = 0;
        worldRot.eulerAngles = euler;
        transform.rotation = AnimMath.Ease(transform.rotation, worldRot, .01f);
    }
    void aimCameraBehindPlayer()
    {

        float playerYaw = transform.eulerAngles.y; // Player's Y rotation
        float camYaw = cam.transform.eulerAngles.y; // Camera's Y Rotation

        while (camYaw > playerYaw + 180) camYaw -= 360;
        while (camYaw < playerYaw - 180) camYaw += 360;

        Quaternion playerRotation = Quaternion.Euler(0, playerYaw, 0); // Player's current rotation
        Quaternion targetRotation = Quaternion.Euler(0, camYaw, 0); // Set target rotation for player

        transform.rotation = AnimMath.Ease(playerRotation, targetRotation, .01f); // Ease towards target rotation
    }

    // Tests
    bool playerWantsToMove() {
        if (inputDir.x != 0 || inputDir.y != 0) return true;
        return false;
    }
    bool wantsToJump() {
        if (Input.GetButtonDown("Jump")) return true;
        return false;
    }
    bool canJump() {

        if (pawn.isGrounded) return true;
        return false;
    }
    bool playerWantsToRun()
    {
        if (Input.GetButton("Fire1")) return true;
        return false;
    }
    bool isPlayerAiming() {
        if (targetingScript && targetingScript.playerWantsToAim && targetingScript.target) return true;
        return false;
    }
    bool shouldDie()
    {
        if (currHealth <= 0) return true;
        return false;
    }

    // Animation Functions
    void WalkAnimation() {



        Vector3 inputDirLocal = transform.InverseTransformDirection(inputDir);
        Vector3 axis = Vector3.Cross(Vector3.up, inputDirLocal);


        float alignment = Vector3.Dot(inputDirLocal, Vector3.forward);
        alignment = Mathf.Abs(alignment); // absolute value

        float degrees = AnimMath.Lerp(10, 40, alignment);
        float speed = 10 * speedMultiplier;
        float wave = Mathf.Sin(Time.time * speed) * degrees; // Oscilates between -degrees and degrees

        
        boneLegLeft.localRotation = Quaternion.AngleAxis(wave, axis);
        boneLegRight.localRotation = Quaternion.AngleAxis(-wave, axis);

        if (boneHip) {
            float walkBobAmount = axis.magnitude; // The amount that the player bobs while they walk
            float offsetY = Mathf.Cos(Time.time * speed) * walkBobAmount * .05f;
            boneHip.localPosition = new Vector3(0, offsetY, 0);
        }
    }
    
    void JumpAnimation() {

    }
    void DeathAnimation() {
    Head.useGravity = true;
    Chest.useGravity = true;
    Midsection.useGravity = true;
    RightArm.useGravity = true;
    RightLeg.useGravity = true;
    LeftArm.useGravity = true;
    LeftLeg.useGravity = true;
    }
}

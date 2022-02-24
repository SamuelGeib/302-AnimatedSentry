using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Reference to PlayerTargeting Script called "player"
    public PlayerTargeting player;

    public Vector3 targetOffset;

    public float mouseSensitivityX = 5;
    public float mouseSensitivityY = 5;
    public float mouseSensitivityScroll = 5;

    private Camera cam;

    private float pitch = 0; // x rotation
    private float yaw = 0; // y rotation
    private float dollyDis = 10; // Distance in Z-axis between camera and target 
    private float shakeTimer;

    void Start()
    {
        cam = GetComponentInChildren<Camera>(); // We need "Inchildren" because the camera is a child object of the camera controller
        if(player == null) {
            PlayerTargeting script = FindObjectOfType<PlayerTargeting>();
            if (script != null) player = script;
        }
    }

    void Update()
    {
        // is the player aiming?
        bool isAiming = (player && player.target && player.playerWantsToAim);

        // 1. ease position of Camera Rig towards player:

        if (player) // Tests if the target exists
        {
            transform.position = AnimMath.Ease(transform.position, player.transform.position + targetOffset, .01f); // Use our animMath class to ease our target to the player
        }

        // 2. Set Camera Rig Rotation:
        float playerYaw = AnimMath.AngleWrapDegrees(yaw, player.transform.eulerAngles.y);

        if (isAiming)
        {

            Quaternion tempPlayer = Quaternion.Euler(0, playerYaw, 0);

            transform.rotation = AnimMath.Ease(transform.rotation, tempPlayer, .001f);

        }
        else
        {
            float mx = Input.GetAxis("Mouse X"); // Mouse Delta X, change in Mouse-X-position
            float my = Input.GetAxis("Mouse Y"); // Mouse Delta X, change in Mouse-X-position

            yaw += mx * mouseSensitivityX;
            pitch += my * -mouseSensitivityY;

            //if (yaw > 360) yaw -= 360;
            //if (yaw < 0) yaw += 360;


            pitch = Mathf.Clamp(pitch, -10, 89); // Lock the camera from looking too far up or down
            transform.rotation = AnimMath.Ease(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .001f);

        }
        // 3. Dolly Camera In/Out:
        
            dollyDis += Input.mouseScrollDelta.y * mouseSensitivityScroll; // dollyDis is the target dis
            dollyDis = Mathf.Clamp(dollyDis, 3, 20); // Value is in Meters

        // Turnary Opperator 
        float tempZ = isAiming ? 2 : dollyDis;

        // This block is one line of code @.@ not using the ; has let us split it across multiple lines
        cam.transform.localPosition = AnimMath.Ease(
            cam.transform.localPosition,
            new Vector3(0, 0, -tempZ),
            .02f);

        // 4. rotate the camera object (for target aiming)

        if(isAiming)
        {
            // point at target:
            Vector3 vToAimTarget = player.target.transform.position - cam.transform.position;
            Quaternion worldRot = Quaternion.LookRotation(vToAimTarget);
            Quaternion localRot = worldRot;

            if(cam.transform.parent) {
                localRot = Quaternion.Inverse(cam.transform.parent.rotation) * worldRot;
            }

            Vector3 euler = localRot.eulerAngles;
            euler.z = 0;
            localRot.eulerAngles = euler;

            cam.transform.localRotation = AnimMath.Ease(cam.transform.localRotation, localRot, .001f);

               
        } else
        {
            cam.transform.localRotation = AnimMath.Ease(cam.transform.localRotation, Quaternion.identity, 0.001f);
        }
        UpdateShake();
    }

    void UpdateShake() {
        if (shakeTimer < 0) return;

        shakeTimer -= Time.deltaTime;

        float p = shakeTimer / 1;
        p = p * p;


        p = AnimMath.Lerp(1, .98f, p);

        Quaternion randomRot = AnimMath.Lerp(Random.rotation, Quaternion.identity, p);

        cam.transform.localRotation *= randomRot; 
    }

    public void Shake(float time) {
        // if (time > shakeTimer)
        shakeTimer += time;
    }

    private void OnDrawGizmos()
    {
        if(!cam) cam = cam = GetComponentInChildren<Camera>();
        if (!cam) return;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
        Gizmos.DrawLine(transform.position, cam.transform.position);
    }
}

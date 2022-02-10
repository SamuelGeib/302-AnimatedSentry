using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public Vector3 targetOffset;

    public float mouseSensitivityX = 5;
    public float mouseSensitivityY = 5;
    public float mouseSensitivityScroll = 5;

    private Camera cam;

    private float pitch = 0; // x rotation
    private float yaw = 0; // y rotation
    private float dollyDis = 10; // Distance in Z-axis between camera and target 


    void Start()
    {
        cam = GetComponentInChildren<Camera>(); // We need "Inchildren" because the camera is a child object of the camera controller
    }

    void Update()
    {
        

        if (target) // Tests if the target exists
        {
            transform.position = AnimMath.Ease(transform.position, target.position + targetOffset, .01f); // Use our animMath class to ease our target to the player
        }

        // 2. Set Rotation (TODO: ease):
        float mx = Input.GetAxis("Mouse X"); // Mouse Delta X, change in Mouse-X-position
        float my = Input.GetAxis("Mouse Y"); // Mouse Delta X, change in Mouse-X-position

        yaw += mx * mouseSensitivityX;
        pitch += my * -mouseSensitivityY;

        pitch = Mathf.Clamp(pitch, -10, 89); // Lock the camera from looking too far up or down

        transform.rotation = Quaternion.Euler(pitch, yaw, 0);

        // 3. Dolly Camera In/Out
        dollyDis += Input.mouseScrollDelta.y * mouseSensitivityScroll; // dollyDis is the target dis
        dollyDis = Mathf.Clamp(dollyDis, 3, 20); // Value is in Meters

        // This block is one line of code @.@ not using the ; has let us split it across multiple lines
        cam.transform.localPosition = AnimMath.Ease(
            cam.transform.localPosition,
            new Vector3(0, 0, -dollyDis),
            .02f);

    }

    // This function is called BEFORE Start
    private void OnDrawGizmos()
    {
        if(!cam) cam = cam = GetComponentInChildren<Camera>();
        if (!cam) return;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
        Gizmos.DrawLine(transform.position, cam.transform.position);
    }
}

using UnityEngine;

/// <summary>
/// Simple fly camera controller to easily inspect the street scene, animals, and lights in Play mode.
/// Controls:
/// - Right Click + Mouse: Look around
/// - W/A/S/D: Move Forward/Left/Backward/Right
/// - Q/E or Space/LeftControl: Move Down/Up
/// - Left Shift: Fast fly
/// </summary>
public class FreeFlyCamera : MonoBehaviour
{
    public float movementSpeed = 10f;
    public float fastSpeedMultiplier = 3f;
    public float lookSensitivity = 2.5f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    private void Start()
    {
        Vector3 rot = transform.localEulerAngles;
        rotationX = rot.x;
        rotationY = rot.y;
    }

    private void Update()
    {
        // Camera rotation with Right Mouse Button
        if (Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            rotationX -= Input.GetAxis("Mouse Y") * lookSensitivity;
            rotationX = Mathf.Clamp(rotationX, -85f, 85f);
            rotationY += Input.GetAxis("Mouse X") * lookSensitivity;

            transform.localEulerAngles = new Vector3(rotationX, rotationY, 0f);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Camera movement
        float speed = movementSpeed * (Input.GetKey(KeyCode.LeftShift) ? fastSpeedMultiplier : 1f);

        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) move += transform.forward;
        if (Input.GetKey(KeyCode.S)) move -= transform.forward;
        if (Input.GetKey(KeyCode.D)) move += transform.right;
        if (Input.GetKey(KeyCode.A)) move -= transform.right;
        if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Space)) move += Vector3.up;
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftControl)) move -= Vector3.up;

        transform.position += move * (speed * Time.deltaTime);
    }
}

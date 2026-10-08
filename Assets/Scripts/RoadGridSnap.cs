using UnityEngine;

/// <summary>
/// Helper component that snaps modular road sections or streetlights to grid units in the Unity Editor.
/// </summary>
[ExecuteInEditMode]
public class RoadGridSnap : MonoBehaviour
{
    [Tooltip("Snap interval in X axis (road width spacing)")]
    public float snapX = 6.0f;

    [Tooltip("Snap interval in Z axis (road piece length)")]
    public float snapZ = 10.0f;

    public bool enableSnapping = true;

    private void Update()
    {
        if (!Application.isPlaying && enableSnapping)
        {
            Vector3 pos = transform.position;
            float newX = Mathf.Round(pos.x / snapX) * snapX;
            float newZ = Mathf.Round(pos.z / snapZ) * snapZ;
            transform.position = new Vector3(newX, pos.y, newZ);
        }
    }
}

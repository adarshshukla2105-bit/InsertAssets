using UnityEngine;

/// <summary>
/// Controls a street light's lamp illumination and emissive material glow.
/// Supports Universal Render Pipeline (URP) materials with HDR emission.
/// </summary>
public class StreetLightController : MonoBehaviour
{
    [Header("Light Component")]
    [Tooltip("The Point Light or Spot Light under the street lamp head")]
    public Light lightSource;

    [Header("Bulb / Fixture Mesh")]
    [Tooltip("MeshRenderer that contains the glowing bulb material")]
    public MeshRenderer bulbRenderer;
    [Tooltip("Index of the emissive material in the MeshRenderer's material list")]
    public int materialIndex = 0;

    [Header("Emission Colors (URP Lit HDR)")]
    [ColorUsage(true, true)]
    public Color lightOnEmission = new Color(2.0f, 1.6f, 0.9f) * 3.0f; // Warm street lamp glow
    public Color lightOffEmission = Color.black;

    [Header("State")]
    public bool isTurnedOn = true;
    public float defaultLightIntensity = 15.0f;

    [Header("Optional Subtle Flicker")]
    public bool enableFlicker = false;
    public float flickerSpeed = 10.0f;
    public float flickerAmount = 0.2f;

    private Material bulbMaterial;
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        if (lightSource == null)
        {
            lightSource = GetComponentInChildren<Light>();
        }

        if (bulbRenderer != null)
        {
            // Use instance material so it doesn't affect other objects sharing the asset
            bulbMaterial = bulbRenderer.materials[materialIndex];
        }

        UpdateLightState();
    }

    private void Update()
    {
        if (isTurnedOn && enableFlicker && lightSource != null)
        {
            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f);
            float factor = 1.0f - (noise * flickerAmount);
            lightSource.intensity = defaultLightIntensity * factor;
        }
    }

    /// <summary>
    /// Turn the street light on or off dynamically (e.g., day/night cycle)
    /// </summary>
    public void SetLightState(bool state)
    {
        isTurnedOn = state;
        UpdateLightState();
    }

    private void UpdateLightState()
    {
        if (lightSource != null)
        {
            lightSource.enabled = isTurnedOn;
            lightSource.intensity = defaultLightIntensity;
        }

        if (bulbMaterial != null)
        {
            Color targetColor = isTurnedOn ? lightOnEmission : lightOffEmission;
            bulbMaterial.SetColor(EmissionColorId, targetColor);

            if (isTurnedOn)
            {
                bulbMaterial.EnableKeyword("_EMISSION");
            }
            else
            {
                bulbMaterial.DisableKeyword("_EMISSION");
            }
        }
    }
}

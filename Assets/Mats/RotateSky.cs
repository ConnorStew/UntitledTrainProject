using UnityEngine;

/// <summary>
/// This class handles rotating the skybox.
/// </summary>
public class RotateSky : MonoBehaviour
{
    /// <summary> The speed to rotate the skybox at. </summary>
    private float RotateSpeed = 1.2f;

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * RotateSpeed);
    }
}

using UnityEngine;

public class PanoramaViewer : MonoBehaviour
{
    [Header("Assign the Sphere GameObject")]
    public GameObject panoramaSphere;

    [Header("Gyro Settings")]
    public float rotationSensitivity = 1.0f;

    private Quaternion _startingOrientation;
    private bool _initialized = false;

    void Start()
    {
        Input.gyro.enabled = true;
        if (panoramaSphere != null)
            _startingOrientation = panoramaSphere.transform.rotation;
        _initialized = true;
    }

    void Update()
    {
        if (!_initialized || panoramaSphere == null) return;

        Quaternion gyroAttitude = Input.gyro.attitude;
        Quaternion correctedRotation = Quaternion.Inverse(gyroAttitude) * _startingOrientation;

        panoramaSphere.transform.rotation = Quaternion.Slerp(
            panoramaSphere.transform.rotation,
            correctedRotation,
            Time.deltaTime * 10f * rotationSensitivity
        );
    }

    public void RecenterView()
    {
        _startingOrientation = panoramaSphere.transform.rotation;
    }

    public void ApplyDirection(float heading)
    {
        if (panoramaSphere == null) return;
        panoramaSphere.transform.rotation = Quaternion.Euler(0, -heading, 0);
        _startingOrientation = panoramaSphere.transform.rotation;
    }
}

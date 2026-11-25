using UnityEngine;

public static class DirectionManager
{
    public static float SavedHeading { get; private set; } = -1f;

    public static void SaveCurrentDirection()
    {
        Input.compass.enabled = true;
        SavedHeading = Input.compass.magneticHeading;
        Debug.Log($"Direction Saved: {SavedHeading}°");
    }

    public static Quaternion GetSavedDirectionRotation()
    {
        if (SavedHeading < 0) return Quaternion.identity;
        return Quaternion.Euler(0, -SavedHeading, 0);
    }

    public static void ResetDirection()
    {
        SavedHeading = -1f;
    }
}

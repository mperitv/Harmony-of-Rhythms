using UnityEngine;

public enum ThreatDirection
{
    LeftMelee = 0,
    UpperLeft = 1,
    UpperRight = 2,
    RightMelee = 3,
    GroundFront = 4
}

public static class ThreatUtility
{
    public static readonly Color[] Colors =
    {
        new Color(1f, 0.35f, 0.35f),
        new Color(0.35f, 0.6f, 1f),
        new Color(1f, 0.85f, 0.25f),
        new Color(0.35f, 0.95f, 0.5f),
        new Color(0.85f, 0.45f, 1f)
    };
}

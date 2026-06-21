using UnityEngine;

public enum LaneType
{
    D = 0,
    F = 1,
    J = 2,
    K = 3
}

public static class LaneUtility
{
    public static readonly Color[] Colors =
    {
        new Color(1f, 0.25f, 0.25f),
        new Color(0.25f, 0.55f, 1f),
        new Color(1f, 0.9f, 0.2f),
        new Color(0.3f, 0.95f, 0.45f)
    };
}

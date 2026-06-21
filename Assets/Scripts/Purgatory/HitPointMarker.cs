using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HitPointMarker : MonoBehaviour
{
    [SerializeField] private ThreatDirection direction;

    public ThreatDirection Direction => direction;

    public void Configure(ThreatDirection threatDirection)
    {
        direction = threatDirection;
        ApplyVisual();
    }

    private void Awake()
    {
        ApplyVisual();
    }

    private void ApplyVisual()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color color = ThreatUtility.Colors[(int)direction];
        color.a = 0.7f;
        spriteRenderer.color = color;
        spriteRenderer.sortingOrder = 3;
    }
}

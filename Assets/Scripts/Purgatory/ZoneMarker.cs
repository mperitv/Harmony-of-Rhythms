using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ZoneMarker : MonoBehaviour
{
    [SerializeField] private ThreatDirection direction;

    private void Awake()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color color = ThreatUtility.Colors[(int)direction];
        color.a = 0.2f;
        spriteRenderer.color = color;
    }
}

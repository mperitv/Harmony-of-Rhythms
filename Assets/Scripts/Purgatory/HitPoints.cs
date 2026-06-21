using UnityEngine;

public class HitPoints : MonoBehaviour
{
    [SerializeField] private HitPointMarker markerPrefab;
    [SerializeField] private Vector3 leftMeleeLocal = new Vector3(-1.6f, 0f, 0f);
    [SerializeField] private Vector3 rightMeleeLocal = new Vector3(1.6f, 0f, 0f);
    [SerializeField] private Vector3 upperLeftLocal = new Vector3(-0.9f, 1.2f, 0f);
    [SerializeField] private Vector3 upperRightLocal = new Vector3(0.9f, 1.2f, 0f);
    [SerializeField] private Vector3 groundFrontLocal = new Vector3(0f, 0f, 0f);
    [SerializeField] private float gizmoSize = 0.28f;

    private Vector3 anchorPosition;
    private HitPointMarker leftMelee;
    private HitPointMarker rightMelee;
    private HitPointMarker upperLeft;
    private HitPointMarker upperRight;
    private HitPointMarker groundFront;

    private void Awake()
    {
        anchorPosition = transform.position;
        leftMelee = CreateMarker(ThreatDirection.LeftMelee, leftMeleeLocal);
        rightMelee = CreateMarker(ThreatDirection.RightMelee, rightMeleeLocal);
        upperLeft = CreateMarker(ThreatDirection.UpperLeft, upperLeftLocal);
        upperRight = CreateMarker(ThreatDirection.UpperRight, upperRightLocal);
        groundFront = CreateMarker(ThreatDirection.GroundFront, groundFrontLocal);
    }

    private void LateUpdate()
    {
        transform.position = anchorPosition;
    }

    public Vector3 GetPosition(ThreatDirection direction)
    {
        HitPointMarker marker = GetMarker(direction);
        return marker != null ? marker.transform.position : transform.position;
    }

    private HitPointMarker GetMarker(ThreatDirection direction)
    {
        switch (direction)
        {
            case ThreatDirection.LeftMelee:
                return leftMelee;
            case ThreatDirection.RightMelee:
                return rightMelee;
            case ThreatDirection.UpperLeft:
                return upperLeft;
            case ThreatDirection.UpperRight:
                return upperRight;
            case ThreatDirection.GroundFront:
                return groundFront;
            default:
                return null;
        }
    }

    private HitPointMarker CreateMarker(ThreatDirection direction, Vector3 localPosition)
    {
        HitPointMarker marker = Instantiate(markerPrefab, transform);
        marker.transform.localPosition = localPosition;
        marker.Configure(direction);
        return marker;
    }

    private void OnDrawGizmosSelected()
    {
        DrawGizmo(ThreatDirection.LeftMelee, leftMeleeLocal);
        DrawGizmo(ThreatDirection.RightMelee, rightMeleeLocal);
        DrawGizmo(ThreatDirection.UpperLeft, upperLeftLocal);
        DrawGizmo(ThreatDirection.UpperRight, upperRightLocal);
        DrawGizmo(ThreatDirection.GroundFront, groundFrontLocal);
    }

    private void DrawGizmo(ThreatDirection direction, Vector3 localPosition)
    {
        Gizmos.color = ThreatUtility.Colors[(int)direction];
        Gizmos.DrawWireCube(transform.TransformPoint(localPosition), Vector3.one * gizmoSize);
    }
}

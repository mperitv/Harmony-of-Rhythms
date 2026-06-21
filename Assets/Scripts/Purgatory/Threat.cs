using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Threat : MonoBehaviour
{
    public ThreatDirection Direction { get; private set; }
    public float IdealHitTime { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool CanReceiveInput => IsActive && !hitRegistered;

    private Vector3 velocity;
    private Vector3 hitTarget;
    private RhythmGameManager gameManager;
    private SpriteRenderer spriteRenderer;
    private bool hitRegistered;

    public void Initialize(
        ThreatDirection direction,
        float idealHitTime,
        Vector3 velocity,
        Vector3 hitTarget,
        RhythmGameManager gameManager)
    {
        Direction = direction;
        IdealHitTime = idealHitTime;
        this.velocity = velocity;
        this.hitTarget = hitTarget;
        this.gameManager = gameManager;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = ThreatUtility.Colors[(int)direction];
    }

    private void Update()
    {
        if (!IsActive || gameManager == null)
        {
            return;
        }

        transform.position += velocity * Time.deltaTime;

        if (Direction == ThreatDirection.GroundFront)
        {
            UpdateGroundFront();
            return;
        }

        if (HasPassedHitPoint())
        {
            Resolve(false);
        }
    }

    private void UpdateGroundFront()
    {
        if (transform.position.y < hitTarget.y - 3f)
        {
            RemoveSilently();
        }
    }

    private bool HasPassedHitPoint()
    {
        Vector3 position = transform.position;
        float passThreshold = gameManager.PassThreshold;

        switch (Direction)
        {
            case ThreatDirection.LeftMelee:
                return position.x > hitTarget.x + passThreshold;
            case ThreatDirection.RightMelee:
                return position.x < hitTarget.x - passThreshold;
            case ThreatDirection.UpperLeft:
            case ThreatDirection.UpperRight:
                return position.y < hitTarget.y - passThreshold;
            default:
                return false;
        }
    }

    public float GetTimingErrorMs(float songTime)
    {
        if (Direction == ThreatDirection.GroundFront)
        {
            return GetGroundInputErrorMs();
        }

        return (songTime - IdealHitTime) * 1000f;
    }

    public float GetGroundInputErrorMs()
    {
        if (Mathf.Abs(velocity.y) < 0.001f)
        {
            return float.MaxValue;
        }

        float secondsToHitPoint = (hitTarget.y - transform.position.y) / velocity.y;
        return secondsToHitPoint * 1000f;
    }

    public void MarkHitKeepMoving()
    {
        hitRegistered = true;
    }

    public void ResolveFromInput(bool wasHandled)
    {
        Resolve(wasHandled);
    }

    private void Resolve(bool wasHandled)
    {
        Vector3 position = transform.position;
        IsActive = false;
        gameManager.RemoveThreat(this);

        if (!wasHandled)
        {
            gameManager.RegisterPassMiss(position);
        }

        Destroy(gameObject);
    }

    private void RemoveSilently()
    {
        IsActive = false;
        gameManager.RemoveThreat(this);
        Destroy(gameObject);
    }
}

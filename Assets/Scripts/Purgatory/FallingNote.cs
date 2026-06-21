using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FallingNote : MonoBehaviour
{
    public LaneType Lane { get; private set; }
    public bool IsActive { get; private set; } = true;

    private float moveSpeed;
    private RhythmGameManager gameManager;
    private SpriteRenderer spriteRenderer;

    public void Initialize(LaneType lane, float moveSpeed, RhythmGameManager gameManager)
    {
        Lane = lane;
        this.moveSpeed = moveSpeed;
        this.gameManager = gameManager;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = LaneUtility.Colors[(int)lane];
    }

    private void Update()
    {
        if (!IsActive || gameManager == null)
        {
            return;
        }

        transform.position += Vector3.left * (moveSpeed * Time.deltaTime);

        if (transform.position.x < gameManager.MissLineX)
        {
            Miss();
        }
    }

    public float DistanceToHitLine()
    {
        return Mathf.Abs(transform.position.x - gameManager.HitLineX);
    }

    public bool TryHit(float perfectWindow, float goodWindow, out bool perfect)
    {
        perfect = false;

        if (!IsActive)
        {
            return false;
        }

        float distance = DistanceToHitLine();

        if (distance <= perfectWindow)
        {
            perfect = true;
            Resolve(true);
            return true;
        }

        if (distance <= goodWindow)
        {
            Resolve(true);
            return true;
        }

        return false;
    }

    private void Miss()
    {
        if (!IsActive)
        {
            return;
        }

        Resolve(false);
    }

    private void Resolve(bool wasHit)
    {
        IsActive = false;
        gameManager.RegisterNoteResult(wasHit);
        Destroy(gameObject);
    }
}

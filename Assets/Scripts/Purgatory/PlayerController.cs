using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 0.75f;
    [SerializeField] private float jumpDuration = 0.35f;

    private float anchorX;
    private float baseY;
    private float jumpTimer;
    private bool isJumping;

    public bool IsJumping => isJumping;

    private void Awake()
    {
        anchorX = transform.position.x;
        baseY = transform.position.y;
    }

    private void LateUpdate()
    {
        Vector3 position = transform.position;
        position.x = anchorX;

        if (isJumping)
        {
            jumpTimer -= Time.deltaTime;
            float progress = 1f - Mathf.Clamp01(jumpTimer / jumpDuration);
            position.y = baseY + Mathf.Sin(progress * Mathf.PI) * jumpHeight;

            if (jumpTimer <= 0f)
            {
                isJumping = false;
                position.y = baseY;
            }
        }
        else
        {
            position.y = baseY;
        }

        transform.position = position;
    }

    public void StartJump()
    {
        if (isJumping)
        {
            return;
        }

        isJumping = true;
        jumpTimer = jumpDuration;
    }
}

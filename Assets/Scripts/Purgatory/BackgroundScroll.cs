using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.8f;

    private void Update()
    {
        transform.position += Vector3.left * (scrollSpeed * Time.deltaTime);
    }
}

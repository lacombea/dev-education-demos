using UnityEngine;

public class Platform : MonoBehaviour
{
    // Could later handle fragile, moving, trampolin types by subclassing/flags
    public bool isMoving = false;
    public float moveRange = 2f;
    public float moveSpeed = 1.5f;
    Vector3 startPos;

    void Awake() => startPos = transform.position;

    void Update()
    {
        if (isMoving)
        {
            Vector3 p = startPos;
            p.x += Mathf.Sin(Time.time * moveSpeed) * moveRange;
            transform.position = p;
        }
    }
}
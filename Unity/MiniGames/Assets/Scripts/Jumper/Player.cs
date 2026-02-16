using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementNew : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpVelocity = 12f;

    private Rigidbody2D rb;
    private InputAction moveAction;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        rb.linearVelocity = new Vector2(moveAction.ReadValue<float>()* moveSpeed, rb.linearVelocity.y);

        // wrap horizontal (optionnel)
        Vector3 pos = transform.position;
        float halfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        if (pos.x > halfWidth + 0.5f) pos.x = -halfWidth - 0.5f;
        else if (pos.x < -halfWidth - 0.5f) pos.x = halfWidth + 0.5f;
        transform.position = pos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform") && rb.linearVelocity.y <= 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
    }
}
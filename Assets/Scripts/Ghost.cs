using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ghost : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Vector2 startDirection = Vector2.right;
    [SerializeField] private LayerMask[] collisionLayers;
    [SerializeField] private float checkDistantce = 0.5f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 currentDirection;

    private static Vector2[] directions = new Vector2[]
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };

    void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        currentDirection = startDirection.normalized;
    }
    void FixedUpdate()
    {
        if (IsBlocked(currentDirection))
        {
            PickDirection();
        }

        rb.MovePosition(rb.position + currentDirection * moveSpeed * Time.fixedDeltaTime);
        if (currentDirection.x > 0f)
        {
            sr.flipX = true;
        }
        else if (currentDirection.x < 0f)
        {
            sr.flipX = false;
        }
    }

    private bool IsBlocked(Vector2 direction)
    {
        foreach (var layer in collisionLayers)
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.position, direction, checkDistantce, layer);
            if (hit.collider != null && hit.collider.gameObject != this.gameObject)
            {
                return true;
            }
        }
        return false;
    }

    private void PickDirection()
    {
        var validOptions = new System.Collections.Generic.List<Vector2>();

        foreach (var dir in directions)
        {
            if (!IsBlocked(dir))
            {
                validOptions.Add(dir);
            }
        }

        if (validOptions.Count > 0)
        {
            currentDirection = validOptions[Random.Range(0, validOptions.Count)];
        }
    }
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        foreach (var dir in directions)
        {
            Gizmos.DrawLine(rb.position, rb.position + dir * checkDistantce);
        }
    }
}

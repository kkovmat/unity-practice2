using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Vector2 startDirection = Vector2.right;
    [SerializeField] private float cellSize = 1f;      
    [SerializeField] private LayerMask wallLayer;      

    private Rigidbody2D rb;
    private Vector2 currentDirection;
    private Vector2 nextDirection;      
    private Vector2 previousPosition;   
    private Vector2 targetPosition;
         

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        currentDirection = startDirection.normalized;
        nextDirection = currentDirection;
        previousPosition = rb.position;  
        targetPosition = rb.position;
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        Vector2 input = Vector2.zero;

        if (keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame)
            input = Vector2.up;
        else if (keyboard.sKey.wasPressedThisFrame || keyboard.downArrowKey.wasPressedThisFrame)
            input = Vector2.down;
        else if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame)
            input = Vector2.left;
        else if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame)
            input = Vector2.right;

        if (input == Vector2.zero) return;

        nextDirection = input;

        
        if (input == -currentDirection && IsBetweenCells())
        {
            currentDirection = input;
            (previousPosition, targetPosition) = (targetPosition, previousPosition);
        }
    }

    void FixedUpdate()
    {
        
        if (!IsBetweenCells())
        {
            ChooseNextStep();
        }

        rb.MovePosition(Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime));

        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
        rb.MoveRotation(angle);
    }

    private bool IsBetweenCells()
    {
        return (rb.position - targetPosition).sqrMagnitude > 0.0001f;
    }

    private void ChooseNextStep()
    {
        Vector2 current = targetPosition;

        if (CanMove(current, nextDirection))
        {
            currentDirection = nextDirection;   
        }
        else if (!CanMove(current, currentDirection))
        {
            return;                             
        }

        previousPosition = current;
        targetPosition = current + currentDirection * cellSize;
    }

    private bool CanMove(Vector2 from, Vector2 direction)
    {
        Vector2 checkPos = from + direction * cellSize;
        return Physics2D.OverlapBox(checkPos, Vector2.one * cellSize * 0.8f, 0f, wallLayer) == null;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Ghost>(out Ghost ghost) && GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Vector2 startDirection = Vector2.right;
    [SerializeField] private bool canMoveBackwards = false;

    private Rigidbody2D rb;
    private Vector2 currentDirection;

    void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        currentDirection = startDirection.normalized;
    }

    void Update()
    {
        Vector2 newDirection = currentDirection;
        var keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) 
        {
            newDirection = Vector2.up;
        } 
        else if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) 
        {
            newDirection = Vector2.down;
        } 
        else if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) 
        {
            newDirection = Vector2.left;
        } 
        else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) 
        {
            newDirection = Vector2.right;
        }
        else
        {
            return;
        }
        
        if (!canMoveBackwards && newDirection == -currentDirection)
        {
            return;
        }

        currentDirection = newDirection;
    }

    void FixedUpdate() 
    {
        rb.MovePosition(rb.position + currentDirection * moveSpeed * Time.fixedDeltaTime);

        float targetAngle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
        rb.MoveRotation(targetAngle);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ghost"))
        {
            GameManager.Instance.GameOver();
        }
    }
}
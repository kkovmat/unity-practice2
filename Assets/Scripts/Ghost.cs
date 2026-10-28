using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ghost : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Vector2 startDirection = Vector2.right;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private LayerMask wallLayer;

    private static readonly List<Ghost> allGhosts = new List<Ghost>();

    private static readonly Vector2[] directions =
    {
        Vector2.up, Vector2.down, Vector2.left, Vector2.right
    };

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 currentDirection;
    private Vector2 targetPosition; // клітинка, в якій привид стоїть або до якої їде
    private bool firstStep = true;

    void OnEnable()  => allGhosts.Add(this);
    void OnDisable() => allGhosts.Remove(this);

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        currentDirection = startDirection.normalized;
        targetPosition = rb.position;
    }

    void FixedUpdate()
    {
        if (!IsBetweenCells())
        {
            ChooseNextStep();
        }

        rb.MovePosition(Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime));

        if (currentDirection.x > 0f) sr.flipX = true;
        else if (currentDirection.x < 0f) sr.flipX = false;
    }

    private bool IsBetweenCells()
    {
        return (rb.position - targetPosition).sqrMagnitude > 0.0001f;
    }

    private void ChooseNextStep()
    {
        Vector2 current = targetPosition;

        if (firstStep)
        {
            firstStep = false;
            if (currentDirection != Vector2.zero && CanMove(current, currentDirection))
            {
                targetPosition = current + currentDirection * cellSize;
                return;
            }
        }

        var options = new List<Vector2>();

        foreach (var dir in directions)
        {
            if (dir == -currentDirection) continue;
            if (CanMove(current, dir)) options.Add(dir);
        }

        // Глухий кут або дорогу перекрив інший привид: дозволяємо розворот
        if (options.Count == 0 && CanMove(current, -currentDirection))
        {
            options.Add(-currentDirection);
        }

        if (options.Count == 0) return; // нікуди йти: спробуємо ще раз на наступному кадрі

        currentDirection = options[Random.Range(0, options.Count)];
        targetPosition = current + currentDirection * cellSize;
    }

    private bool CanMove(Vector2 from, Vector2 direction)
    {
        Vector2 nextCell = from + direction * cellSize;

        if (Physics2D.OverlapBox(nextCell, Vector2.one * cellSize * 0.8f, 0f, wallLayer) != null)
            return false;

        return !IsCellTakenByOtherGhost(nextCell);
    }

    private bool IsCellTakenByOtherGhost(Vector2 cell)
    {
        float tolerance = cellSize * 0.5f;

        foreach (var other in allGhosts)
        {
            if (other == this) continue;

            // Клітинка, в якій інший привид стоїть або до якої вже їде
            if ((other.targetPosition - cell).sqrMagnitude < 0.01f)
                return true;

            // Інший привид ще фізично заходить у цю клітинку або виходить із неї
            if (Vector2.Distance(other.rb.position, cell) < tolerance)
                return true;
        }

        return false;
    }
}
using UnityEngine;

public class Pellet : MonoBehaviour
{
    [SerializeField] private int scoreValue = 10;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.TryGetComponent<Movement>(out Movement movement))
        {
            ScoreManager.Instance.AddScore(scoreValue);
            Destroy(gameObject);
        }
    }
}

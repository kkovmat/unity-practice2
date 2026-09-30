using UnityEngine;

public class Pellet : MonoBehaviour
{
    [SerializeField] private int scoreValue = 10;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) 
        {
            ScoreManager.Instance.AddScore(scoreValue);
            Destroy(gameObject);
        }
    }

    void Start()
    {

    }

    
    void Update()
    {
        
    }
}

using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    private bool isGameOver = false;

    void Awake()
    {
        Instance = this;
        gameOverUI.SetActive(false);
    }

    public void GameOver()
    {
       if (isGameOver) return;
        isGameOver = true;

        gameOverUI.SetActive(true);
        finalScoreText.text = "Score: " + ScoreManager.Instance.GetCurrentScore();
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        ScoreManager.Instance.ResetScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

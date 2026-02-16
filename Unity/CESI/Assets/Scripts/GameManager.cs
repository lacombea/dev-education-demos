using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int score = 0;
    public TMP_Text scoreText;
    
    public float timeLeft = 60f;
    public TMP_Text timerText;

    public GameObject gameOverPanel;
    public TMP_Text scoreFinalText;

    public void Awake()
    {
        Instance = this;
    }
    
    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "Score : " + score;
    }

    public void Update()
    {
        timeLeft -= Time.deltaTime;
        
        if (timeLeft <= 0)
        {
            EndGame();
        }

        timerText.text = "Temps : " + Mathf.Ceil(timeLeft);
    }

    private void EndGame()
    {
        timeLeft = 0;
        FindFirstObjectByType<PlayerController>().enabled = false;
        gameOverPanel.SetActive(true);
        scoreFinalText.text = "Score : " + score;
    }
    
    public void RestartGame()
    {
        // Recharge la scène actuelle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}


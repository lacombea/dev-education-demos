using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TMP_Text scoreText;
    public Transform player;

    float bestHeight = 0f;
    int score = 0;

    void Awake() => Instance = this;

    void Update()
    {
        float h = Mathf.Max(bestHeight, player.position.y);
        bestHeight = h;
        score = Mathf.FloorToInt(bestHeight * 10f); // example scaling
        if (scoreText) scoreText.text = "Score: " + score.ToString();
    }

    public void OnPlayerBounced(float bounceY)
    {
        if (bounceY > bestHeight) bestHeight = bounceY;
    }
}
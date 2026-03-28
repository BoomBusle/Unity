using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    public Text livesText;
    public Text coinsText;
    public Text timerText;
    public Text messageText;

    private bool _showingMessage;
    private float _messageTimer;

    void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameOver += ShowGameOver;
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameOver -= ShowGameOver;
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        GameManager gm = GameManager.Instance;

        if (livesText != null)
            livesText.text = $"Lives: {gm.lives}/{gm.maxLives}";

        if (coinsText != null)
            coinsText.text = $"Coins: {gm.coinsCollected}";

        if (timerText != null)
        {
            float remaining = Mathf.Max(0f, gm.timeLimit - gm.elapsedTime);
            int min = (int)(remaining / 60f);
            int sec = (int)(remaining % 60f);
            timerText.text = $"Time: {min:00}:{sec:00}";

            if (remaining < 15f && !gm.isGameOver)
                timerText.color = Color.red;
            else
                timerText.color = Color.white;
        }

        if (_showingMessage && messageText != null)
        {
            _messageTimer -= Time.deltaTime;
            if (_messageTimer <= 0f)
            {
                messageText.gameObject.SetActive(false);
                _showingMessage = false;
            }
        }
    }

    private void ShowGameOver()
    {
        if (messageText == null) return;

        GameManager gm = GameManager.Instance;
        string reason = gm.lives <= 0 ? "Lives = 0" : "Time is up!";
        messageText.text = $"GAME OVER\n{reason}\nCoins: {gm.coinsCollected}  |  Collisions: {gm.collisionCount}";
        messageText.color = Color.red;
        messageText.gameObject.SetActive(true);
        _showingMessage = true;
        _messageTimer = 10f;
    }

    public void ShowFinish()
    {
        if (messageText == null) return;

        GameManager gm = GameManager.Instance;
        messageText.text = $"FINISH!\nCoins: {gm.coinsCollected}  |  Time: {gm.elapsedTime:F1}s";
        messageText.color = Color.green;
        messageText.gameObject.SetActive(true);
        _showingMessage = true;
        _messageTimer = 10f;
    }
}

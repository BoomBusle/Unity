using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameOver += HandleGameOver;
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameOver -= HandleGameOver;
    }

    private void HandleGameOver()
    {
        Debug.Log("========================================");
        Debug.Log("  ГРА ЗАКІНЧЕНА - ВИ ПРОГРАЛИ!");
        Debug.Log($"  Зібрано монет: {GameManager.Instance.coinsCollected}");
        Debug.Log($"  Зіткнень: {GameManager.Instance.collisionCount}");
        Debug.Log($"  Час: {GameManager.Instance.elapsedTime:F1}с");
        Debug.Log("========================================");
    }
}

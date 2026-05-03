using UnityEngine;

public class Trap : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        PlayerRunner player = other.GetComponent<PlayerRunner>();
        if (player != null)
        {
            if (GameManager.Instance != null && !GameManager.Instance.isGameOver)
            {
                player.TakeDamage();
                GameManager.Instance.LoseLife();
                if (!GameManager.Instance.isGameOver)
                    player.Respawn();
            }
        }
    }
}

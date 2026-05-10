using UnityEngine;
using UnityEngine.UI;

namespace Lab7
{
    public class GameHUD : MonoBehaviour
    {
        public Health playerHealth;
        public Text hpText;
        public Text statusText;
        public GameObject gameOverPanel;

        void Start()
        {
            if (playerHealth == null)
            {
                var p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) playerHealth = p.GetComponent<Health>();
            }
            if (playerHealth != null)
            {
                playerHealth.onChanged.AddListener(OnHpChanged);
                playerHealth.onDeath.AddListener(OnDeath);
                OnHpChanged(playerHealth.CurrentHP, playerHealth.maxHP);
            }
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
        }

        void Update()
        {
            if (statusText != null)
            {
                int alive = 0;
                var enemies = GameObject.FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
                foreach (var e in enemies)
                    if (!e.IsDead) alive++;
                statusText.text = "Ворогів живих: " + alive;
            }
        }

        void OnHpChanged(int cur, int max)
        {
            if (hpText != null) hpText.text = "HP: " + cur + " / " + max;
        }

        void OnDeath()
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(true);
        }
    }
}

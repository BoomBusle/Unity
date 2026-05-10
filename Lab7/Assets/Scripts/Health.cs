using UnityEngine;
using UnityEngine.Events;

namespace Lab7
{
    public class Health : MonoBehaviour
    {
        public int maxHP = 100;
        public int CurrentHP { get; private set; }
        public bool IsDead => CurrentHP <= 0;

        public UnityEvent onDeath;
        public UnityEvent<int, int> onChanged;

        void Awake()
        {
            CurrentHP = maxHP;
        }

        public void TakeDamage(int dmg)
        {
            if (IsDead) return;
            CurrentHP = Mathf.Max(0, CurrentHP - dmg);
            onChanged?.Invoke(CurrentHP, maxHP);
            if (CurrentHP <= 0)
                onDeath?.Invoke();
        }
    }
}

using UnityEngine;

namespace Lab7
{
    [RequireComponent(typeof(Collider))]
    public class Zone : MonoBehaviour
    {
        public ZoneType type = ZoneType.Road;

        [Tooltip("Множник максимальної швидкості гравця у цій зоні")]
        public float playerSpeedMultiplier = 1f;

        [Tooltip("Прискорення гравця (як швидко набирає швидкість). Низьке = слизько")]
        public float acceleration = 30f;

        [Tooltip("Гальмування гравця (як швидко зупиняється). Низьке = ковзання")]
        public float deceleration = 30f;

        void Reset()
        {
            var col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
        }
    }
}

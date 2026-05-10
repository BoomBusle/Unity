using UnityEngine;

namespace Lab7
{
    [RequireComponent(typeof(Collider))]
    public class Projectile : MonoBehaviour
    {
        public float speed = 18f;
        public int damage = 35;
        public float lifeTime = 4f;
        public string ignoreTag = "Player";

        public HitEffect hitEffectPrefab;
        public ParticleSystem hitParticlesPrefab;

        Vector3 direction;
        float spawnTime;

        public void Launch(Vector3 dir)
        {
            direction = dir;
            direction.y = 0f;
            direction.Normalize();
            transform.rotation = Quaternion.LookRotation(direction);
            spawnTime = Time.time;
        }

        void Update()
        {
            transform.position += direction * speed * Time.deltaTime;
            if (Time.time - spawnTime > lifeTime)
                Destroy(gameObject);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!string.IsNullOrEmpty(ignoreTag) && other.CompareTag(ignoreTag)) return;
            if (other.GetComponent<Zone>() != null) return;
            if (other.GetComponent<Projectile>() != null) return;
            if (other.GetComponent<HitEffect>() != null) return;

            var hp = other.GetComponent<Health>();
            if (hp != null && !hp.IsDead)
            {
                hp.TakeDamage(damage);
                SpawnHitEffect();
                Destroy(gameObject);
            }
        }

        void SpawnHitEffect()
        {
            if (hitEffectPrefab != null)
            {
                var fx = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                fx.gameObject.SetActive(true);
            }
            if (hitParticlesPrefab != null)
            {
                var p = Instantiate(hitParticlesPrefab, transform.position, Quaternion.identity);
                p.gameObject.SetActive(true);
            }
        }
    }
}

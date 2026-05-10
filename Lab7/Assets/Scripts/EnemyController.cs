using UnityEngine;

namespace Lab7
{
    public enum AttackMode
    {
        Melee,
        Ranged
    }

    [RequireComponent(typeof(ZoneTracker))]
    [RequireComponent(typeof(Health))]
    public class EnemyController : MonoBehaviour
    {
        [Header("Поведінка")]
        [Tooltip("Зона, у якій ворог 'мешкає'. Він також може бігати по дорозі та льоду.")]
        public ZoneType habitat = ZoneType.Field;

        [Tooltip("Радіус, у якому ворог помічає гравця і починає атаку")]
        public float aggroRange = 14f;
        public float deAggroRange = 22f;

        public float speed = 2.5f;
        public float rotationSpeed = 8f;

        [Header("Атака")]
        public AttackMode attackMode = AttackMode.Melee;
        public float meleeAttackRange = 1.6f;
        public float attackCooldown = 1f;
        public int attackDamage = 10;

        [Header("Дистанційна атака")]
        public float preferredRangedDistance = 7f;
        public Projectile rangedProjectilePrefab;
        public Transform rangedMuzzle;
        public HitEffect muzzleFlashPrefab;
        public ParticleSystem muzzleParticlesPrefab;

        [Header("Ефект смерті")]
        public GameObject deathEffectPrefab;

        [Header("Перевірка зон")]
        public float zoneCheckRadius = 0.4f;
        public float zoneProbeDistance = 0.35f;

        Transform target;
        ZoneTracker zoneTracker;
        Health health;
        float lastAttack;
        bool aggressive;

        public bool IsDead => health != null && health.IsDead;
        public bool IsAggressive => aggressive;

        void Awake()
        {
            zoneTracker = GetComponent<ZoneTracker>();
            health = GetComponent<Health>();
            health.onDeath.AddListener(HandleDeath);
        }

        void Start()
        {
            FindTarget();
        }

        void Update()
        {
            if (health.IsDead) return;
            if (target == null)
            {
                FindTarget();
                if (target == null) return;
            }

            Vector3 toPlayer = target.position - transform.position;
            toPlayer.y = 0f;
            float dist = toPlayer.magnitude;
            if (dist < 0.001f) return;

            if (!aggressive && dist <= aggroRange) aggressive = true;
            else if (aggressive && dist > deAggroRange) aggressive = false;

            if (!aggressive) return;

            Vector3 dir = toPlayer / dist;
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);

            if (attackMode == AttackMode.Melee)
            {
                if (dist <= meleeAttackRange) { TryMeleeAttack(); return; }
                StepToward(dir);
            }
            else
            {
                float diff = dist - preferredRangedDistance;
                if (diff > 0.6f) StepToward(dir);
                else if (diff < -1.5f) StepToward(-dir);
                else TryRangedShoot(dir);
            }
        }

        void StepToward(Vector3 dir)
        {
            Vector3 step = dir * speed * Time.deltaTime;
            Vector3 nextPos = transform.position + step;
            if (CanStepInto(nextPos))
                transform.position = nextPos;
        }

        void TryMeleeAttack()
        {
            if (Time.time - lastAttack < attackCooldown) return;
            lastAttack = Time.time;
            if (target == null) return;
            var hp = target.GetComponent<Health>();
            if (hp != null) hp.TakeDamage(attackDamage);
        }

        void TryRangedShoot(Vector3 dir)
        {
            if (Time.time - lastAttack < attackCooldown) return;
            if (rangedProjectilePrefab == null) return;
            lastAttack = Time.time;

            Vector3 origin = rangedMuzzle != null
                ? rangedMuzzle.position
                : transform.position + Vector3.up * 0.8f + transform.forward * 0.7f;

            var p = Instantiate(rangedProjectilePrefab, origin, Quaternion.LookRotation(dir));
            p.gameObject.SetActive(true);
            p.Launch(dir);

            if (muzzleFlashPrefab != null)
            {
                var mf = Instantiate(muzzleFlashPrefab, origin, Quaternion.LookRotation(dir));
                mf.gameObject.SetActive(true);
            }
            if (muzzleParticlesPrefab != null)
            {
                var mp = Instantiate(muzzleParticlesPrefab, origin, Quaternion.LookRotation(dir));
                mp.gameObject.SetActive(true);
            }
        }

        bool CanStepInto(Vector3 pos)
        {
            Vector3 probe = pos + Vector3.up * zoneProbeDistance;
            Collider[] hits = Physics.OverlapSphere(probe, zoneCheckRadius, ~0, QueryTriggerInteraction.Collide);
            bool hasAllowed = false;
            for (int i = 0; i < hits.Length; i++)
            {
                var z = hits[i].GetComponent<Zone>();
                if (z == null) continue;
                if (z.type == ZoneType.Road || z.type == ZoneType.Ice || z.type == habitat)
                    hasAllowed = true;
                else
                    return false;
            }
            return hasAllowed;
        }

        void FindTarget()
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) target = go.transform;
        }

        void HandleDeath()
        {
            if (deathEffectPrefab != null)
            {
                var fx = Instantiate(deathEffectPrefab, transform.position + Vector3.up * 0.6f, Quaternion.identity);
                fx.SetActive(true);
            }
            Destroy(gameObject, 0.05f);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.5f, 0.1f, 0.4f);
            Gizmos.DrawWireSphere(transform.position, aggroRange);
            Gizmos.color = new Color(0.3f, 0.7f, 1f, 0.4f);
            Gizmos.DrawWireSphere(transform.position, attackMode == AttackMode.Ranged ? preferredRangedDistance : meleeAttackRange);
        }
    }
}

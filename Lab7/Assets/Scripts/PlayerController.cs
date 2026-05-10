using UnityEngine;

namespace Lab7
{
    [RequireComponent(typeof(ZoneTracker))]
    public class PlayerController : MonoBehaviour
    {
        public float baseSpeed = 5f;
        public float rotationSpeed = 12f;
        public float stopDistance = 0.2f;
        public float defaultAcceleration = 30f;
        public float defaultDeceleration = 30f;

        public Transform muzzle;
        public Projectile projectilePrefab;
        public HitEffect muzzleFlashPrefab;
        public ParticleSystem muzzleParticlesPrefab;
        public float fireCooldown = 0.4f;

        public LayerMask clickMask = ~0;

        Camera cam;
        ZoneTracker zoneTracker;

        Vector3 moveTarget;
        bool hasMoveTarget;
        Vector3 currentVelocity;
        float lastFireTime;

        public Vector3 CurrentVelocity => currentVelocity;

        void Awake()
        {
            zoneTracker = GetComponent<ZoneTracker>();
        }

        void Start()
        {
            cam = Camera.main;
            moveTarget = transform.position;
        }

        void Update()
        {
            if (cam == null) cam = Camera.main;
            HandleInput();
            Move();
        }

        void HandleInput()
        {
            if (cam == null) return;
            if (!Input.GetMouseButtonDown(0)) return;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 250f, clickMask, QueryTriggerInteraction.Collide);

            EnemyController enemy = null;
            float bestEnemyDist = float.MaxValue;
            Vector3 groundPoint = Vector3.zero;
            float bestGroundDist = float.MaxValue;
            bool foundGround = false;

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit h = hits[i];
                if (h.collider.transform == transform || h.collider.transform.IsChildOf(transform)) continue;

                EnemyController e = h.collider.GetComponentInParent<EnemyController>();
                if (e != null && !e.IsDead)
                {
                    if (h.distance < bestEnemyDist)
                    {
                        enemy = e;
                        bestEnemyDist = h.distance;
                    }
                    continue;
                }

                if (h.collider.GetComponent<Zone>() != null || h.collider.CompareTag("Ground"))
                {
                    if (h.distance < bestGroundDist)
                    {
                        foundGround = true;
                        groundPoint = h.point;
                        bestGroundDist = h.distance;
                    }
                }
            }

            if (enemy != null)
            {
                TryShoot(enemy.transform.position + Vector3.up * 0.5f);
                hasMoveTarget = false;
                return;
            }

            if (foundGround)
            {
                Vector3 p = groundPoint;
                p.y = transform.position.y;
                moveTarget = p;
                hasMoveTarget = true;
            }
        }

        void Move()
        {
            Zone zone = zoneTracker.Current;
            float mult = zone != null ? zone.playerSpeedMultiplier : 1f;
            float accel = zone != null ? zone.acceleration : defaultAcceleration;
            float decel = zone != null ? zone.deceleration : defaultDeceleration;
            float maxSpeed = baseSpeed * mult;

            Vector3 desiredVel;
            if (hasMoveTarget)
            {
                Vector3 toTarget = moveTarget - transform.position;
                toTarget.y = 0f;
                float dist = toTarget.magnitude;
                if (dist <= stopDistance)
                {
                    desiredVel = Vector3.zero;
                    hasMoveTarget = false;
                }
                else
                {
                    desiredVel = (toTarget / dist) * maxSpeed;
                }
            }
            else
            {
                desiredVel = Vector3.zero;
            }

            float rate = (desiredVel.sqrMagnitude > currentVelocity.sqrMagnitude) ? accel : decel;
            currentVelocity = Vector3.MoveTowards(currentVelocity, desiredVel, rate * Time.deltaTime);

            transform.position += currentVelocity * Time.deltaTime;

            if (currentVelocity.sqrMagnitude > 0.04f)
            {
                Quaternion target = Quaternion.LookRotation(currentVelocity.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
            }
        }

        void TryShoot(Vector3 worldTarget)
        {
            if (projectilePrefab == null) return;
            if (Time.time - lastFireTime < fireCooldown) return;
            lastFireTime = Time.time;

            Vector3 origin = muzzle != null ? muzzle.position : transform.position + Vector3.up * 1f + transform.forward * 0.6f;

            Vector3 dir = worldTarget - origin;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) return;
            dir.Normalize();

            transform.rotation = Quaternion.LookRotation(dir);

            var p = Instantiate(projectilePrefab, origin, Quaternion.LookRotation(dir));
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
    }
}

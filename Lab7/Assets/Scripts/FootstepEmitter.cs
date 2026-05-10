using UnityEngine;

namespace Lab7
{
    [RequireComponent(typeof(ZoneTracker))]
    public class FootstepEmitter : MonoBehaviour
    {
        public ParticleSystem roadFx;
        public ParticleSystem fieldFx;
        public ParticleSystem swampFx;
        public ParticleSystem iceFx;

        public float stepInterval = 0.30f;
        public int burstCount = 6;
        public float minFrameMove = 0.02f;
        public Vector3 footOffset = new Vector3(0f, -0.9f, 0f);

        ZoneTracker zone;
        float nextStep;
        Vector3 lastPos;

        void Awake()
        {
            zone = GetComponent<ZoneTracker>();
            lastPos = transform.position;
        }

        void Update()
        {
            Vector3 delta = transform.position - lastPos;
            delta.y = 0f;
            float moved = delta.magnitude;
            lastPos = transform.position;

            if (moved < minFrameMove) return;
            if (Time.time < nextStep) return;
            nextStep = Time.time + stepInterval;

            EmitForCurrentZone();
        }

        void EmitForCurrentZone()
        {
            ParticleSystem ps = SelectSystem();
            if (ps == null) return;
            var p = new ParticleSystem.EmitParams { position = transform.position + footOffset };
            ps.Emit(p, burstCount);
        }

        ParticleSystem SelectSystem()
        {
            Zone z = zone != null ? zone.Current : null;
            if (z == null) return roadFx;
            switch (z.type)
            {
                case ZoneType.Road:  return roadFx;
                case ZoneType.Field: return fieldFx;
                case ZoneType.Swamp: return swampFx;
                case ZoneType.Ice:   return iceFx;
            }
            return roadFx;
        }
    }
}

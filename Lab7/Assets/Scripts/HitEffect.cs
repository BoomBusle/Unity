using UnityEngine;

namespace Lab7
{
    public class HitEffect : MonoBehaviour
    {
        public float duration = 0.35f;
        public float startScale = 0.5f;
        public float endScale = 1.4f;
        public float lightStartIntensity = 6f;

        float t;
        Light pointLight;
        Renderer rend;
        Color baseEmission;
        bool hasEmission;

        void Awake()
        {
            pointLight = GetComponentInChildren<Light>();
            rend = GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                Material m = rend.material;
                if (m != null && m.HasProperty("_EmissionColor"))
                {
                    baseEmission = m.GetColor("_EmissionColor");
                    hasEmission = true;
                }
            }
            if (pointLight != null) pointLight.intensity = lightStartIntensity;
        }

        void Update()
        {
            t += Time.deltaTime;
            float p = duration > 0f ? t / duration : 1f;
            if (p >= 1f) { Destroy(gameObject); return; }
            float scale = Mathf.Lerp(startScale, endScale, p);
            transform.localScale = new Vector3(scale, scale, scale);
            float inv = 1f - p;
            if (pointLight != null) pointLight.intensity = lightStartIntensity * inv;
            if (hasEmission && rend != null)
            {
                rend.material.SetColor("_EmissionColor", baseEmission * inv);
            }
        }
    }
}

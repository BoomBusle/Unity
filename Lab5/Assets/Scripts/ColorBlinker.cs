using UnityEngine;

public class ColorBlinker : MonoBehaviour
{
    [Tooltip("Multiplier applied to original colors. 1,1,1,1 = unchanged.")]
    public Color tint = Color.white;

    public Renderer[] targets;

    private Color[] _originals;

    void Awake()
    {
        if (targets == null || targets.Length == 0)
        {
            Transform searchRoot = transform.parent != null ? transform.parent : transform;
            targets = searchRoot.GetComponentsInChildren<Renderer>();
        }

        _originals = new Color[targets.Length];
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] == null) continue;
            _originals[i] = targets[i].material.color;
        }
    }

    void LateUpdate()
    {
        if (targets == null || _originals == null) return;

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] == null) continue;
            Color c = _originals[i] * tint;
            targets[i].material.color = c;
        }
    }
}

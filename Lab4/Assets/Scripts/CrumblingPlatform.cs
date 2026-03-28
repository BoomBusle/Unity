using UnityEngine;

public class CrumblingPlatform : MonoBehaviour
{
    public float crumbleDelay = 0.5f;
    public float respawnDelay = 5f;
    public Transform triggerZone;

    private Vector3 _originalPos;
    private bool _crumbling;
    private Renderer _renderer;
    private Collider _collider;
    private Color _defaultColor;

    void Start()
    {
        _originalPos = transform.position;
        _renderer = GetComponent<Renderer>();
        _collider = GetComponent<Collider>();
        if (_renderer != null)
            _defaultColor = _renderer.material.color;
    }

    void Update()
    {
        if (_crumbling) return;
        if (triggerZone == null) return;

        Collider[] hits = Physics.OverlapBox(
            triggerZone.position,
            triggerZone.localScale * 0.5f,
            Quaternion.identity
        );

        foreach (var hit in hits)
        {
            if (hit.GetComponent<CharacterController>() != null)
            {
                _crumbling = true;
                if (_renderer != null)
                    _renderer.material.color = Color.red;
                Invoke(nameof(Fall), crumbleDelay);
                break;
            }
        }
    }

    private void Fall()
    {
        if (_collider != null) _collider.enabled = false;
        if (_renderer != null) _renderer.enabled = false;
        Invoke(nameof(RespawnPlatform), respawnDelay);
    }

    private void RespawnPlatform()
    {
        transform.position = _originalPos;
        if (_collider != null) _collider.enabled = true;
        if (_renderer != null)
        {
            _renderer.enabled = true;
            _renderer.material.color = _defaultColor;
        }
        _crumbling = false;
    }
}

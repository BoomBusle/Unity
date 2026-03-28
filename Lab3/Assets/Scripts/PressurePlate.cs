using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public SpikeTrap linkedSpikes;
    public float activateDelay = 0.3f;

    private Renderer _renderer;
    private Color _defaultColor;
    private bool _activated;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
            _defaultColor = _renderer.material.color;
    }

    void Update()
    {
        if (_activated) return;

        Collider[] hits = Physics.OverlapBox(
            transform.position,
            transform.localScale * 0.5f,
            transform.rotation
        );

        foreach (var hit in hits)
        {
            if (hit.GetComponent<CharacterController>() != null)
            {
                _activated = true;

                if (_renderer != null)
                    _renderer.material.color = Color.red;
                transform.position += Vector3.down * 0.05f;

                if (linkedSpikes != null)
                    linkedSpikes.ActivateAfterDelay(activateDelay);

                break;
            }
        }
    }
}

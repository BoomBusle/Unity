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

    void OnTriggerEnter(Collider other)
    {
        TryActivate(other);
    }

    void OnTriggerStay(Collider other)
    {
        TryActivate(other);
    }

    private void TryActivate(Collider other)
    {
        if (_activated) return;
        if (other.GetComponent<CharacterController>() == null) return;

        _activated = true;

        if (_renderer != null)
            _renderer.material.color = Color.red;
        transform.position += Vector3.down * 0.05f;

        if (linkedSpikes != null)
            linkedSpikes.ActivateAfterDelay(activateDelay);
    }
}

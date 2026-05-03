using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Coin : MonoBehaviour
{
    public float pickupAnimDuration = 0.45f;

    private Animator _animator;
    private bool _picked;

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (_picked) return;
        if (other.GetComponent<CharacterController>() == null) return;

        _picked = true;

        if (GameManager.Instance != null)
            GameManager.Instance.CollectCoin();

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (_animator != null)
        {
            _animator.SetTrigger("Pickup");
            Destroy(gameObject, pickupAnimDuration);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

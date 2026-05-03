using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public float raiseHeight = 1.5f;
    public float raiseSpeed = 8f;
    public float stayUpDuration = 2f;

    private Vector3 _hiddenPos;
    private Vector3 _raisedPos;
    private bool _raising;
    private bool _lowering;
    private float _stayTimer;
    private bool _isUp;
    private bool _hitPlayer;

    void Start()
    {
        _hiddenPos = transform.position;
        _raisedPos = _hiddenPos + Vector3.up * raiseHeight;
    }

    void Update()
    {
        if (_raising)
        {
            transform.position = Vector3.MoveTowards(transform.position, _raisedPos, raiseSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _raisedPos) < 0.01f)
            {
                _raising = false;
                _stayTimer = stayUpDuration;
                _isUp = true;
                _hitPlayer = false;
            }
        }
        else if (_isUp && _stayTimer > 0f)
        {
            _stayTimer -= Time.deltaTime;
            if (_stayTimer <= 0f)
            {
                _isUp = false;
                _lowering = true;
            }
        }
        else if (_lowering)
        {
            transform.position = Vector3.MoveTowards(transform.position, _hiddenPos, raiseSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _hiddenPos) < 0.01f)
                _lowering = false;
        }
    }

    void OnTriggerEnter(Collider other) { TryDamage(other); }
    void OnTriggerStay(Collider other) { TryDamage(other); }

    private void TryDamage(Collider other)
    {
        if (_hitPlayer) return;
        if (!_raising && !_isUp) return;
        if (other.GetComponent<CharacterController>() == null) return;

        PlayerRunner player = other.GetComponent<PlayerRunner>();
        if (player == null) return;
        if (GameManager.Instance == null || GameManager.Instance.isGameOver) return;

        _hitPlayer = true;
        player.TakeDamage();
        GameManager.Instance.LoseLife();
        if (!GameManager.Instance.isGameOver)
            player.Respawn();
    }

    public void ActivateAfterDelay(float delay)
    {
        Invoke(nameof(Raise), delay);
    }

    private void Raise()
    {
        _raising = true;
        _hitPlayer = false;
    }
}

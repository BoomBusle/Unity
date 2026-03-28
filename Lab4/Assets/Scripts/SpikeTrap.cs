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

            if (!_hitPlayer)
                CheckPlayerOverlap();

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

        // Also check during raising phase
        if (_raising && !_hitPlayer)
            CheckPlayerOverlap();
    }

    private void CheckPlayerOverlap()
    {
        Collider[] hits = Physics.OverlapBox(
            transform.position,
            transform.localScale * 0.5f,
            transform.rotation
        );

        foreach (var hit in hits)
        {
            PlayerRunner player = hit.GetComponent<PlayerRunner>();
            if (player != null && GameManager.Instance != null && !GameManager.Instance.isGameOver)
            {
                _hitPlayer = true;
                GameManager.Instance.LoseLife();
                if (!GameManager.Instance.isGameOver)
                    player.Respawn();
                break;
            }
        }
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

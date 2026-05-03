using UnityEngine;

public class PlayerRunner : MonoBehaviour
{
    public float forwardSpeed = 5f;
    public float horizontalSpeed = 8f;
    public float jumpForce = 10f;
    public float boostMultiplier = 2f;
    public float boostMaxDuration = 3f;
    public float gravity = -25f;
    public float horizontalLimit = 4f;

    [Header("Animation")]
    public Animator animator;
    public Animator flashAnimator;

    private float _verticalVelocity;
    private bool _isGrounded;
    private bool _isBoosting;
    private float _boostTimer;
    private Vector3 _startPosition;
    private CharacterController _controller;
    private bool _finished;
    private bool _dead;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _startPosition = transform.position;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver += HandleGameOver;
            GameManager.Instance.OnLevelFinished += HandleLevelFinished;
        }
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver -= HandleGameOver;
            GameManager.Instance.OnLevelFinished -= HandleLevelFinished;
        }
    }

    void Update()
    {
        if (_dead || _finished)
        {
            UpdateAnimatorIdle();
            return;
        }

        if (GameManager.Instance != null && (!GameManager.Instance.isPlaying || GameManager.Instance.isGameOver))
        {
            UpdateAnimatorIdle();
            return;
        }

        float speed = forwardSpeed;
        float h = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _verticalVelocity = jumpForce;
            _isGrounded = false;
            if (animator != null) animator.SetTrigger("JumpTrigger");
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !_isBoosting && _boostTimer < boostMaxDuration)
            _isBoosting = true;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            _isBoosting = false;

        if (_isBoosting)
        {
            _boostTimer += Time.deltaTime;
            if (_boostTimer >= boostMaxDuration)
            {
                _isBoosting = false;
                _boostTimer = boostMaxDuration;
            }
            speed *= boostMultiplier;
        }
        else
        {
            _boostTimer = Mathf.Max(0f, _boostTimer - Time.deltaTime * 0.5f);
        }

        _verticalVelocity += gravity * Time.deltaTime;

        Vector3 move = new Vector3(h * horizontalSpeed, _verticalVelocity, speed) * Time.deltaTime;
        _controller.Move(move);

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -horizontalLimit, horizontalLimit);
        transform.position = pos;

        if (_controller.isGrounded)
        {
            _isGrounded = true;
            _verticalVelocity = -2f;
        }

        UpdateAnimatorRunning(speed);

        if (transform.position.y < -5f)
        {
            TakeDamage();
            if (GameManager.Instance != null)
                GameManager.Instance.LoseLife();

            if (GameManager.Instance == null || !GameManager.Instance.isGameOver)
                Respawn();
        }
    }

    private void UpdateAnimatorRunning(float currentSpeed)
    {
        if (animator == null) return;
        animator.SetFloat("Speed", currentSpeed);
        animator.SetBool("IsBoost", _isBoosting);
        animator.SetBool("Grounded", _isGrounded);
    }

    private void UpdateAnimatorIdle()
    {
        if (animator == null) return;
        animator.SetFloat("Speed", 0f);
        animator.SetBool("IsBoost", false);
        animator.SetBool("Grounded", true);
    }

    public void TakeDamage()
    {
        Animator a = flashAnimator != null ? flashAnimator : animator;
        if (a != null) a.SetTrigger("Flash");
    }

    public void Respawn()
    {
        _controller.enabled = false;
        transform.position = _startPosition;
        _controller.enabled = true;
        _verticalVelocity = 0f;
        _isGrounded = true;
        _isBoosting = false;
        _boostTimer = 0f;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (_dead || _finished) return;

        if (hit.gameObject.CompareTag("Obstacle"))
        {
            TakeDamage();
            if (GameManager.Instance != null)
                GameManager.Instance.LoseLife();

            if (GameManager.Instance == null || !GameManager.Instance.isGameOver)
                Respawn();
        }

        if (hit.gameObject.CompareTag("Finish"))
        {
            _finished = true;
            if (GameManager.Instance != null)
                GameManager.Instance.FinishLevel();
        }
    }

    private void HandleGameOver()
    {
        _dead = true;
        if (animator != null) animator.SetTrigger("Die");
    }

    private void HandleLevelFinished()
    {
        _finished = true;
        if (animator != null) animator.SetTrigger("Win");
    }
}

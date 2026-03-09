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

    private float _verticalVelocity;
    private bool _isGrounded;
    private bool _isBoosting;
    private float _boostTimer;
    private Vector3 _startPosition;
    private CharacterController _controller;
    private bool _finished;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _startPosition = transform.position;
    }

    void Update()
    {
        if (_finished) return;

        float speed = forwardSpeed;

        float h = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _verticalVelocity = jumpForce;
            _isGrounded = false;
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

        if (transform.position.y < -5f)
            Respawn();
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
        if (hit.gameObject.CompareTag("Obstacle"))
            Respawn();

        if (hit.gameObject.CompareTag("Finish"))
        {
            _finished = true;
            Debug.Log("ФІНІШ!");
        }
    }
}

using UnityEngine;

public class PendulumObstacle : MonoBehaviour
{
    public float maxAngle = 45f;
    public float swingSpeed = 2f;
    public float armLength = 3f;

    private Vector3 _pivotPoint;
    private Vector3 _previousPosition;

    void Start()
    {
        _pivotPoint = transform.position + Vector3.up * armLength;
        _previousPosition = transform.position;
    }

    void Update()
    {
        float angle = maxAngle * Mathf.Sin(Time.time * swingSpeed);
        float rad = angle * Mathf.Deg2Rad;

        Vector3 newPosition = _pivotPoint + new Vector3(Mathf.Sin(rad), -Mathf.Cos(rad), 0f) * armLength;
        Vector3 delta = newPosition - _previousPosition;

        transform.Translate(delta, Space.World);
        _previousPosition = newPosition;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}

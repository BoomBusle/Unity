using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 pointA = new Vector3(-5f, 0.5f, 0f);
    public Vector3 pointB = new Vector3(5f, 0.5f, 0f);
    public float speed = 2f;

    private Vector3 _direction;
    private Vector3 _target;

    void Start()
    {
        transform.position = pointA;
        _target = pointB;
        _direction = (pointB - pointA).normalized;
    }

    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.Translate(_direction * step, Space.World);

        if (Vector3.Distance(transform.position, _target) < 0.1f)
        {
            if (_target == pointB)
            {
                _target = pointA;
                _direction = (pointA - pointB).normalized;
            }
            else
            {
                _target = pointB;
                _direction = (pointB - pointA).normalized;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(pointA, pointB);
        Gizmos.DrawWireSphere(pointA, 0.3f);
        Gizmos.DrawWireSphere(pointB, 0.3f);
    }
}

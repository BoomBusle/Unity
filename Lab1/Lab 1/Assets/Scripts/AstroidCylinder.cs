using UnityEngine;

public class AstroidCylinder : MonoBehaviour
{
    public float radius = 4f;
    public float speed = 1f;
    public Vector3 centerOffset = Vector3.zero;

    private float _t;
    private Vector3 _previousPosition;

    void Start()
    {
        _t = 0f;
        Vector3 startPos = GetAstroidPosition(_t);
        transform.position = startPos + centerOffset;
        _previousPosition = transform.position;
    }

    void Update()
    {
        _t += speed * Time.deltaTime;

        Vector3 newPosition = GetAstroidPosition(_t) + centerOffset;
        Vector3 delta = newPosition - _previousPosition;

        transform.Translate(delta, Space.World);
        _previousPosition = newPosition;
    }

    Vector3 GetAstroidPosition(float t)
    {
        float cosT = Mathf.Cos(t);
        float sinT = Mathf.Sin(t);
        float x = radius * cosT * cosT * cosT;
        float z = radius * sinT * sinT * sinT;
        return new Vector3(x, 0f, z);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 offset = Application.isPlaying ? centerOffset : transform.position;
        int segments = 64;
        Vector3 prev = GetAstroidPosition(0f) + offset;
        for (int i = 1; i <= segments; i++)
        {
            float t = (float)i / segments * Mathf.PI * 2f;
            Vector3 next = GetAstroidPosition(t) + offset;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}

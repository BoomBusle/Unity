using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Vector3 targetPoint = new Vector3(0f, 2f, 15f);
    public float radius = 25f;
    public float speed = 10f;
    public float height = 12f;

    private float _angle;

    void Update()
    {
        _angle += speed * Time.deltaTime;

        float rad = _angle * Mathf.Deg2Rad;
        float x = targetPoint.x + radius * Mathf.Cos(rad);
        float z = targetPoint.z + radius * Mathf.Sin(rad);

        transform.position = new Vector3(x, height, z);
        transform.LookAt(targetPoint);
    }
}

using UnityEngine;

public class LaserComponent : MonoBehaviour
{
    [SerializeField]
    protected LineRenderer _lineRenderer;

    public void Initialize(Transform firePoint, Transform target)
    {
        _lineRenderer.SetPositions(new Vector3[]
        {
            firePoint.position,
            target.position
        });
        Destroy(gameObject, 0.1f);
    }

}

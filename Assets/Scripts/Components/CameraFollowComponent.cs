using UnityEngine;

public class CameraFollowComponent : MonoBehaviour
{
    [SerializeField]
    protected float _speed = 7f;
    [SerializeField]
    protected Transform _target;

    private void FixedUpdate()
    {
        if(!_target) { return; }
        transform.position = Vector3.Lerp(transform.position, _target.transform.position, _speed * Time.deltaTime);
    }

}

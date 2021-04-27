using UnityEngine;

public class GemstoneComponent : MonoBehaviour
{
    [SerializeField]
    protected int _money;
    protected Transform _target;

    private void Update()
    {
        if(!_target) { return; }
        transform.position = Vector3.Lerp(transform.position, _target.position, 5f * Time.deltaTime);
        if(Vector3.Distance(transform.position, _target.position) < 0.1f)
        {
            FindObjectOfType<PlayerControllerComponent>().AddMoney(_money);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponentInParent<PlayerControllerComponent>();
        if(!player) { return; }
        _target = player.transform;
    }
}

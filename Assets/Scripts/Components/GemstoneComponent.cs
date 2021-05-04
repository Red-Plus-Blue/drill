using UnityEngine;

public class GemstoneComponent : MonoBehaviour
{
    [SerializeField]
    protected int _money;
    protected Transform _target;

    private void Update()
    {
        if(!_target) { return; }
        var distance = _target.position - transform.position;
        transform.position += distance.normalized * 3f * Time.deltaTime;
        transform.localScale = Vector3.one * (Mathf.Min(0.5f, distance.magnitude) / 0.5f);
        
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

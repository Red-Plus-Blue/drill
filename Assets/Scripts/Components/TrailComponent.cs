using UnityEngine;

public class TrailComponent : MonoBehaviour
{
    [SerializeField]
    protected SpriteRenderer _renderer;

    protected float _fadeStartTime;
    protected float _fadeEndTime;

    private void Awake()
    {
        _fadeStartTime = Time.time + 1.0f;
        _fadeEndTime = _fadeStartTime + 1.0f;
    }

    private void Update()
    {
        if(Time.time < _fadeStartTime)
        {
            return;
        }

        _renderer.color = new Color(
            _renderer.color.r,
            _renderer.color.g,
            _renderer.color.b,
            0.7f * (_fadeEndTime - Time.time)
        );

        if(Time.time >= _fadeEndTime)
        {
            Destroy(gameObject);
        }
    }

}

using UnityEngine;

using UniRx;

public class TrailCreatorComponent : MonoBehaviour
{
    [SerializeField]
    protected GameObject _trail;
    [SerializeField]
    protected float _trailDelay;

    protected float _nextTrailTime;

    private void Awake()
    {
        transform.ObserveEveryValueChanged(transform => transform.position)
            .Where(_ => Time.time > _nextTrailTime)
            .Subscribe(_ =>
            {
                Instantiate(_trail, transform.position, transform.rotation);
                _nextTrailTime = Time.time + _trailDelay;
            });
    }


}

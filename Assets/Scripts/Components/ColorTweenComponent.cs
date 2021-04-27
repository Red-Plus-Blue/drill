using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class ColorTweenComponent : MonoBehaviour
{
    [SerializeField]
    protected Color _from;
    [SerializeField]
    protected Color _to;
    [SerializeField]
    protected Image _target;

    protected Sequence _animation;

    private void Awake()
    {
        _animation = DOTween.Sequence()
            .Append(_target.DOColor(_to, 0.5f))
            .Append(_target.DOColor(_from, 0.5f))
            .SetLoops(-1);
    }

    private void OnDestroy()
    {
        _animation.Kill();
    }
}

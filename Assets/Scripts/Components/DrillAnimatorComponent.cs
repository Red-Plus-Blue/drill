using UnityEngine;

using System.Linq;
using System.Collections.Generic;

using DG.Tweening;

using UniRx;

public class DrillAnimatorComponent : MonoBehaviour
{
    [SerializeField]
    protected SpriteRenderer _drillRenderer;
    [SerializeField]
    protected int _maxHeat;
    [SerializeField]
    protected Color _maxHeathColor;
    [SerializeField]
    protected SpriteRenderer _dirtRenderer;
    [SerializeField]
    protected List<Sprite> _dirtLevels;

    protected Color _startColor;

    protected int _heat;
    protected float _nextHeatDecayTime;

    protected int _damageToBlocks;

    [SerializeField]
    protected Transform _drill;

    [SerializeField]
    protected List<Sprite> _drillFrames;
    [SerializeField]
    protected SpriteRenderer _drillSprite;
    [SerializeField]
    protected List<Sprite> _trackFrames;
    [SerializeField]
    protected List<SpriteRenderer> _trackRenderers;

    protected Sequence _animation;

    protected int Frame => (int)(0.5 + Mathf.Cos(Time.time * 5.0f));

    private void Awake()
    {
        _startColor = _drillRenderer.color;

        _animation = DOTween.Sequence()
            .Append(_drill.DOLocalMoveY(-0.03f, .25f))
            .Append(_drill.DOLocalMoveY(0.03f, .25f))
            .SetLoops(-1);

        transform
            .ObserveEveryValueChanged(transform => transform.position)
            .Subscribe(_ => UpdateTracks());

        transform
            .ObserveEveryValueChanged(transform => transform.rotation)
            .Subscribe(_ => UpdateTracks());
    }

    private void Update()
    {
        _drillSprite.sprite = _drillFrames[Frame];

        if(Time.time > _nextHeatDecayTime)
        {
            _heat = Mathf.Max(0, _heat - 1);
            _nextHeatDecayTime = Time.time + 0.25f;
        }

        _drillRenderer.color = Color.Lerp(_startColor, _maxHeathColor, (float)_heat / _maxHeat);
    }

    public void DamageBlock()
    {
        _damageToBlocks += 1;
        _dirtRenderer.sprite = _dirtLevels[Mathf.Min(_dirtLevels.Count - 1, _damageToBlocks / 100)];
    }

    public void IncreaseHeat(int amount)
    {
        _heat = Mathf.Min(_maxHeat, _heat + amount);
        _nextHeatDecayTime = Time.time + 1.0f;
    }

    protected void UpdateTracks()
    {
        _trackRenderers
            .ForEach(track => track.sprite = _trackFrames[Frame]);
    }

    private void OnDestroy()
    {
        _animation.Kill();
    }
}

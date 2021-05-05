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
    protected Color _maxHeathColor;
    [SerializeField]
    protected SpriteRenderer _dirtRenderer;
    [SerializeField]
    protected List<Sprite> _dirtLevels;
    [SerializeField]
    protected List<ParticleSystem> _drillParticles;

    protected Color _startColor;

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

    protected (int, int) _heatBounds;

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

    private void Start()
    {
        var player = GetComponent<PlayerControllerComponent>();
        player.Heat.Bounds.Subscribe(bounds => _heatBounds = bounds);
        player.Heat.Value.Subscribe(UpdateHeat);
    }

    private void Update()
    {
        _drillSprite.sprite = _drillFrames[Frame];
        
    }

    public void DamageBlock()
    {
        _damageToBlocks += 1;
        _dirtRenderer.sprite = _dirtLevels[Mathf.Min(_dirtLevels.Count - 1, _damageToBlocks / 100)];
    }

    public void ShowDrillParticles()
    {
        _drillParticles
           .Where(particle => particle.isStopped)
           .ToList()
           .ForEach(particle => particle.Play());
    }

    public void HideDrillParticles()
    {
        _drillParticles.ForEach(particle => particle.Stop());
    }

    protected void UpdateHeat(int heat)
    {
        _drillRenderer.color = Color.Lerp(_startColor, _maxHeathColor, (float)heat / _heatBounds.Item2);
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

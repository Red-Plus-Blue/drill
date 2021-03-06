using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;

public class PlayerControllerComponent : MonoBehaviour
{
    protected static Player SavedPlayer = new Player()
    {
        Money = 0,
        Durability = 1_000f,
        Fuel = 100f
    };

    public IRange<int> Money => _money;
    public IRange<float> Fuel => _fuel;
    public IRange<float> Durability => _durability;
    public IRange<int> Heat => _heat;

    public bool InputLocked;
    public int MiningDamage = 20;
    protected int _baseMiningDamage = 20;

    [SerializeField]
    protected float _speed = 3f;
    [SerializeField]
    protected float _rotationSpeed = 90f;
    [SerializeField]
    protected GameObject _rockEffect;
    [SerializeField]
    protected Transform _drillHead;

    protected DrillAnimatorComponent _animator;

    protected Range<int> _money = new Range<int>(0, 0, 999_999_999);
    protected Range<float> _fuel = new Range<float>(100, 0, 100);
    protected Range<float> _durability = new Range<float>(1_000, 0, 1_000);
    protected Range<int> _heat = new Range<int>(0, 0, 100);

    protected float _drillDelay = 0.15f;

    protected float _nextDrillTime;
    protected float _nextHeatDecayTime;
    protected bool _outOfFuel;

    protected Rigidbody2D _rigidbody2D;

    protected Func<float> _vertical = () => Input.GetAxis("Vertical");
    protected Func<float> _horizontal = () => -Input.GetAxis("Horizontal");

    public bool CanBuy(int cost) => _money.Current > cost;

    private void Awake()
    {
        _animator = GetComponent<DrillAnimatorComponent>();
    }

    private void Start()
    {
        _money.Set(SavedPlayer.Money);
        _fuel.Set(SavedPlayer.Fuel);
        _durability.Set(SavedPlayer.Durability);
        _heat.Value.Subscribe(value => MiningDamage = _baseMiningDamage + (value / 5));
    }

    private void Update()
    {
        if(!_outOfFuel && (_fuel.Current <= 0f))
        {
            _outOfFuel = true;
            GameManagerComponent.Instance.Lose("Out of Fuel");
        }

        if (Time.time > _nextHeatDecayTime)
        {
            _heat.Set(_heat.Current - 2);
            _nextHeatDecayTime = Time.time + 0.25f;
        }
    }

    private void FixedUpdate()
    {
        var diggableTilemap = FindObjectOfType<DiggableTilemapComponent>();
        if (diggableTilemap)
        {
            diggableTilemap.Dig(
                Mathf.RoundToInt(_drillHead.position.x),
                Mathf.RoundToInt(_drillHead.position.y)
            );
        }

        if (InputLocked || _outOfFuel) { return; }

        var horizontal = _horizontal();
        var vertical = _vertical();

        if ((vertical != 0f) || (horizontal != 0f))
        {
            _fuel.Set(_fuel.Current - Time.fixedDeltaTime);
        }

        transform.position += transform.up * vertical * _speed * Time.fixedDeltaTime;
        transform.rotation *= Quaternion.Euler(0f, 0f, horizontal * _rotationSpeed * Time.fixedDeltaTime);
    }

    public void AttachInput(Func<float> vertical, Func<float> horizontal)
    {
        _vertical = vertical;
        _horizontal = horizontal;
    }

    public void AddMoney(int amount)
    {
        _money.Set(_money.Current + amount);
    }

    public void AddFuel(float amount)
    {
        _fuel.Set(_fuel.Current + amount);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.GetComponentInParent<EnemyComponent>();
        if(enemy && !other.isTrigger)
        {
            enemy.Stun();
            return;
        }

        var block = other.GetComponent<BlockComponent>();
        if (!block) { return; }

        _animator.ShowDrillParticles();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var block = other.GetComponent<BlockComponent>();
        if(!block) { return; }

        _animator.ShowDrillParticles();

        if ((Time.time >= _nextDrillTime) && (_durability.Current > 0f))
        {
            _durability.Set(_durability.Current - 1);
            _animator.DamageBlock();
            _heat.Set(_heat.Current + 1);
            block.TakeDamge(MiningDamage, true);
            var effect = Instantiate(_rockEffect, block.transform);
            Destroy(effect, 0.6f);
            _nextDrillTime = Time.time + _drillDelay;
            _nextHeatDecayTime = Time.time + 1.0f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var block = other.GetComponent<BlockComponent>();
        if (!block) { return; }

        _animator.HideDrillParticles();
    }

    private void OnDestroy()
    {
        _money.RemoveAllListeners();
        _fuel.RemoveAllListeners();
        _durability.RemoveAllListeners();
        _heat.RemoveAllListeners();
    }

    public void Die()
    {
        gameObject.SetActive(false);
        SavedPlayer = new Player()
        {
            Money = _money.Minimum,
            Durability = _durability.Maximum,
            Fuel = _fuel.Maximum
        };
        GameManagerComponent.Instance.Lose("Destroyed");
    }

    public void RepairDrill()
    {
        _durability.Set(_durability.Maximum);
    }

    public void ResetDefaults()
    {
        _money.Set(_money.Minimum);
        _durability.Set(_durability.Maximum);
        _fuel.Set(_fuel.Maximum);
    }

    public void StoreResults()
    {
        SavedPlayer = new Player()
        {
            Money = _money.Current,
            Durability = _durability.Current,
            Fuel = _fuel.Current
        };
    }

}

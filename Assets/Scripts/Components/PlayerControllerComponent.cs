using UnityEngine;

using System.Linq;
using System.Collections.Generic;

public class PlayerControllerComponent : MonoBehaviour
{
    public IObservable<int> Money => _money;
    public IObservable<float> FuelLevel => _fuelLevel;
    public IObservable<float> Durability => _durability;

    public bool CanBuy(int cost) => _money.Get() > cost;

    public bool InputLocked;

    public int MiningDamage = 2;

    protected static Player SavedPlayer = new Player()
    {
        Money = 0,
        Durability = 1_000f,
        Fuel = 100f
    };

    [SerializeField]
    protected float _speed = 3f;
    [SerializeField]
    protected float _rotationSpeed = 90f;
    [SerializeField]
    protected GameObject _rockEffect;
    [SerializeField]
    protected List<ParticleSystem> _drillParticles;

    protected DrillAnimatorComponent _animator;

    protected Subject<int> _money = new Subject<int>(0);
    protected Subject<float> _fuelLevel = new Subject<float>(0f);
    protected Subject<float> _durability = new Subject<float>(0f);

    protected float _drillDurabilityMax = 1_000;
    protected float _drillDurability = 1_000;

    protected float _drillDelay = 0.15f;

    protected float _nextDrillTime;
    protected bool _outOfFuel;

    protected float _currentFuel = 100f;
    protected float _maxFuel = 100f;

    protected Rigidbody2D _rigidbody2D;

    private void Awake()
    {
        _animator = GetComponent<DrillAnimatorComponent>();
    }

    private void Start()
    {
        _money.Set(SavedPlayer.Money);

        _currentFuel = SavedPlayer.Fuel;
        _fuelLevel.Set(_currentFuel / _maxFuel);

        _drillDurability = SavedPlayer.Durability;
        _durability.Set(_drillDurability / _drillDurabilityMax);
    }

    private void Update()
    {
        if(!_outOfFuel && (_currentFuel <= 0f))
        {
            _outOfFuel = true;
            GameManagerComponent.Instance.Lose("Out of Fuel");
        }
    }

    private void FixedUpdate()
    {
        if(InputLocked || _outOfFuel) { return; }

        var horizontal = -Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        if((vertical != 0f) || (horizontal != 0f))
        {
            _currentFuel -= Time.fixedDeltaTime;
            _fuelLevel.Set(_currentFuel / _maxFuel);
        }

        transform.position += transform.up * vertical * _speed * Time.fixedDeltaTime;
        transform.rotation *= Quaternion.Euler(0f, 0f, horizontal * _rotationSpeed * Time.fixedDeltaTime);
    }

    public void AddMoney(int amount)
    {
        _money.Set(_money.Get() + amount);
    }

    public void AddFuel(float amount)
    {
        _currentFuel = Mathf.Min(_maxFuel, _currentFuel + amount);
        _fuelLevel.Set(_currentFuel / _maxFuel);
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

        _drillParticles
            .Where(particle => particle.isStopped)
            .ToList()
            .ForEach(particle => particle.Play());
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var block = other.GetComponent<BlockComponent>();
        if(!block) { return; }

        _drillParticles
            .Where(particle => particle.isStopped)
            .ToList()
            .ForEach(particle => particle.Play());

        if ((Time.time >= _nextDrillTime) && (_drillDurability > 0f))
        {
            _drillDurability -= 1;
            _durability.Set(_drillDurability / _drillDurabilityMax);
            _animator.DamageBlock();
            _animator.IncreaseHeat(1);
            block.TakeDamge(MiningDamage, true);
            var effect = Instantiate(_rockEffect, block.transform);
            Destroy(effect, 0.6f);
            _nextDrillTime = Time.time + _drillDelay;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var block = other.GetComponent<BlockComponent>();
        if (!block) { return; }

        _drillParticles.ForEach(particle => particle.Stop());
    }

    public void Die()
    {
        gameObject.SetActive(false);
        SavedPlayer = new Player()
        {
            Money = 0,
            Durability = 1_000f,
            Fuel = 100
        };
        GameManagerComponent.Instance.Lose("Destroyed");
    }

    public void RepairDrill()
    {
        _drillDurability = _drillDurabilityMax;
        _durability.Set(_drillDurability / _drillDurabilityMax);
    }

    public void ResetDefaults()
    {
        _money.Set(0);
        _drillDurability = 1_000f;
        _currentFuel = 100f;
    }

    public void StoreResults()
    {
        SavedPlayer = new Player()
        {
            Money = _money.Get(),
            Durability = _drillDurability,
            Fuel = _currentFuel
        };
    }

}

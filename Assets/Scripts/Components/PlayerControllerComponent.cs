using UnityEngine;

using System.Linq;
using System.Collections.Generic;

public class PlayerControllerComponent : MonoBehaviour
{
    public bool InputLocked;

    [SerializeField]
    protected float _speed = 3f;
    [SerializeField]
    protected float _rotationSpeed = 90f;
    [SerializeField]
    protected GameObject _rockEffect;
    [SerializeField]
    protected List<ParticleSystem> _drillParticles;

    protected DrillAnimatorComponent _animator;

    protected float _drillDurabilityMax = 1_000;
    protected float _drillDurability = 1_000;

    protected float _drillDelay = 0.15f;

    protected float _nextDrillTime;
    protected bool _outOfFuel;

    protected float _currentFuel = 100f;
    protected float _maxFuel = 100f;

    public int Money { get; protected set; }

    public int MiningDamage = 2;

    protected Rigidbody2D _rigidbody2D;
    protected UIComponent _ui;

    private void Awake()
    {
        _animator = GetComponent<DrillAnimatorComponent>();
        _ui = FindObjectOfType<UIComponent>();
    }

    private void Start()
    {
        Money = GameManagerComponent.Instance.Money;
        _ui.SetMoney(Money);

        _currentFuel = GameManagerComponent.Instance.Fuel;
        _ui.SetFuelLevel(_currentFuel / _maxFuel);

        _drillDurability = GameManagerComponent.Instance.Durability;
        _ui.SetDurability(_drillDurability / _drillDurabilityMax);
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
            _ui.SetFuelLevel(_currentFuel / _maxFuel);
        }

        transform.position += transform.up * vertical * _speed * Time.fixedDeltaTime;
        transform.rotation *= Quaternion.Euler(0f, 0f, horizontal * _rotationSpeed * Time.fixedDeltaTime);
    }

    public void AddMoney(int amount)
    {
        Money += amount;
        _ui.SetMoney(Money);
    }

    public void AddFuel(float amount)
    {
        _currentFuel = Mathf.Min(_maxFuel, _currentFuel + amount);
        _ui.SetFuelLevel(_currentFuel / _maxFuel);
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
            _animator.DamageBlock();
            _animator.IncreaseHeat(1);
            _ui.SetDurability(_drillDurability / _drillDurabilityMax);
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
        GameManagerComponent.Instance.Lose("Destroyed");
    }

    public void RepairDrill()
    {
        _drillDurability = _drillDurabilityMax;
        _ui.SetDurability(_drillDurability / _drillDurabilityMax);
    }

    public void StoreResults()
    {
        GameManagerComponent.Instance.Fuel = _currentFuel;
        GameManagerComponent.Instance.Money = Money;
        GameManagerComponent.Instance.Durability = _drillDurability;
    }

}

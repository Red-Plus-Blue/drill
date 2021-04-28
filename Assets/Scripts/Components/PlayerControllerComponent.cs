using UnityEngine;

using System.Linq;
using System.Collections.Generic;

public class PlayerControllerComponent : MonoBehaviour
{
    public IRange<int> Money => _money;
    public IRange<float> Fuel => _fuel;
    public IRange<float> Durability => _durability;

    public bool CanBuy(int cost) => _money.Current > cost;

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

    protected Range<int> _money = new Range<int>(0, 0, 999_999_999);
    protected Range<float> _fuel = new Range<float>(100, 0, 100);
    protected Range<float> _durability = new Range<float>(1_000, 0, 1_000);

    protected float _drillDelay = 0.15f;

    protected float _nextDrillTime;
    protected bool _outOfFuel;

    protected Rigidbody2D _rigidbody2D;

    private void Awake()
    {
        _animator = GetComponent<DrillAnimatorComponent>();
    }

    private void Start()
    {
        _money.Set(SavedPlayer.Money);
        _fuel.Set(SavedPlayer.Fuel);
        _durability.Set(SavedPlayer.Durability);
    }

    private void Update()
    {
        if(!_outOfFuel && (_fuel.Current <= 0f))
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
            _fuel.Set(_fuel.Current - Time.fixedDeltaTime);
        }

        transform.position += transform.up * vertical * _speed * Time.fixedDeltaTime;
        transform.rotation *= Quaternion.Euler(0f, 0f, horizontal * _rotationSpeed * Time.fixedDeltaTime);
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

        if ((Time.time >= _nextDrillTime) && (_durability.Current > 0f))
        {
            _durability.Set(_durability.Current - 1);
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

    private void OnDestroy()
    {
        _money.RemoveAllListeners();
        _fuel.RemoveAllListeners();
        _durability.RemoveAllListeners();
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

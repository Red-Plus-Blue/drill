using UnityEngine;

using System;

using Random = UnityEngine.Random;

public class EnemyComponent : MonoBehaviour
{
    [SerializeField]
    protected float _moveSpeed = 2.0f;
    [SerializeField]
    protected Transform _turret;
    [SerializeField]
    protected Transform _firePoint;
    [SerializeField]
    protected LaserComponent _laserPrefab;
    [SerializeField]
    protected GameObject _sparksPrefab;

    protected Action _state;

    protected BlockComponent _target;
    protected float _nextMineTime;

    protected Vector3 _targetVector;
    protected PlayerControllerComponent _player;

    protected bool _isAiming;
    protected float _stunEndTime;

    protected float _shootCooldownEnd;

    protected LayerMask _layers;

    private void Awake()
    {
        _layers = LayerMask.GetMask("Player", "Block", "BlockNoCollide", "Default");

        var axis = (Random.Range(0, 2) == 0) ? transform.up : transform.right;
        var sign = (Random.Range(0, 2) == 0) ? 1 : -1;
        transform.up = axis * sign;
        
        _state = StatePatrol;
    }

    private void FixedUpdate()
    {
        if(Time.time < _stunEndTime)
        {
            return;
        }

        if (_isAiming && Aim())
        {
            return;
        }
        else
        {
            _turret.rotation = Quaternion.RotateTowards(_turret.rotation, transform.rotation, 90.0f * Time.deltaTime);
        }

        _state?.Invoke();
    }

    public void Stun()
    {
        if(Time.time < _stunEndTime)
        {
            return;
        }

        var stunDuration = 5.0f;
        var sparks = Instantiate(_sparksPrefab, transform);
        sparks.transform.localScale = Vector3.one;
        Destroy(sparks, stunDuration);
        _stunEndTime = Time.time + stunDuration;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var player = other.GetComponentInParent<PlayerControllerComponent>();
        if(!player) { return; }

        _isAiming = true;
        _player = player;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponentInParent<PlayerControllerComponent>();
        if (!player) { return; }

        _isAiming = true;
        _player = player;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _targetVector = -transform.up;
        _state = StateTurnAround;
    }

    protected bool Aim()
    {
        if(Time.time < _shootCooldownEnd)
        {
            return false;
        }

        var direction = _player.transform.position - _turret.position;
        var lookRotation = Quaternion.LookRotation(_turret.forward, direction);
        if (Quaternion.Angle(lookRotation, _turret.rotation) < 0.1f)
        {
            Shoot();
            return true;
        }

        var hit = Physics2D.Raycast(_turret.position, direction);
        if (hit.collider)
        {
            if(!hit.collider.GetComponentInParent<PlayerControllerComponent>())
            {
                _isAiming = false;
                _player = null;
                return false;
            }
        }

        _turret.rotation = Quaternion.RotateTowards(_turret.rotation, lookRotation, 45.0f * Time.deltaTime);
        return true;
    }

    public void Shoot()
    {
        var laser = Instantiate(_laserPrefab, Vector3.zero, Quaternion.identity);
        laser.Initialize(_firePoint, _player.transform);
        _player.Die();
        _shootCooldownEnd = Time.time + 2f;
    }

    protected void StatePatrol()
    {
        var hit = Physics2D.Raycast(transform.position, transform.up, 0.5f, _layers);
        if(hit.collider)
        {
            var exit = hit.collider.GetComponent<ExitComponent>();
            if(exit)
            {
                _targetVector = -transform.up;
                _state = StateTurnAround;
                return;
            }

            var elevator = hit.collider.GetComponent<ElevatorComponent>();
            if(elevator)
            {
                _targetVector = -transform.up;
                _state = StateTurnAround;
                return;
            }


            var block = hit.collider.GetComponent<BlockComponent>();
            if(block)
            {
                if(block.Impassable)
                {
                    _targetVector = -transform.up;
                    _state = StateTurnAround;
                    return;
                }
                _state = StateMine;
                _target = block;
            }
            return;
        }

        transform.position += transform.up * _moveSpeed * Time.fixedDeltaTime;
    }

    protected void StateMine()
    {
        if(Time.time > _nextMineTime)
        {
            if(!_target)
            {
                _targetVector = -transform.up;
                _state = StateTurnAround;
                return;
            }

            _target.TakeDamge(3, false);
            _nextMineTime = Time.time + 0.3f;
        }
    }

    protected void StateTurnAround()
    {
        var targetRotation = Quaternion.LookRotation(transform.forward, _targetVector);
        if(Quaternion.Angle(targetRotation, transform.rotation) < 0.5f)
        {
            _state = StatePatrol;
            return;
        }

        transform.rotation *= Quaternion.Euler(0f, 0f, 180.0f * Time.deltaTime);
    }
}

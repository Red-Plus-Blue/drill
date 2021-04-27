using UnityEngine;

using System;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public class BlockComponent : MonoBehaviour
{
    protected const string LAYER_NO_COLLIDE = "BlockNoCollide";

    public int Money;
    public float Fuel;
    public bool Impassable { get => _impassable; }

    [SerializeField]
    protected bool _impassable;
    [SerializeField]
    protected Sprite _damagedSprite;
    [SerializeField]
    protected List<GameObject> _spawnOnDeath;

    protected SpriteRenderer _renderer;
    protected Block _block;

    private void Awake()
    {
        if(_spawnOnDeath.Count == 0) { _spawnOnDeath = null;  }

        _renderer = GetComponentInChildren<SpriteRenderer>();
        _renderer.flipX = Random.Range(0, 2) == 0;
        _renderer.flipY = Random.Range(0, 2) == 0;
    }

    public void TakeDamge(int amount, bool isPlayer)
    {
        if(_impassable) { return; }
        if(_block == null)
        {
            TakeFirstDamage();
        }
        _block.TakeDamage(amount, isPlayer);

        if(!_block.Dead)
        {
            transform.localScale = (0.3f + (0.7f * (Convert.ToSingle(_block.Health) / _block.HealthMax))) * Vector3.one;
        }
    }

    protected void TakeFirstDamage()
    {
        gameObject.layer = LayerMask.NameToLayer(LAYER_NO_COLLIDE);
        _renderer.flipX = false;
        _renderer.flipY = false;
        _renderer.sprite = _damagedSprite;
        _block = new Block();
        _block.OnDeath += OnBlockDeath;
    }

    protected void OnBlockDeath(bool isPlayer)
    {
        if(isPlayer)
        {
            _spawnOnDeath?.ForEach(prefab => Instantiate(
                prefab,
                transform.position + (Vector3)(Random.insideUnitCircle.normalized * 0.5f),
                Quaternion.identity));

            var player = FindObjectOfType<PlayerControllerComponent>();
            player.AddMoney(Money);
            player.AddFuel(Fuel);
        }
        Destroy(gameObject);
    }

}

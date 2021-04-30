using UnityEngine;

using System;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public class BlockComponent : MonoBehaviour
{
    protected const string LAYER_NO_COLLIDE = "BlockNoCollide";

    public BlockDefinition Definition { 
        get => _definition; 
        set => _definition = value;
    }

    [SerializeField]
    protected BlockDefinition _definition;

    protected SpriteRenderer _renderer;
    protected Block _block;

    private void Awake()
    {
        if(_definition.SpawnOnDeath?.Count == 0) { _definition.SpawnOnDeath = null;  }

        _renderer = GetComponentInChildren<SpriteRenderer>();
        _renderer.flipX = Random.Range(0, 2) == 0;
        _renderer.flipY = Random.Range(0, 2) == 0;
    }

    public void TakeDamge(int amount, bool isPlayer)
    {
        if(_definition.Impassable) { return; }
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
        _renderer.sprite = _definition.DamagedSprite;
        _block = new Block();
        _block.OnDeath += OnBlockDeath;
    }

    protected void OnBlockDeath(bool isPlayer)
    {
        if(isPlayer)
        {
            _definition.SpawnOnDeath?.ForEach(prefab => Instantiate(
                prefab,
                transform.position + (Vector3)(Random.insideUnitCircle.normalized * 0.5f),
                Quaternion.identity));

            var player = FindObjectOfType<PlayerControllerComponent>();
            player.AddMoney(_definition.Money);
            player.AddFuel(_definition.Fuel);
        }
        Destroy(gameObject);
    }

}

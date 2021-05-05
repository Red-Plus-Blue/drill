using UnityEngine;

public class IncreaseMiningSpeedEffect : MonoBehaviour
{
    protected PlayerControllerComponent _player;
    protected int _initialMiningDamage;

    private void Awake()
    {
        _player = FindObjectOfType<PlayerControllerComponent>();
        _initialMiningDamage = _player.MiningDamage;
        _player.MiningDamage *= 2;
        Destroy(gameObject, 30.0f);
    }

    private void OnDestroy()
    {
        if(!_player)
        {
            return;
        }
        _player.MiningDamage = _initialMiningDamage;
    }

}

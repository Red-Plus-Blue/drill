using UnityEngine;

public class IncreaseMiningSpeedEffect : MonoBehaviour
{
    protected PlayerControllerComponent _player;

    private void Awake()
    {
        _player = FindObjectOfType<PlayerControllerComponent>();
        _player.MiningDamage = 4;
        Destroy(gameObject, 30.0f);
    }

    private void OnDestroy()
    {
        if(!_player)
        {
            return;
        }
        _player.MiningDamage = 2;
    }

}

using UnityEngine;
using UnityEngine.Events;

using TMPro;

public class ShopComponent : MonoBehaviour
{
    protected static ShopComponent _currentShop;

    [SerializeField]
    protected string _item;
    [SerializeField]
    protected GameObject _effect;
    [SerializeField]
    protected int _cost;
    [SerializeField]
    protected TMP_Text _title;

    protected UIComponent _ui;
    protected PlayerControllerComponent _player;

    private void Awake()
    {
        _ui = FindObjectOfType<UIComponent>();
        _player = FindObjectOfType<PlayerControllerComponent>();
        _title.text = _item;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.GetComponentInParent<PlayerControllerComponent>()) { return; }
        _currentShop = this;
        var cost = _cost.ToString("###,###,##0");
        _ui.SetPrompt($"Buy: {_item}: (${cost})\nPess 'q' to buy");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(_currentShop == this)
        {
            _currentShop = null;
            _ui.HidePrompt();
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q) && _currentShop == this)
        {
            if(_player.Money >= _cost)
            {
                _player.AddMoney(-_cost);
                Instantiate(_effect, Vector3.zero, Quaternion.identity);
            }
        }
    }
}

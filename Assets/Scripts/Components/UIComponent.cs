using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using TMPro;

public class UIComponent : MonoBehaviour
{
    [SerializeField]
    protected TMP_Text _level;
    [SerializeField]
    protected TMP_Text _money;
    [SerializeField]
    protected TMP_Text _prompt;
    [SerializeField]
    protected RectTransform _fuel;
    [SerializeField]
    protected RectTransform _durability;
    [SerializeField]
    protected GameObject _defeatScreen;
    [SerializeField]
    protected TMP_Text _defeatText;
    [SerializeField]
    protected List<GameObject> _ui;
    [SerializeField]
    protected RectTransform _joystick;

    protected PlayerControllerComponent _player;

    protected (float, float) _fuelRange;
    protected (float, float) _durabilityRange;

    private void Start()
    {
        var player = FindObjectOfType<PlayerControllerComponent>();
        player.Money.Value.Subscribe(SetMoney);
        player.Durability.Bounds.Subscribe(value => _durabilityRange = value);
        player.Durability.Value.Subscribe(SetDurability);

        player.Fuel.Bounds.Subscribe(value => _fuelRange = value);
        player.Fuel.Value.Subscribe(SetFuelLevel);

        _level.text = $"Level: {GameManagerComponent.Instance.Level}";

#if UNITY_ANDROID
  player.AttachInput(
            () =>
            {
                var y = new Vector2(_joystick.localPosition.x, _joystick.localPosition.y).normalized.y;
                return Mathf.Abs(y) < 0.05f ? 0f : y;
            },
            () =>
            {
                var x = new Vector2(_joystick.localPosition.x, _joystick.localPosition.y).normalized.x;
                return -(Mathf.Abs(x) < 0.05f ? 0f : x);
            }
        );
#endif
    }

    public void SetDurability(float durability)
    {
        var value = durability / _durabilityRange.Item2;
        _durability.localScale = new Vector3(value , _durability.localScale.y, _durability.localScale.z);
    }

    public void SetFuelLevel(float fuel)
    {
        var value = fuel / _fuelRange.Item2;
        _fuel.localScale = new Vector3(value, _fuel.localScale.y, _fuel.localScale.z);
    }

    public void SetMoney(int amount)
    {
        _money.text = amount.ToString("###,###,##0");
    }

    public void SetPrompt(string text)
    {
        _prompt.gameObject.SetActive(true);
        _prompt.text = text;
    }

    public void HidePrompt()
    {
        _prompt.gameObject.SetActive(false);
    }

    public void ShowUI()
    {
        _ui.ForEach(component => component.SetActive(true));
    }

    public void HideDefeatScreen()
    {
        _defeatScreen.SetActive(false);
    }

    public void ShowDefeatScreen(string details)
    {
        _defeatScreen.SetActive(true);
        _defeatText.text = details;
    }
}

using UnityEngine;

using System.Collections.Generic;

using TMPro;

public class UIComponent : MonoBehaviour
{
    [SerializeField]
    protected TMP_Text _level;
    [SerializeField]
    protected TMP_Text _money;
    [SerializeField]
    protected RectTransform _needle;
    [SerializeField]
    protected TMP_Text _prompt;
    [SerializeField]
    protected RectTransform _durability;
    [SerializeField]
    protected GameObject _defeatScreen;
    [SerializeField]
    protected TMP_Text _defeatText;
    [SerializeField]
    protected List<GameObject> _ui;
  

    protected float _fuelLevel;

    private void Start()
    {
        _level.text = $"Level: {GameManagerComponent.Instance.Level}";
    }

    public void SetDurability(float durability)
    {
        _durability.localScale = new Vector3(durability, _durability.localScale.y, _durability.localScale.z);
    }

    public void SetFuelLevel(float level)
    {
        _fuelLevel = level;
        _needle.rotation = Quaternion.Euler(0f, 0f, 80f + (-160 * level));
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

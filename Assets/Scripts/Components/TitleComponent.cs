using UnityEngine;

using System.Linq;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

public class TitleComponent : MonoBehaviour
{
    [SerializeField]
    protected RectTransform _wordHolder;
    [SerializeField]
    protected GameObject _title;

    public bool SkipTitle;

    private void Awake()
    {

        if(!SkipTitle && (!GameManagerComponent.Instance || GameManagerComponent.Instance.IsFirstLoad))
        {
            StartCoroutine(ShowTitle());
        }
        else
        {
            _title.SetActive(false);
            FindObjectOfType<UIComponent>().ShowUI();
        }
    }

    protected IEnumerator ShowTitle()
    {
        FindObjectOfType<PlayerControllerComponent>().InputLocked = true;
        yield return new WaitForSeconds(0.1f);
        var words = _wordHolder
            .GetComponentsInChildren<Transform>(true)
            .ToList();

        foreach(var word in words)
        {
            word.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void OnClickStart()
    {
        FindObjectOfType<PlayerControllerComponent>().InputLocked = false;
        GameManagerComponent.Instance.StartGame(true);
    }
}

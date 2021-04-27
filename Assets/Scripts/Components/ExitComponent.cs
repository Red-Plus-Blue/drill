using UnityEngine;

public class ExitComponent : MonoBehaviour
{
    public static int TouchingExitCount;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q) && (TouchingExitCount > 0))
        {
            FindObjectOfType<PlayerControllerComponent>().StoreResults();
            FindObjectOfType<GameManagerComponent>().ExitLevel(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.GetComponentInParent<PlayerControllerComponent>()) { return; }
        TouchingExitCount += 1;
        FindObjectOfType<UIComponent>().SetPrompt("Press Q to\nGo Deeper");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.GetComponentInParent<PlayerControllerComponent>()) { return; }
        TouchingExitCount -= 1;

        if(TouchingExitCount <= 0)
        {
            FindObjectOfType<UIComponent>().HidePrompt();
        }
    }
}

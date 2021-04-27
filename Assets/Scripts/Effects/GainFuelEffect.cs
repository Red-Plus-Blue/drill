using UnityEngine;

public class GainFuelEffect : MonoBehaviour
{
    private void Awake()
    {
        FindObjectOfType<PlayerControllerComponent>().AddFuel(100f);
        Destroy(gameObject);
    }
}

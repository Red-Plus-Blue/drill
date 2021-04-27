using UnityEngine;

public class RepairDrillEffect : MonoBehaviour
{
    private void Awake()
    {
        FindObjectOfType<PlayerControllerComponent>().RepairDrill();
        Destroy(gameObject);
    }

}

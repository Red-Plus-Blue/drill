using UnityEngine;

public class RemoveBlocksEffect : MonoBehaviour
{
    public int X;
    public int Y;
    public int Width;
    public int Height;

    private void Awake()
    {
        FindObjectOfType<MapComponent>().SelectBox(X, Y, Width, Height, (block) => Destroy(block.gameObject));
        Destroy(gameObject);
    }
}

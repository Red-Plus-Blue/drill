using UnityEngine;
using UnityEngine.Tilemaps;

public class DiggableTilemapComponent : MonoBehaviour
{
    [SerializeField]
    protected Tilemap _tileMap;
    [SerializeField]
    protected GameObject _blockPrefab;

    public void Dig(int x, int y)
    {
        var tile = _tileMap.GetTile(new Vector3Int(x, y, 0));
        if(tile)
        {
            _tileMap.SetTile(new Vector3Int(x, y, 0), null);
            Instantiate(_blockPrefab, new Vector3(x, y, 0), Quaternion.identity);
        }
    }
}

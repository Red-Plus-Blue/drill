using UnityEngine;
using UnityEngine.Tilemaps;

using System.Linq;

public class TilemapShadow : MonoBehaviour
{
    [SerializeField]
    protected Tilemap _tileMap;
    [SerializeField]
    protected Tilemap _shadowMap;
    [SerializeField]
    protected Tile _shadow;

    private void Awake()
    {
        Tilemap.tilemapTileChanged += OnTileMapChanged;
    }

    private void OnDestroy()
    {
        Tilemap.tilemapTileChanged -= OnTileMapChanged;
    }
    protected void OnTileMapChanged(Tilemap tilemap, Tilemap.SyncTile[] tiles)
    {
        if (tilemap != _tileMap) { return; }
        tiles.ToList().ForEach(delta => _shadowMap.SetTile(delta.position, delta.tile ? _shadow : null));
    }
}

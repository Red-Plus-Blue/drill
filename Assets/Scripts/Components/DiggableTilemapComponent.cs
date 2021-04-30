﻿using UnityEngine;
using UnityEngine.Tilemaps;

public class DiggableTilemapComponent : MonoBehaviour
{
    [SerializeField]
    protected Tilemap _tileMap;
    [SerializeField]
    protected BlockComponent _blockPrefab;

    public void Dig(int x, int y)
    {
        var tile = _tileMap.GetTile(new Vector3Int(x, y, 0)) as BlockTile;
        if(tile)
        {
            if(tile.Definition.Impassable) { return; }
            _tileMap.SetTile(new Vector3Int(x, y, 0), null);
            var block = Instantiate(_blockPrefab, new Vector3(x, y, 0), Quaternion.identity);
            block.Definition = tile.Definition;
        }
    }
}

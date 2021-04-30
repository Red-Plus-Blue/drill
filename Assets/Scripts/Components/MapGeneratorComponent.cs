using UnityEngine;
using UnityEngine.Tilemaps;

using System;
using System.Linq;
using System.Collections.Generic;

using TMPro;

using Random = UnityEngine.Random;

public class MapGeneratorComponent : MonoBehaviour
{
    [SerializeField]
    protected GameObject _enemyPrefab;
    [SerializeField]
    protected BlockComponent _blockPrefab;
    [SerializeField]
    protected BlockComponent _fuelBlock;
    [SerializeField]
    protected List<BlockComponent> _moneyBlocks;
    [SerializeField]
    protected BlockComponent _impassableBlockPrefab;
    [SerializeField]
    protected Transform _background;
    [SerializeField]
    protected GameObject _exitPrefab;

    [SerializeField]
    protected Tile _tile;
    [SerializeField]
    protected Tile _impassableTile;
    [SerializeField]
    protected Tilemap _dirt;

    protected int _width;
    protected int _height;

    protected BlockComponent[,] _blocks;

    public bool IsOnMap(int x, int y) => (x >= 0) && (y >= 0) && (x < _blocks.GetLength(0)) && (y < _blocks.GetLength(1));

    private void Start()
    {
        _width = 25 + Random.Range((GameManagerComponent.Instance.Level * 2), (GameManagerComponent.Instance.Level * 4));
        _height = 25 + Random.Range((GameManagerComponent.Instance.Level * 2), (GameManagerComponent.Instance.Level * 4));
        GenerateMap();
    }

    protected void ResizeBackground()
    {
        _background.localScale = new Vector3(_width, _height, 1f);
        _background.position = new Vector3((_width / 2f) - 0.5f, (_height / 2f) - 0.5f, 1f);
        var material = _background.GetComponent<MeshRenderer>().material;
        material.SetTextureScale("_MainTex", new Vector2(_width, _height));
    }

    protected void GenerateMap()
    {
        ResizeBackground();

        // Fill with blocks
        _blocks = new BlockComponent[_width, _height];
        Enumerable.Range(0, _height)
            .ToList()
            .ForEach(y => {
                Enumerable.Range(0, _width)
                    .ToList()
                    .ForEach(x => {
                        var name = $"Block: ({x} ,{y})";
                        var position = new Vector3(x, y, transform.position.z);
                        var isEdge = (x <= 2) || (y <= 2) || (x >= _width - 3) || (y >= _height - 3);
                        var tile = isEdge ? _impassableTile : _tile;
                        
                        /*
                        if (prefab != _impassableBlockPrefab)
                        {
                            prefab = (Random.Range(0, 10) < 1) ? _moneyBlocks[Random.Range(0, _moneyBlocks.Count)] : prefab;
                        }
                        if (prefab != _impassableBlockPrefab)
                        {
                            prefab = (Random.Range(0, 75) < 1) ? _fuelBlock : prefab;
                        }*/

                        _dirt.SetTile(new Vector3Int(x, y, 0), tile);

                        //var block = Instantiate(prefab, position, Quaternion.identity);
                        //block.name = name;
                        //_blocks[x, y] = block;
                    });
            });

        /*

        Enumerable.Range(2, 4).ToList().ForEach(_ =>
        {
            var spawn = new Vector2Int(
                Random.Range((int)(0.15f * _width), (int)(0.85f * _width)),
                Random.Range((int)(0.15f * _height), (int)(0.85f * _height))
            );

            Destroy(_blocks[spawn.x, spawn.y].gameObject);
            Instantiate(_enemyPrefab, new Vector3(spawn.x, spawn.y, transform.position.z), Quaternion.identity);
        });

        // Spawn Player
        var spawn = new Vector2Int(
            Random.Range((int)(0.15f * _width), (int)(0.85f * _width)),
            Random.Range((int)(0.15f * _height), (int)(0.85f * _height))
        );

        DoForEachInRange(spawn, 2, (block) => Destroy(block.gameObject));

        var player = FindObjectOfType<PlayerControllerComponent>();
        player.transform.position = new Vector3(spawn.x, spawn.y, transform.position.z);
        var elevator = GameObject.FindGameObjectWithTag("Elevator");
        elevator.transform.position = new Vector3(spawn.x, spawn.y, transform.position.z);

        // Generate Exit
        var isX = Random.Range(0, 2) == 0;
        var isMaximum = Random.Range(0, 2) == 0;
        var position = Random.Range(0, isX ? _width : _height);
        var x = isX ? position : (isMaximum ? _width - 2 : 1);
        var y = !isX ? position : (isMaximum ? _height - 2 : 1);

        DoForEachInRange(new Vector2Int(x, y), 2, (block) =>
        {
            Destroy(block.gameObject);
            Instantiate(_exitPrefab, block.gameObject.transform.position, Quaternion.identity);
        });
        */
    }

    public void SelectBox(int lowerLeftX, int lowerLeftY, int width, int height, Action<BlockComponent> doForEach)
    {
        Enumerable.Range(0, height)
           .ToList()
           .ForEach(yOffset => {
               Enumerable.Range(0, width)
                   .ToList()
                   .ForEach(xOffset => {
                       var x = lowerLeftX + xOffset;
                       var y = lowerLeftY + yOffset;
                       var block = _blocks[x, y];
                       doForEach(block);
                   });
           });
    }
    protected void DoForEachInRange(Vector2Int center, int range, Action<BlockComponent> action)
    {
        for (var yOffset = -range; yOffset <= range; yOffset++)
        {
            for (var xOffset = -range; xOffset <= range; xOffset++)
            {
                var position = center + new Vector2Int(xOffset, yOffset);
                if (position.x == 0 || position.y == 0 || position.x == _width - 1 || position.y == _height - 1)
                {
                    continue;
                }

                if (!IsOnMap(position.x, position.y)) { continue; }
                action(_blocks[position.x, position.y]);
            }
        }
    }
}

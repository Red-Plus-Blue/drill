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
    protected Tile _block;
    [SerializeField]
    protected Tile _impassableBlock;
    [SerializeField]
    protected Tile _fuelBlock;
    [SerializeField]
    protected List<Tile> _moneyBlocks;
    [SerializeField]
    protected Transform _background;
    [SerializeField]
    protected GameObject _exitPrefab;

    [SerializeField]
    protected Tilemap _dirt;

    protected int _width;
    protected int _height;

    public bool IsOnMap(int x, int y) => (x >= 0) && (y >= 0) && (x < _width) && (y < _height);

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
        Enumerable.Range(0, _height)
            .ToList()
            .ForEach(y => {
                Enumerable.Range(0, _width)
                    .ToList()
                    .ForEach(x => {
                        var name = $"Block: ({x} ,{y})";
                        var position = new Vector3(x, y, transform.position.z);
                        var isEdge = (x <= 2) || (y <= 2) || (x >= _width - 3) || (y >= _height - 3);
                        var block = isEdge ? _impassableBlock : _block;
                        if (block != _impassableBlock)
                        {
                            block = (Random.Range(0, 10) < 1) ? _moneyBlocks[Random.Range(0, _moneyBlocks.Count)] : block;
                        }
                        if (block != _impassableBlock)
                        {
                            block = (Random.Range(0, 75) < 1) ? _fuelBlock : block;
                        }

                        _dirt.SetTile(new Vector3Int(x, y, 0), block);
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
        */

        // Spawn Player
        var spawn = new Vector2Int(
            Random.Range((int)(0.15f * _width), (int)(0.85f * _width)),
            Random.Range((int)(0.15f * _height), (int)(0.85f * _height))
        );

        DoForEachInRange(spawn, 2, (position) => _dirt.SetTile((Vector3Int)position, null));

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

        DoForEachInRange(new Vector2Int(x, y), 2, (position) =>
        {
            _dirt.SetTile((Vector3Int)position, null);
            Instantiate(_exitPrefab, new Vector3(position.x, position.y, transform.position.z), Quaternion.identity);
        });
    }

    protected void DoForEachInRange(Vector2Int center, int range, Action<Vector2Int> action)
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
                action?.Invoke(position);
            }
        }
    }
}

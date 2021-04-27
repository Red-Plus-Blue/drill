using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;

using TMPro;

using Random = UnityEngine.Random;

public class MapComponent : MonoBehaviour
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
    protected GameObject _floatingTextPrefab;
    [SerializeField]
    protected GameObject _tutorialShopPrefab;

    [SerializeField]
    protected GameObject _exitPrefab;

    protected int _width;
    protected int _height;

    protected BlockComponent[,] _blocks;

    public bool IsOnMap(int x, int y) => (x >= 0) && (y >= 0) && (x < _blocks.GetLength(0) ) && (y < _blocks.GetLength(1));

    private void Start()
    {
        if (GameManagerComponent.Instance.IsTutorial)
        {
            _width = 15;
            _height = 40;
            GenerateTutorial();
        }
        else
        {
            _width = 25 + Random.Range((GameManagerComponent.Instance.Level * 2), (GameManagerComponent.Instance.Level * 4));
            _height = 25 + Random.Range((GameManagerComponent.Instance.Level * 2), (GameManagerComponent.Instance.Level * 4));
            GenerateMap();
        }
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
                        var prefab = isEdge ? _impassableBlockPrefab : _blockPrefab;
                        if(prefab != _impassableBlockPrefab)
                        {
                            prefab = (Random.Range(0, 10) < 1) ? _moneyBlocks[Random.Range(0, _moneyBlocks.Count)] : prefab;
                        }
                        if (prefab != _impassableBlockPrefab)
                        {
                            prefab = (Random.Range(0, 75) < 1) ? _fuelBlock : prefab;
                        }
                        var block = Instantiate(prefab, position, Quaternion.identity);
                        block.name = name;
                        _blocks[x, y] = block;
                    });
            });

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

    protected void GenerateTutorial()
    {
        ResizeBackground();

        void MakeFloatingText(Vector3 position, string message)
        {
            var text = Instantiate(_floatingTextPrefab, position, Quaternion.identity);
            text.GetComponentInChildren<TMP_Text>().text = message;
        }

       

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
                        var prefab = isEdge ? _impassableBlockPrefab : _blockPrefab;
                        var block = Instantiate(prefab, position, Quaternion.identity);
                        block.name = name;
                        _blocks[x, y] = block;
                    });
            });


        // Spawn Player
        var spawn = new Vector2Int(7, 5);
        DoForEachInRange(spawn, 2, (block) => Destroy(block.gameObject));

        var player = FindObjectOfType<PlayerControllerComponent>();
        player.transform.position = new Vector3(spawn.x, spawn.y, transform.position.z);
        var elevator = GameObject.FindGameObjectWithTag("Elevator");
        elevator.transform.position = new Vector3(spawn.x, spawn.y, transform.position.z);

        SelectBox(4, 8, 7, 4, (block) => Destroy(block.gameObject));

        MakeFloatingText(new Vector3(7f, 8.5f, transform.position.z), "You can move forward and backward\nwith W and S.\nYou can turn with A and D.");

        SelectBox(4, 14, 7, 4, (block) => Destroy(block.gameObject));
        SelectBox(3, 18, 9, 2, (block) =>
        {
            Instantiate(_fuelBlock, block.transform.position, Quaternion.identity);
            Destroy(block.gameObject);

        });

        MakeFloatingText(new Vector3(7f, 14f, transform.position.z), "You can dig through blocks\nby moving into them.");

        MakeFloatingText(new Vector3(7f, 17f, transform.position.z), "/\\ Fuel Blocks replenish Fuel /\\\nIf you run out of fuel, you lose.");

        SelectBox(4, 20, 7, 4, (block) => Destroy(block.gameObject));

        SelectBox(3, 24, 9, 2, (block) =>
        {
            Instantiate(_moneyBlocks[Random.Range(0, _moneyBlocks.Count)], block.transform.position, Quaternion.identity);
            Destroy(block.gameObject);

        });

        MakeFloatingText(new Vector3(7f, 23f, transform.position.z), "/\\ Gem blocks give you money /\\\nMoney is for score and to buy bonuses.");

        SelectBox(4, 26, 7, 4, (block) => Destroy(block.gameObject));

        MakeFloatingText(new Vector3(7f, 26f, transform.position.z), "This is a shop, you can buy things here.\nBuy passage to the next area.");

        Instantiate(_tutorialShopPrefab, new Vector3(7f, 27f, transform.position.z), Quaternion.identity);

        SelectBox(3, 30, 9, 1, (block) =>
        {
            var newBlock = Instantiate(_impassableBlockPrefab, block.transform.position, Quaternion.identity);
            _blocks[Mathf.RoundToInt(block.transform.position.x), Mathf.RoundToInt(block.transform.position.y)] = newBlock;
            Destroy(block.gameObject);
        });

        SelectBox(4, 32, 7, 4, (block) => Destroy(block.gameObject));

        MakeFloatingText(new Vector3(7f, 34f, transform.position.z), "This is the exit.\nYour goal is to reach this in each level.");

        DoForEachInRange(new Vector2Int(7, 38), 2, (block) =>
        {
            Destroy(block.gameObject);
            Instantiate(_exitPrefab, block.gameObject.transform.position, Quaternion.identity);
        });
    }

    protected void DoForEachInRange(Vector2Int center, int range, Action<BlockComponent> action)
    {
        for(var yOffset = -range; yOffset <= range; yOffset++)
        {
            for (var xOffset = -range; xOffset <= range; xOffset++)
            {
                var position = center + new Vector2Int(xOffset, yOffset);
                if(position.x == 0 || position.y == 0 || position.x == _width - 1 || position.y == _height - 1)
                {
                    continue;
                }

                if(!IsOnMap(position.x, position.y)) { continue; }
                action(_blocks[position.x, position.y]);
            }
        }
    }
}
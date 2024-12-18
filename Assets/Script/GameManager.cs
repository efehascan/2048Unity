using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _widht = 4;
    [SerializeField] private int _height = 4;
    [SerializeField] public Node nodePrefeb;
    [SerializeField] Block blockPrefeb;
    [SerializeField] public SpriteRenderer boardPrefeb;
    [SerializeField] public List<BlockType> _types;

    private List<Node> _nodes;
    private List<Block> _blocks;
    GameState _gameState;
    private int round;
    
    BlockType GetBlockTypeByValue(int value) => _types.First(t=>t.Value == value);

    private void Start()
    {
        ChangeState(GameState.GenerateLevel);
    }

    void ChangeState(GameState newState)
    {
        _gameState = newState;

        switch (newState) {
            case GameState.GenerateLevel:
                GenerateGrid();
                break;
            case GameState.SpawningBlocks:
                SpawnBlocks(round++ == 0 ? 2 : 1);
                break;
            case GameState.WaitingInput:
                break;
            case GameState.Moving:
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    void GenerateGrid()
    {
        round = 0;
        _nodes = new List<Node>();
        _blocks = new List<Block>();
        for (int x = 0; x < _widht; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var node = Instantiate(nodePrefeb, new Vector2(x,y), Quaternion.identity);
                _nodes.Add(node);
            }
        }
        var center = new Vector2((float) _widht /2 - 0.5f,(float) _height /2 - 0.5f);

        var board = Instantiate(boardPrefeb, center, Quaternion.identity);
        board.size = new Vector2(_widht, _height);

        Camera.main.transform.position = new Vector3(center.x, center.y, -10);
        
        ChangeState(GameState.SpawningBlocks);
    }

    void SpawnBlocks(int amount)
    {
        var freeNodes = _nodes.Where(n=>n.OccipiedBlock == null).OrderBy(b => Random.value).ToList();

        foreach (var node in freeNodes.Take(amount))
        {
            var block = Instantiate(blockPrefeb, node.Pos, Quaternion.identity);
            block.Init(GetBlockTypeByValue(Random.value > 0.8f ? 4 : 2));
            _blocks.Add(block);

        }

        if (freeNodes.Count() == 1)
        {
            //Oyunu kaybettin
            return;
        }
    }
}

[Serializable]

public struct BlockType
{
    public int Value;
    public Color Color;
}

public enum GameState
{
    GenerateLevel,
    SpawningBlocks,
    WaitingInput,
    Moving,
    Win,
    Lose
}

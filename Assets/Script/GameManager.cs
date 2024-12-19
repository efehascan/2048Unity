using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    [SerializeField] private int widht = 4;
    [SerializeField] private int height = 4;
    [SerializeField] private Node nodePrefeb;
    [SerializeField] private Block blockPrefeb;
    [SerializeField] private SpriteRenderer boardPrefeb;
    [SerializeField] private List<BlockType> _types;
    [SerializeField] private float switchNodeTime = 0.3f;
    [SerializeField] private float _travelTime = 0.2f;
    [SerializeField] private int _winCondition = 2048;
    [SerializeField] private GameObject _winScreen, _loseScreen;
    
    private List<Node> _nodes;
    private List<Block> _blocks;
    private GameState _gameState;
    private int round;

    private BlockType GetBlockTypeByValue(int value) => _types.First(t => t.Value == value);

    private void Start() {
        ChangeState(GameState.GenerateLevel);
    }

    private void ChangeState(GameState newState) {
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
                _winScreen.SetActive(true);
                break;
            case GameState.Lose:
                _loseScreen.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    private void Update() {
        if (_gameState != GameState.WaitingInput) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) Shift(Vector2.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) Shift(Vector2.right);
        if (Input.GetKeyDown(KeyCode.UpArrow)) Shift(Vector2.up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) Shift(Vector2.down);
    }

    private void GenerateGrid() {
        round = 0;
        _nodes = new List<Node>();
        _blocks = new List<Block>();
        for (int x = 0; x < widht; x++) {
            for (int y = 0; y < height; y++) {
                var node = Instantiate(nodePrefeb, new Vector2(x, y), Quaternion.identity);
                _nodes.Add(node);
            }
        }

        var center = new Vector2((float)widht / 2 - 0.5f, (float)height / 2 - 0.5f);

        var board = Instantiate(boardPrefeb, center, Quaternion.identity);
        board.size = new Vector2(widht, height);

        Camera.main.transform.position = new Vector3(center.x, center.y, -10);

        ChangeState(GameState.SpawningBlocks);
    }

    private void SpawnBlocks(int amount) {
        var freeNodes = _nodes.Where(n => n.OccipiedBlock == null).OrderBy(b => Random.value).ToList();

        foreach (var node in freeNodes.Take(amount)) {
            SpawnBlock(node, Random.value > 0.8f ? 4 : 2);
        }

        if (freeNodes.Count == 1) {
            ChangeState(GameState.Lose);
            return;
        }

        ChangeState(_blocks.Any(b => b.Value == _winCondition) ? GameState.Win : GameState.WaitingInput);
    }

    private void SpawnBlock(Node node, int value) {
        var block = Instantiate(blockPrefeb, node.Pos, Quaternion.identity);
        block.Init(GetBlockTypeByValue(value));
        block.SetBlock(node);
        _blocks.Add(block);
    }

    private void Shift(Vector2 direction) {
        ChangeState(GameState.Moving);

        var orderedBlocks = _blocks.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();
        if (direction == Vector2.right || direction == Vector2.up) orderedBlocks.Reverse();

        foreach (var block in orderedBlocks) {
            var next = block.Node;
            do {
                block.SetBlock(next);

                var possibleNode = GetNodeAtPosition(next.Pos + direction);
                if (possibleNode != null) {
                    if (possibleNode.OccipiedBlock != null && possibleNode.OccipiedBlock.CanMerge(block.Value)) {
                        block.MergeBlock(possibleNode.OccipiedBlock);
                    } else if (possibleNode.OccipiedBlock == null) next = possibleNode;
                }

            } while (next != block.Node);
        }

        var sequence = DOTween.Sequence();

        foreach (var block in orderedBlocks) {
            var movePoint = block.MergingBlock != null ? block.MergingBlock.Node.Pos : block.Node.Pos;

            sequence.Insert(0, block.transform.DOMove(movePoint, _travelTime).SetEase(Ease.InQuad));
        }

        sequence.OnComplete(() => {
            foreach (var block in orderedBlocks.Where(b => b.MergingBlock != null)) {
                MergeBlocks(block.MergingBlock, block);
            }
            ChangeState(GameState.SpawningBlocks);
        });
    }

    private void MergeBlocks(Block baseBlock, Block mergingBlock) {
        var newValue = baseBlock.Value * 2;
        SpawnBlock(baseBlock.Node, newValue);
        RemoveBlock(baseBlock);
        RemoveBlock(mergingBlock);
    }

    private void RemoveBlock(Block block) {
        _blocks.Remove(block);
        Destroy(block.gameObject);
    }

    private Node GetNodeAtPosition(Vector2 position) {
        return _nodes.FirstOrDefault(n => n.Pos == position);
    }
}

[Serializable]
public struct BlockType {
    public int Value;
    public Color Color;
}

public enum GameState {
    GenerateLevel,
    SpawningBlocks,
    WaitingInput,
    Moving,
    Win,
    Lose
}

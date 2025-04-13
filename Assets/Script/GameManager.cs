using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    [SerializeField] private int widht = 4;
    [SerializeField] private int height = 4;
    [SerializeField] private Node nodePrefeb;
    [SerializeField] private Block blockPrefeb;
    [SerializeField] private SpriteRenderer boardPrefeb;
    [SerializeField] private List<BlockType> types;
    [SerializeField] private float switchNodeTime = 0.3f;
    [SerializeField] private float travelTime = 0.2f; 
    [SerializeField] private int winCondition = 2048;
    
    private List<Node> nodes;
    private List<Block> blocks;
    private GameState gameState;
    private int round;

    private BlockType GetBlockTypeByValue(int value) => types.First(t => t.Value == value);

    private void Start() {
        ChangeState(GameState.GenerateLevel);
    }

    /// <summary>
    /// Oyun durumunu değiştirir ve yeni duruma göre ilgili işlemleri başlatır (seviye oluşturma, blok oluşturma, kazanma/kaybetme ekranları vb.).
    /// </summary>
    private void ChangeState(GameState newState) {
        gameState = newState;

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
                SceneManager.LoadScene(2);
                break;
            case GameState.Lose:
                SceneManager.LoadScene(1);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    private void Update() {
        if (gameState != GameState.WaitingInput) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) 
            Shift(Vector2.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) 
            Shift(Vector2.right);
        if (Input.GetKeyDown(KeyCode.UpArrow)) 
            Shift(Vector2.up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) 
            Shift(Vector2.down);
    }

    /// <summary>
    /// Oyun alanını oluşturur: düğümleri yerleştirir, tahtayı ve kamerayı konumlandırır, ardından blok oluşturma aşamasına geçer.
    /// </summary>
    private void GenerateGrid() {
        round = 0;
        nodes = new List<Node>();
        blocks = new List<Block>();
        for (int x = 0; x < widht; x++) {
            for (int y = 0; y < height; y++) {
                var node = Instantiate(nodePrefeb, new Vector2(x, y), Quaternion.identity);
                nodes.Add(node);
            }
        }

        var center = new Vector2((float)widht / 2 - 0.5f, (float)height / 2 - 0.5f);

        var board = Instantiate(boardPrefeb, center, Quaternion.identity);
        board.size = new Vector2(widht, height);

        Camera.main.transform.position = new Vector3(center.x, center.y, -10);

        ChangeState(GameState.SpawningBlocks);
    }

    /// <summary>
    /// Belirtilen sayıda rastgele boş düğüme yeni bloklar yerleştirir. 
    /// Blok değeri %80 ihtimalle 2, %20 ihtimalle 4 olur. 
    /// Oyun kaybedildiyse veya kazanıldıysa ilgili duruma geçer.
    /// </summary>
    private void SpawnBlocks(int amount) {
        var freeNodes = nodes.Where(n => n.OccipiedBlock == null).OrderBy(b => Random.value).ToList();

        foreach (var node in freeNodes.Take(amount)) {
            SpawnBlock(node, Random.value > 0.8f ? 4 : 2);
        }

        if (freeNodes.Count == 1) {
            ChangeState(GameState.Lose);
            return;
        }

        ChangeState(blocks.Any(b => b.Value == winCondition) ? GameState.Win : GameState.WaitingInput);
    }

    /// <summary>
    /// Belirtilen düğüm konumuna verilen değere sahip yeni bir blok oluşturur, başlatır ve blok listesine ekler.
    /// </summary>
    private void SpawnBlock(Node node, int value) {
        var block = Instantiate(blockPrefeb, node.Pos, Quaternion.identity);
        block.Init(GetBlockTypeByValue(value));
        block.SetBlock(node);
        blocks.Add(block);
    }

    
    /// <summary>
    /// Belirtilen yönde blokları kaydırır, birleştirilebilecek olanları işaretler, hareket animasyonlarını başlatır 
    /// ve hareket tamamlandıktan sonra yeni blokların yerleştirilmesini başlatır.
    /// </summary>
    private void Shift(Vector2 direction) {
        ChangeState(GameState.Moving);

        var orderedBlocks = blocks.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();
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

            sequence.Insert(0, block.transform.DOMove(movePoint, travelTime).SetEase(Ease.InQuad));
        }

        sequence.OnComplete(() => {
            foreach (var block in orderedBlocks.Where(b => b.MergingBlock != null)) {
                MergeBlocks(block.MergingBlock, block);
            }
            ChangeState(GameState.SpawningBlocks);
        });
    }

    /// <summary>
    /// İki bloğu birleştirir, yeni değere sahip bir blok oluşturur ve eski blokları sahneden kaldırır.
    /// </summary>
    private void MergeBlocks(Block baseBlock, Block mergingBlock) {
        var newValue = baseBlock.Value * 2;
        SpawnBlock(baseBlock.Node, newValue);
        RemoveBlock(baseBlock);
        RemoveBlock(mergingBlock);
    }

    /// <summary>
    /// Verilen bloğu oyun alanından ve blok listesinden kaldırır.
    /// </summary>
    private void RemoveBlock(Block block) {
        blocks.Remove(block);
        Destroy(block.gameObject);
    }

    /// <summary>
    /// Belirtilen pozisyondaki düğümü (_nodes listesinden) bulur ve döndürür. Düğüm yoksa null döner.
    /// </summary>
    private Node GetNodeAtPosition(Vector2 position) {
        return nodes.FirstOrDefault(n => n.Pos == position);
    }
}

[Serializable]
public struct BlockType {
    public int Value;
    public Color Color;
}

/// <summary>
/// Blok tabanlı bir oyunun durumlarını temsil eder: Seviye oluşturma, blokların yerleştirilmesi, oyuncu girişi bekleme, hareket etme, kazanma ve kaybetme durumları.
/// </summary>
public enum GameState {
    GenerateLevel,
    SpawningBlocks,
    WaitingInput,
    Moving,
    Win,
    Lose
}

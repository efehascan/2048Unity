using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Block : MonoBehaviour
{
    public int Value;
    public Node Node;
    public Block MergingBlock;
    bool Merged = false;
    public Vector2 Pos => transform.position;
    [SerializeField] SpriteRenderer _renderer;
    [SerializeField] private TextMeshPro _text;
    

    public void Init(BlockType type)
    {
        Value = type.Value;
        _renderer.color = type.Color;
        _text.text = type.Value.ToString();
        
    }

    public void SetBlock(Node node)
    {
        if(Node != null) Node.OccipiedBlock = null;
        Node = node;
        Node.OccipiedBlock = this;
    }

    public void MergeBlock(Block blockToMergewith)
    {
        MergingBlock = blockToMergewith;
        
        Node.OccipiedBlock = null;
        
        blockToMergewith.Merged = true;
    }

    public bool CanMerge(int value) => value == Value && !Merged && MergingBlock == null;
}

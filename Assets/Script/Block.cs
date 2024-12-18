using System;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    int value;
    public Node Node;
    public Vector2 Pos => transform.position;
    [SerializeField] SpriteRenderer _renderer;
    [SerializeField] private TextMeshPro _text;
    

    public void Init(BlockType type)
    {
        value = type.Value;
        _renderer.color = type.Color;
        _text.text = type.Value.ToString();
        
    }

    public void SetBlock(Node node)
    {
        if(Node != null) Node.OccipiedBlock = null;
        Node = node;
        Node.OccipiedBlock = this;
    }

}

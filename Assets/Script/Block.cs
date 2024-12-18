using System;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    int value;
    [SerializeField] SpriteRenderer _renderer;
    [SerializeField] private TextMeshPro _text;
    

    public void Init(BlockType type)
    {
        value = type.Value;
        _renderer.color = type.Color;
        _text.text = type.Value.ToString();
        
    }

}

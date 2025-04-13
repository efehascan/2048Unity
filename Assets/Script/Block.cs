using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Block : MonoBehaviour
{
    public int Value;
    public Node Node;
    public Block MergingBlock;
    public bool Merged = false;
    public Vector2 Pos => transform.position;
    [SerializeField] SpriteRenderer _renderer;
    [SerializeField] private TextMeshPro _text;
    

    /// <summary>
    /// Bloğu belirtilen tür ile başlatır: değerini, rengini ve metin içeriğini ayarlar.
    /// </summary>
    public void Init(BlockType type)
    {
        Value = type.Value;
        _renderer.color = type.Color;
        _text.text = type.Value.ToString();
        
    }

    /// <summary>
    /// Bloğu belirtilen düğüme yerleştirir ve önceki düğüm ile olan bağlantıyı kaldırarak yeni düğümü işaretler.
    /// </summary>
    public void SetBlock(Node node)
    {
        if(Node != null) Node.OccipiedBlock = null;
        Node = node;
        Node.OccipiedBlock = this;
    }

    /// <summary>
    /// Belirtilen blok ile birleştirme işlemini başlatır; mevcut düğüm bağlantısını kaldırır ve diğer bloğu birleşmiş olarak işaretler.
    /// </summary>
    public void MergeBlock(Block blockToMergewith)
    {
        MergingBlock = blockToMergewith;
        
        Node.OccipiedBlock = null;
        
        blockToMergewith.Merged = true;
    }

    public bool CanMerge(int value) => value == Value && !Merged && MergingBlock == null;
}

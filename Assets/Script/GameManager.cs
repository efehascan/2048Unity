using Unity.Mathematics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _widht = 4;
    [SerializeField] private int _height = 4;
    [SerializeField] public Node nodePrefeb;
    [SerializeField] public SpriteRenderer boardPrefeb;

    private void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < _widht; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var node = Instantiate(nodePrefeb, new Vector2(x,y), Quaternion.identity);
            }
        }
        var center = new Vector2((float) _widht /2 - 0.5f,(float) _height /2 - 0.5f);

        var board = Instantiate(boardPrefeb, Vector3.zero, Quaternion.identity);

    }
}

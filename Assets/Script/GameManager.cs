using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _widht = 4;
    [SerializeField] private int _height = 4;
    [SerializeField] public Node _nodePrefeb;

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
                var node = Instantiate(_nodePrefeb, new Vector2(x,y), Quaternion.identity);
            }
        }
    }
}

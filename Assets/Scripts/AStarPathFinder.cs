using UnityEngine;

public class AStarPathFinder : MonoBehaviour
{
    public GameObject tilePrefab;
    private GameObject[,] tileObjects;
    GridBase currentGrid;
    Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    private int width = 20;
    private int height = 20;

    private void Start()
    {
        CreateGrid(width, height);

        for (int i = 0; i < 5; i++)
        {
            AddObstacle(5, i);
        }

        CalculatePath(0, 0, 19, 0);
    }

    public void CreateGrid(int width, int height)
    {
        currentGrid = new GridBase(width, height);
        tileObjects = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x, 0, y);
                GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity);
                tile.GetComponent<Renderer>().material.color = Color.green;
                tileObjects[x, y] = tile;
            }
        }
    }

    public void AddObstacle(int x, int y)
    {
        currentGrid.SetObstacle(x, y);
        tileObjects[x, y].GetComponent<Renderer>().material.color = Color.red;
    }

    public void CalculatePath(int startX, int startY, int endX, int endY)
    {
        tileObjects[startX, startY].GetComponent<Renderer>().material.color = Color.yellow;
        tileObjects[endX, endY].GetComponent<Renderer>().material.color = Color.blue;


    }
}
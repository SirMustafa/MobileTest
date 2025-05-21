using UnityEngine;

public class GridBase 
{
    private int width;
    private int height;
    private int[,] gridArray;

    public GridBase(int width, int height)
    {
        this.width = width;
        this.height = height;
        gridArray = new int[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                gridArray[x, y] = 0;
    }

    public void SetObstacle(int x, int y)
    {
        gridArray[x, y] = 1;
    }

    public bool IsWalkable(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return false;

        return gridArray[x, y] == 0;
    }
}
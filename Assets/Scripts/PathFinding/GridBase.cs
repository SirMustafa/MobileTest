public class GridBase
{
    public int width;
    public int height;
    private Node[,] nodes;

    public GridBase(int width, int height)
    {
        this.width = width;
        this.height = height;
        nodes = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                nodes[x, y] = new Node(x, y, true);
            }
        }
    }

    public void SetObstacle(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
            nodes[x, y].isWalkable = false;
    }
    public void RemoveObstacle(int x, int y)
    {
        nodes[x, y].isWalkable = true;
    }

    public Node GetNode(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
            return nodes[x, y];
        return null;
    }
}
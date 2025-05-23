using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinder : MonoBehaviour
{
    public static AStarPathFinder Instance;
    [SerializeField] bool iWannaSee;
    GridBase currentGrid;
    Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    HashSet<Node> openSet = new HashSet<Node>();
    HashSet<Node> closedSet = new HashSet<Node>();

    private void Awake()
    {
        Instance = this;
    }

    public void CreateGrid(int width, int height)
    {
        currentGrid = new GridBase(width, height);
    }

    public void AddObstacle(int x, int y)
    {
        currentGrid.SetObstacle(x, y);
    }

    public void RemoveObstacle(int x, int y)
    {
        currentGrid.RemoveObstacle(x, y);
    }

    public List<Vector2Int> CalculatePath(Vector2Int startPos, Vector2Int endPos)
    {
        openSet.Clear();
        closedSet.Clear();

        Node start = currentGrid.GetNode(startPos.x, startPos.y);

        start.gCost = 0;
        start.hCost = CalculateManhattanDistance(startPos, endPos);
        start.parent = null;
        openSet.Add(start);

        Node closestNode = start;
        int closestDistance = start.hCost;

        while (openSet.Count > 0)
        {
            Node currentNode = GetLowestFCostNode(openSet);

            if (currentNode.getPos() == endPos)
            {
                return BuildPath(currentNode);
            }

            Vector2Int currentPos = currentNode.getPos();

            foreach (Vector2Int dir in directions)
            {
                Vector2Int newPos = currentPos + dir;
                Node neighbor = currentGrid.GetNode(newPos.x, newPos.y);

                if (neighbor != null && neighbor.isWalkable && !closedSet.Contains(neighbor))
                {
                    int tentativeGCost = currentNode.gCost + 1;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                        neighbor.gCost = tentativeGCost;
                        neighbor.hCost = CalculateManhattanDistance(newPos, endPos);
                        neighbor.parent = currentNode;
                    }
                    else if (tentativeGCost < neighbor.gCost)
                    {
                        neighbor.gCost = tentativeGCost;
                        neighbor.parent = currentNode;
                    }

                    int distToEnd = neighbor.hCost;
                    if (distToEnd < closestDistance)
                    {
                        closestDistance = distToEnd;
                        closestNode = neighbor;
                    }
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
        }

        if (closestNode != null && closestNode != start)
        {
            return BuildPath(closestNode);
        }

        return null;
    }

    private Node GetLowestFCostNode(HashSet<Node> nodes)
    {
        Node lowest = null;
        int lowestFCost = int.MaxValue;

        foreach (var node in nodes)
        {
            if (node.fCost < lowestFCost)
            {
                lowestFCost = node.fCost;
                lowest = node;
            }
        }

        return lowest;
    }

    private int CalculateManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private List<Vector2Int> BuildPath(Node endNode)
    {
        var tmp = new List<Vector2Int>();
        for (Node n = endNode; n != null; n = n.parent)
        {
            tmp.Add(n.getPos());
        }
        tmp.Reverse();
        return tmp;
    }
    private void OnDrawGizmos()
    {
        if (!iWannaSee || currentGrid == null) return;

        int width = currentGrid.width;
        int height = currentGrid.height;

        Vector3 cellSize = new Vector3(1, 0, 1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node node = currentGrid.GetNode(x, y);
                if (node == null) continue;

                Vector3 center = new Vector3(x, 0, y);
                Gizmos.color = node.isWalkable ? Color.cyan : Color.red;
                Gizmos.DrawWireCube(center, cellSize);
            }
        }
    }
}
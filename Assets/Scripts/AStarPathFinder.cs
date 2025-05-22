using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinder : MonoBehaviour
{
    public static AStarPathFinder Instance;

    GridBase currentGrid;
    Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int( 1,  1),
        new Vector2Int( 1, -1),
        new Vector2Int(-1,  1),
        new Vector2Int(-1, -1),
    };

    HashSet<Node> openSet = new HashSet<Node>();
    HashSet<Node> closedSet = new HashSet<Node>();
    private List<Vector2Int> path;

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

    public void CalculatePath(Vector2Int startPos, Vector2Int endPos)
    {
        openSet.Clear();
        closedSet.Clear();

        Node start = currentGrid.GetNode(startPos.x, startPos.y);
        start.gCost = 0;
        start.hCost = CalculateManhattanDistance(startPos, endPos);
        start.parent = null;
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            int lowestFCost = int.MaxValue;
            Node lowestFcostNode = null;

            foreach (Node currentNode in openSet)
            {
                if (currentNode.fCost < lowestFCost)
                {
                    lowestFCost = currentNode.fCost;
                    lowestFcostNode = currentNode;
                }
            }

            if (lowestFcostNode.getPos() == endPos)
            {
                Debug.Log("istenilen yere ulaþýldý");
                path = BuildPath(lowestFcostNode);
                return;
            }

            Vector2Int currentPos = new Vector2Int(lowestFcostNode.x, lowestFcostNode.y);

            foreach (Vector2Int dir in directions)
            {
                Vector2Int newPos = currentPos + dir;
                if (currentGrid.GetNode(newPos.x, newPos.y) != null &&
                    currentGrid.GetNode(newPos.x, newPos.y).isWalkable &&
                    !closedSet.Contains(currentGrid.GetNode(newPos.x, newPos.y)))
                {
                    Node nodeToAddList = currentGrid.GetNode(newPos.x, newPos.y);
                    if (!openSet.Contains(nodeToAddList))
                    {
                        openSet.Add(nodeToAddList);
                        nodeToAddList.gCost = lowestFcostNode.gCost + 1;
                        nodeToAddList.hCost = CalculateManhattanDistance(endPos, nodeToAddList.getPos());
                        nodeToAddList.parent = lowestFcostNode;
                    }
                    else
                    {
                        int newGCost = lowestFcostNode.gCost + 1;
                        if (newGCost < nodeToAddList.gCost)
                        {
                            nodeToAddList.gCost = newGCost;
                            nodeToAddList.parent = lowestFcostNode;
                        }
                    }
                }
            }

            closedSet.Add(lowestFcostNode);
            openSet.Remove(lowestFcostNode);
        }
    }

    private int CalculateManhattanDistance(Vector2Int nodesPos, Vector2Int desiredPos)
    {
        return Mathf.Abs(nodesPos.x - desiredPos.x) + Mathf.Abs(nodesPos.y - desiredPos.y);
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
        if (currentGrid == null) return;

        float size = 1f;
        Vector3 offset = Vector3.zero;

        for (int x = 0; x < currentGrid.width; x++)
        {
            for (int y = 0; y < currentGrid.height; y++)
            {
                Vector3 cellCenter = new Vector3(x + 0.5f, 0f, y + 0.5f) + offset;

                if (path != null && path.Contains(new Vector2Int(x, y)))
                {
                    Gizmos.color = Color.green;
                }
                else if (!currentGrid.GetNode(x, y).isWalkable)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.white;
                }

                Gizmos.DrawWireCube(cellCenter, new Vector3(size, 0f, size));
                Gizmos.DrawCube(cellCenter, new Vector3(size * 0.95f, 0f, size * 0.95f));
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

public class LPathFinder : MonoBehaviour
{
    public static LPathFinder Instance;
    [SerializeField] bool iwantoSee;

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

    private Vector2Int lastTarget;
    private bool hasLast = false;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        var maybePos = GetMouseGridPosition();
        if (!maybePos.HasValue) return;

        Vector2Int target = maybePos.Value;

        if (!hasLast || target != lastTarget)
        {
            lastTarget = target;
            hasLast = true;

            CalculatePath(new Vector2Int(0, 0), target);
        }
    }
    private Vector2Int? GetMouseGridPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (new Plane(Vector3.up, Vector3.zero).Raycast(ray, out float enter))
        {
            Vector3 worldPoint = ray.GetPoint(enter);
            int gx = Mathf.FloorToInt(worldPoint.x);
            int gy = Mathf.FloorToInt(worldPoint.z);
            if (gx >= 0 && gx < currentGrid.width && gy >= 0 && gy < currentGrid.height)
                return new Vector2Int(gx, gy);
        }
        return null;
    }

    public void CreateGrid(int width, int height)
    {
        currentGrid = new GridBase(width, height);
        AddObstacle(7, 0);
        AddObstacle(7, 1);
        AddObstacle(7, 2);
        AddObstacle(7, 3);
        AddObstacle(7, 4);
        AddObstacle(7, 5);

        AddObstacle(13, 14);
        AddObstacle(13, 13);
        AddObstacle(13, 12);
        AddObstacle(13, 11);
        AddObstacle(13, 10);
        AddObstacle(13, 9);
        AddObstacle(13, 8);
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
                path = BuildPath(lowestFcostNode);
                return;
            }

            Vector2Int currentPos = new Vector2Int(lowestFcostNode.x, lowestFcostNode.y);

            for (int i = 0; i < directions.Length; i++)
            {
                Vector2Int dir = directions[i];
                Vector2Int newPos = currentPos + dir;

                if (currentGrid.GetNode(newPos.x, newPos.y) != null &&
                    currentGrid.GetNode(newPos.x, newPos.y).isWalkable &&
                    !closedSet.Contains(currentGrid.GetNode(newPos.x, newPos.y)))
                {
                    Node nodeToAddList = currentGrid.GetNode(newPos.x, newPos.y);

                    int movementCost = IsDiagonalMove(dir) ? 14 : 10;
                    int newGCost = lowestFcostNode.gCost + movementCost;

                    if (!openSet.Contains(nodeToAddList))
                    {
                        openSet.Add(nodeToAddList);
                        nodeToAddList.gCost = newGCost;
                        nodeToAddList.hCost = CalculateManhattanDistance(endPos, nodeToAddList.getPos());
                        nodeToAddList.parent = lowestFcostNode;
                    }
                    else
                    {
                        if (newGCost < nodeToAddList.gCost)
                        {
                            nodeToAddList.gCost = newGCost;
                            nodeToAddList.parent = lowestFcostNode;
                        }
                    }
                }

                closedSet.Add(lowestFcostNode);
                openSet.Remove(lowestFcostNode);
            }
        }
    }

    private bool IsDiagonalMove(Vector2Int direction)
    {
        return direction.x != 0 && direction.y != 0;
    }

    private int CalculateManhattanDistance(Vector2Int nodesPos, Vector2Int desiredPos)
    {
        int dx = Mathf.Abs(nodesPos.x - desiredPos.x);
        int dy = Mathf.Abs(nodesPos.y - desiredPos.y);

        return 10 * (dx + dy) + (14 - 2 * 10) * Mathf.Min(dx, dy);

        //return Mathf.Abs(nodesPos.x - desiredPos.x) + Mathf.Abs(nodesPos.y - desiredPos.y);
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
        if (currentGrid == null && !iwantoSee) return;

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
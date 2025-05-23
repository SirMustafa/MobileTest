using UnityEngine;

public class Node
{
    public int x, y;
    public int gCost, hCost;
    public int fCost => gCost + hCost;
    public bool isWalkable;
    public Node parent;

    public Node(int x, int y, bool isWalkable)
    {
        this.x = x;
        this.y = y;
        this.isWalkable = isWalkable;
    }

    public Vector2Int getPos()
    {
        return new Vector2Int(x, y);
    }
}
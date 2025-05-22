using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPlayer : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;

    private List<Vector2Int> currentPath;
    private int currentWaypoint = 0;
    private bool isMoving = false;

    public void MoveTo(int targetX, int targetY)
    {
        if (isMoving)
        {
            StopAllCoroutines();
        }

       // Vector2Int gridPos = AStarPathFinder.Instance.WorldToGrid(transform.position);
       // List<Vector2Int> path = AStarPathFinder.Instance.GetPathAsPositions(gridPos.x, gridPos.y, targetX, targetY);

       // currentPath = path;
        currentWaypoint = 0;
        StartCoroutine(FollowPath());
    }

    private IEnumerator FollowPath()
    {
        isMoving = true;

        while (currentWaypoint < currentPath.Count)
        {
           // Vector2Int targetGridPos = currentPath[currentWaypoint];
           //// Vector3 targetWorldPos = AStarPathFinder.Instance.GridToWorld(targetGridPos.x, targetGridPos.y);
           // targetWorldPos.y = transform.position.y;
           //
           // while (Vector3.Distance(transform.position, targetWorldPos) > 0.1f)
           // {
           //     transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, moveSpeed * Time.deltaTime);
               yield return null;
           // }
           // currentWaypoint++;
        }

        isMoving = false;
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}
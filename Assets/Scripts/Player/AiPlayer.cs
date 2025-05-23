using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPlayer : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float sleepTime;

    private List<Vector2Int> currentPath;
    private int currentWaypoint = 0;
    private bool isMoving = false;
    public Vector2Int worldBorders { get; private set; }

    private void Start()
    {
        worldBorders = GameManager.Intance.WorldPos();
        StartCoroutine(WaitAndMove());
    }

    IEnumerator WaitAndMove()
    {
        yield return new WaitForSeconds(sleepTime);
        MoveTo();
    }

    public void MoveTo()
    {
        if (isMoving) return;

        Vector2Int target = new Vector2Int(Random.Range(0, worldBorders.x), Random.Range(0, worldBorders.y));
        Vector2Int currentPos = new Vector2Int((int)transform.position.x, (int)transform.position.z);

        currentPath = AStarPathFinder.Instance.CalculatePath(currentPos, target);

        if (currentPath == null || currentPath.Count <= 1)
        {
            isMoving = false;
            return;
        }

        currentWaypoint = 1;
        isMoving = true;
        MoveStep();
    }

    private void MoveStep()
    {
        if (currentWaypoint >= currentPath.Count)
        {
            isMoving = false;
            StartCoroutine(WaitAndMove());
            return;
        }

        Vector2Int nextGridPos = currentPath[currentWaypoint];
        Vector3 nextWorldPos = new Vector3(nextGridPos.x, 0f, nextGridPos.y);

        transform.DOMove(nextWorldPos, 1f / moveSpeed).SetEase(Ease.Linear)
            .OnComplete(() => { worldBorders = nextGridPos; currentWaypoint++; MoveStep(); });
    }
}
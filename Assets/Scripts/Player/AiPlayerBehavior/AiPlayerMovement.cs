using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPlayerMovement : AiPlayerBase
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float sleepTime = 1f;

    private List<Vector2Int> currentPath;
    private int currentWaypoint = 0;
    Vector2Int worldBorders;

    private void Start()
    {
        worldBorders = GameManager.Intance.WorldPos();
        MoveTo();
    }

    void MoveTo()
    {
        Vector2Int randomTarget = new Vector2Int(Random.Range(0, worldBorders.x), Random.Range(0, worldBorders.y));
        currentPath = AStarPathFinder.Instance.CalculatePath(new Vector2Int((int)transform.position.x, (int)transform.position.z), randomTarget);

        if (currentPath != null && currentPath.Count > 0)
        {
            currentWaypoint = 0;
            StartCoroutine(FollowPath());
        }
        else
        {
            Invoke(nameof(MoveTo), 1f);
        }
    }

    private IEnumerator FollowPath()
    {
        yield return new WaitForSeconds(sleepTime);

        while (currentWaypoint < currentPath.Count)
        {
            Vector3 targetPos = new Vector3(currentPath[currentWaypoint].x, transform.position.y, currentPath[currentWaypoint].y);

            Vector3 direction = (targetPos - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                float yRotation = Quaternion.LookRotation(direction).eulerAngles.y;
                transform.DORotate(new Vector3(0, yRotation, 0), 0.3f);
            }

            float distance = Vector3.Distance(transform.position, targetPos);
            float duration = distance / moveSpeed;

            Tween moveTween = transform.DOMove(targetPos, duration).SetEase(Ease.Linear);
            yield return moveTween.WaitForCompletion();

            currentWaypoint++;
        }

        MoveTo();
    }

    public override void OnEnterState(AiPlayerManager AiPlayer)
    {
        
    }

    public override void OnUpdtaeState(AiPlayerManager AiPlayer)
    {
        
    }

    public override void OnColliderState(AiPlayerManager AiPlayer)
    {
        
    }
}
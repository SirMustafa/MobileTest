using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    public int range = 5;
    public float delayBeforeExplode = 5f;

    public LayerMask blowableLayer;
    public float tileSize = 1f;

    private static readonly Vector3[] directions = new Vector3[]
    {
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };

    private void OnEnable()
    {
        transform.DOLocalMoveY(1, 1).SetLoops(-1, LoopType.Yoyo);
        ExplodeAfterDelay().Forget();
    }

    private async UniTaskVoid ExplodeAfterDelay()
    {
        await UniTask.Delay((int)(delayBeforeExplode * 1000));
        ExplodeGrid();
        transform.DOKill();
        Destroy(gameObject);
    }

    private void ExplodeGrid()
    {
        Vector3 origin = transform.position;

        foreach (var dir in directions)
        {
            TryExplodeDirection(origin, dir);
        }
    }

    private void TryExplodeDirection(Vector3 origin, Vector3 dir)
    {
        for (int step = 1; step <= range; step++)
        {
            Vector3 checkPos = origin + dir * step * tileSize;
            Ray ray = new Ray(origin, dir);
            float distance = step * tileSize;
            Debug.DrawRay(origin, dir * distance, Color.yellow, 2f);

            if (Physics.Raycast(ray, out RaycastHit hit, distance))
            {
                GameObject hitObject = hit.collider.gameObject;

                if ((blowableLayer.value & (1 << hitObject.layer)) != 0)
                {
                    Destroy(hitObject);
                }

                break;
            }
        }
    }
}
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    public int range = 5;
    public LayerMask blowableLayer;
    public float tileSize = 1f;

    private void OnEnable()
    {
        //this.transform.DOLocalMoveY(1, 1).SetLoops(-1, LoopType.Yoyo);
        ExplodeGrid();
    }

    private void ExplodeGrid()
    {
        Vector3 origin = transform.position;
        TryExplodeDirection(Vector3.right);
        TryExplodeDirection(Vector3.left);
        TryExplodeDirection(Vector3.forward);
        TryExplodeDirection(Vector3.back);
        //transform.DOKill();
    }

    private void TryExplodeDirection(Vector3 dir)
    {
        Vector3 origin = transform.position;

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
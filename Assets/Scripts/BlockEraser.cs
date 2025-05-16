using System.Collections.Generic;
using UnityEngine;

public class BlockEraser : MonoBehaviour
{
    private List<Vector3> childPositions = new();

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            childPositions.Add(transform.GetChild(i).position);
        }
    }

    private void OnEnable()
    {
        foreach (var pos in childPositions)
        {
            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 1f))
            {
                Destroy(hit.collider.gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (var pos in childPositions)
        {
            Gizmos.DrawLine(pos, pos + Vector3.down * 1f);
        }
    }
}
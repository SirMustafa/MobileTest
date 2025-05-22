using UnityEngine;

public class PlacedObjects : MonoBehaviour
{
    private void OnEnable()
    {
        AStarPathFinder.Instance.AddObstacle((int)this.transform.position.x, (int)this.transform.position.z);
    }
}
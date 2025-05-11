using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] GameObject platePrefab;
    [SerializeField] int XgridSize;
    [SerializeField] int YgridSize;

    void Start()
    {
        for (int i = 0; i < XgridSize; i++)
        {
            for (int j = 0; j < YgridSize; j++)
            {
                Vector3 position = new Vector3(i * (1f + 0.1f), 0f, j * (1f + 0.1f));
                GameObject newPlate = Instantiate(platePrefab, this.transform);
                newPlate.transform.localPosition = position;
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData")]
public class LvlData : ScriptableObject
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    public GameObject cornerPrefab;
    public List<GameObject> objects = new List<GameObject>();
    public List<Vector2> positions;
}
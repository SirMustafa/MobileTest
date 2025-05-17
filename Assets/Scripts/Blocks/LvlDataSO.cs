using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData")]
public class LvlDataSO : ScriptableObject
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    public GameObject cornerPrefab;
    public List<Blocks> placedBlocksData;
}
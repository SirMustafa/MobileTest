using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "EditorSettings")]
public class EditorSO : ScriptableObject
{
    public VisualTreeAsset uiAsset;
    public GameObject tilePrefab;
    public GameObject cornerPrefab;
    public GameObject parentObj;
    public LvlData levelData;
    public GameObject[] slotPrefabs = new GameObject[5];
}
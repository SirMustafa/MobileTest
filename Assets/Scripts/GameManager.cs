using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] bool isTesting;
    [SerializeField] GameDataSO gameData;
    [SerializeField] List<LvlDataSO> levelDataList = new List<LvlDataSO>();
    [SerializeField] List<GameObject> PlayerList = new List<GameObject>();
    LvlDataSO currentLvl;
    private const float refWidth = 15f;
    private const float refHeight = 17f;
    float widthSlider;
    float heightSlider;

    private static readonly Vector2[] refPositions = new Vector2[]
    {
        new Vector2(4f, 5f),
        new Vector2(10f, 5f),
        new Vector2(10f, 11f),
        new Vector2(4f, 11f)
    };

    void Start()
    {
        Application.targetFrameRate = 60;
        widthSlider = currentLvl.width;
        heightSlider = currentLvl.height;
        if (isTesting) return;
        LoadLevelData();
        LoadPlayers();
    }

    public List<Vector2> GetResponsivePositions()
    {
        var result = new List<Vector2>(refPositions.Length);

        foreach (var p in refPositions)
        {
            float nx = p.x / refWidth;
            float ny = p.y / refHeight;

            float scaledX = nx * widthSlider;
            float scaledY = ny * heightSlider;

            result.Add(new Vector2(scaledX, scaledY));
        }

        return result;
    }

    private void LoadLevelData()
    {
        currentLvl = levelDataList[Random.Range(0, levelDataList.Count)];

        foreach (var block in currentLvl.placedBlocksData)
        {
            Vector3 position = new Vector3(block.index.x, 0f, block.index.y);
            Instantiate(block.slotObject, position, Quaternion.identity);
        }
    }

    private void LoadPlayers()
    {
        
    }
}
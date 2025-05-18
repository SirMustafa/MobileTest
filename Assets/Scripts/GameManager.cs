using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] bool isTesting;
    [SerializeField] List<LvlDataSO> levelDataList = new List<LvlDataSO>();
    [SerializeField] List<GameObject> PlayerList = new List<GameObject>();
    bool isOnePlayer = true;

    void Start()
    {
        Application.targetFrameRate = 60;

        if (isTesting) return;
        LoadLevelData();
        LoadPlayers();
    }

    private void LoadLevelData()
    {
        LvlDataSO currentLvl = levelDataList[Random.Range(0, levelDataList.Count)];

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
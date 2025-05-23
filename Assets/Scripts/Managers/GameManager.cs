using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Intance;
    [Header("Settings")]
    [SerializeField] private bool isTesting;
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private List<LvlDataSO> levelDataList = new List<LvlDataSO>();
    [SerializeField] private List<GameObject> playerPrefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> aiPrefabs = new List<GameObject>();
    private LvlDataSO currentLevel;

    private static readonly Vector2[] referencePositions = new Vector2[]
    {
        new Vector2(5f, 4f),
        new Vector2(11f, 4f),
        new Vector2(11f, 10f),
        new Vector2(5f, 10f)
    };

    private void Awake()
    {
        Intance = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        if (isTesting) return;

        currentLevel = levelDataList[Random.Range(0, levelDataList.Count)];
        AStarPathFinder.Instance.CreateGrid(currentLevel.width - 1, currentLevel.height - 1);

        DrawBaseGrid(currentLevel.width, currentLevel.height);
        LoadLevelData();
        LoadPlayers();
    }

    public Vector2Int WorldPos()
    {
        return new Vector2Int(currentLevel.width, currentLevel.height);
    }

    private void LoadLevelData()
    {
        foreach (var block in currentLevel.placedBlocksData)
        {
            Vector3 position = new Vector3(block.index.x, 0f, block.index.y);
            Instantiate(block.slotObject, position, Quaternion.identity, this.transform);
            AStarPathFinder.Instance.AddObstacle((int)block.index.x, (int)block.index.y);
        }
    }

    private void LoadPlayers()
    {
        List<Vector2> spawnPositions = new List<Vector2>(referencePositions);

        int index1 = gameData.playerInex1;
        int index2 = gameData.playerInex2;

        for (int i = 0; i < 4; i++)
        {
            Vector3 spawnPos = new Vector3(spawnPositions[i].x, 0f, spawnPositions[i].y);

            if (i == index1)
            {
                GameObject playerGO = Instantiate(playerPrefabs[index1], spawnPos, Quaternion.identity);
                playerGO.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player1");
            }
            else if (i == index2)
            {
                GameObject playerGO = Instantiate(playerPrefabs[index2], spawnPos, Quaternion.identity);
                playerGO.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
            }
            else
            {
                Instantiate(aiPrefabs[i], spawnPos, Quaternion.identity);
            }
        }
    }

    private void DrawBaseGrid(int width, int height)
    {
        GameObject tilePrefab = currentLevel.tilePrefab;
        GameObject cornerPrefab = currentLevel.cornerPrefab;

        GameObject obj = Instantiate(tilePrefab, new Vector3(0, 0f, 0), Quaternion.identity, this.transform);
        obj.transform.localScale = new Vector3(currentLevel.width, 1, currentLevel.height);
        obj.transform.position = new Vector3(currentLevel.width / 2, 0, currentLevel.height / 2);

        for (int x = 0; x < width; x++)
        {
            Instantiate(cornerPrefab, new Vector3(x, 0f, 0), Quaternion.identity, this.transform);
            Instantiate(cornerPrefab, new Vector3(x, 0f, height - 1), Quaternion.identity, this.transform);
        }

        for (int y = 1; y < height - 1; y++)
        {
            Instantiate(cornerPrefab, new Vector3(0, 0f, y), Quaternion.identity, this.transform);
            Instantiate(cornerPrefab, new Vector3(width - 1, 0f, y), Quaternion.identity, this.transform);
        }
    }
}
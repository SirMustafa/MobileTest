using UnityEditor;
using UnityEngine;

public class LvlEditorWindow : EditorWindow
{
    private GameObject groundPrefab;
    private GameObject placeablePrefab;
    private GameObject gridParent;
    private int gridWidth = 5;
    private int gridHeight = 5;
    private bool isPreviewing = false;
    private string folderPath = "Assets/Scripts/SO";

    [MenuItem("Tools/Tilemap Level Editor")]
    public static void ShowWindow()
    {
        LvlEditorWindow window = GetWindow<LvlEditorWindow>();
        window.titleContent = new GUIContent("Tilemap Editor");
        window.Show();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        isPreviewing = false;
        SceneView.RepaintAll();
    }

    private void OnGUI()
    {
        GUILayout.Label("Tilemap Editor", EditorStyles.boldLabel);

        GUILayout.Space(10);
        GUILayout.Label("Grid Width: " + gridWidth);
        gridWidth = EditorGUILayout.IntSlider(gridWidth, 1, 50);

        GUILayout.Label("Grid Height: " + gridHeight);
        gridHeight = EditorGUILayout.IntSlider(gridHeight, 1, 50);

        groundPrefab = (GameObject)EditorGUILayout.ObjectField("Zemin Prefab", groundPrefab, typeof(GameObject), false);
        placeablePrefab = (GameObject)EditorGUILayout.ObjectField("Yerleþtirilecek Prefab", placeablePrefab, typeof(GameObject), false);
        gridParent = (GameObject)EditorGUILayout.ObjectField("Grid Parent Objesi", gridParent, typeof(GameObject), true);

        GUILayout.Space(10);
        if (GUILayout.Button(isPreviewing ? "Önizlemeyi Kapat" : "Önizlemeyi Çizdir"))
        {
            isPreviewing = !isPreviewing;

            if (isPreviewing)
            {
                InstantiateGrid();
            }
            else
            {
                while (gridParent.transform.childCount > 0)
                {
                    DestroyImmediate(gridParent.transform.GetChild(0).gameObject);
                }
            }

            SceneView.RepaintAll();
        }

        using (new EditorGUI.DisabledScope(!isPreviewing))
        {
            if (GUILayout.Button("LevelData Kaydet"))
            {
                CreateLvlData(gridWidth, gridHeight);
                isPreviewing = false;
                SceneView.RepaintAll();
            }
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!isPreviewing) return;

        Handles.color = Color.green;

        for (int x = 0; x <= gridWidth; x++)
        {
            Handles.DrawLine(
                new Vector3(x, 0, 0),
                new Vector3(x, 0, gridHeight)
            );
        }

        for (int y = 0; y <= gridHeight; y++)
        {
            Handles.DrawLine(
                new Vector3(0, 0, y),
                new Vector3(gridWidth, 0, y)
            );
        }

        Handles.color = Color.white;
    }

    private void InstantiateGrid()
    {
        if (groundPrefab == null)
        {
            Debug.LogWarning("Zemin prefab atanmadý!");
            return;
        }

        if (gridParent == null)
        {
            Debug.LogWarning("Grid parent objesi atanmadý!");
            return;
        }

        while (gridParent.transform.childCount > 0)
        {
            DestroyImmediate(gridParent.transform.GetChild(0).gameObject);
        }

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = new Vector3(x, 0, y);

                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(groundPrefab);
                instance.transform.position = position;
                instance.transform.SetParent(gridParent.transform);
            }
        }
    }

    private void CreateLvlData(int x, int y)
    {
        LvlDataSO lvlData = CreateInstance<LvlDataSO>();
        lvlData.width = x;
        lvlData.height = y;

        string path = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/LevelData.asset");

        AssetDatabase.CreateAsset(lvlData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
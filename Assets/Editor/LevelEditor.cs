using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

public class LevelEditor : EditorWindow
{
    [SerializeField] private VisualTreeAsset uiAsset = default;

    private List<Blocks> placedPrefabs = new();

    private SliderInt widthSlider;
    private SliderInt heightSlider;
    private Label widthLabel;
    private Label heightLabel;
    private ObjectField tilePrefabField;
    private ObjectField cornerPrefabField;
    private ObjectField parentObjField;
    private ObjectField levelDataSOField;
    private Button drawButton, clearButton, saveButton, loadButton;
    private ObjectField[] slotFields = new ObjectField[5];
    private RadioButton[] slotRadios = new RadioButton[5];

    private const float refWidth = 15f;
    private const float refHeight = 17f;

    private static readonly Vector2[] refPositions = new Vector2[]
    {
        new Vector2(4f, 5f),
        new Vector2(10f, 5f),
        new Vector2(10f, 11f),
        new Vector2(4f, 11f)
    };

    public List<Vector2> GetResponsivePositions()
    {
        var result = new List<Vector2>(refPositions.Length);

        foreach (var p in refPositions)
        {
            float nx = p.x / refWidth;
            float ny = p.y / refHeight;

            float scaledX = nx * widthSlider.value;
            float scaledY = ny * heightSlider.value;

            result.Add(new Vector2(scaledX, scaledY));
        }

        return result;
    }

    private GameObject tilePrefab, cornerPrefab, parentObj;
    private int selectedSlot = -1;

    [MenuItem("Window/Custom Tools/LevelEditor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditor>("LevelEditor");
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    public void CreateGUI()
    {
        var root = rootVisualElement;
        root.Add(uiAsset.Instantiate());

        widthSlider = root.Q<SliderInt>("WidthSlider");
        heightSlider = root.Q<SliderInt>("HeightSlider");
        widthLabel = root.Q<Label>("WidthValue");
        heightLabel = root.Q<Label>("HeightValue");
        widthLabel.text = widthSlider.value.ToString();
        heightLabel.text = heightSlider.value.ToString();
        widthSlider.RegisterValueChangedCallback(evt => widthLabel.text = evt.newValue.ToString());
        heightSlider.RegisterValueChangedCallback(evt => heightLabel.text = evt.newValue.ToString());

        tilePrefabField = root.Q<ObjectField>("TilePrefabField");
        parentObjField = root.Q<ObjectField>("ParentObjField");
        cornerPrefabField = root.Q<ObjectField>("SlotField1");
        levelDataSOField = root.Q<ObjectField>("LevelData");

        for (int i = 0; i < 5; i++)
        {
            slotFields[i] = root.Q<ObjectField>($"SlotField{i + 1}");
            slotRadios[i] = root.Q<RadioButton>($"Slot{i + 1}");
            int idx = i;
            slotRadios[i].RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue) selectedSlot = idx;
                else if (selectedSlot == idx) selectedSlot = -1;
            });
        }

        drawButton = root.Q<Button>("DrawButton"); drawButton.clicked += DrawLevel;
        clearButton = root.Q<Button>("ClearButton"); clearButton.clicked += ClearLevel;
        saveButton = root.Q<Button>("SaveButton"); saveButton.clicked += SaveLevelData;
        loadButton = root.Q<Button>("LoadButton"); loadButton.clicked += LoadLevelData;
    }

    private void OnSceneGUI(SceneView view)
    {
        if (focusedWindow != this) return;
        var e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                var go = hit.collider.gameObject;

                if (!go.CompareTag("TilePrefab"))
                {
                    if (e.shift)
                    {
                        DestroyImmediate(go);
                    }
                }

                else
                {
                    if (selectedSlot >= 0 && slotFields[selectedSlot].value is GameObject prefab)
                        InstantiateAt(prefab, hit.point);
                }

                e.Use();
            }
        }

        if (e.type == EventType.Layout)
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
    }

    private void DrawLevel()
    {
        if (!ValidateBasics()) return;
        ClearLevel();
        placedPrefabs.Clear();

        int W = widthSlider.value;
        int H = heightSlider.value;

        DrawBaseGrid(W, H);
        PlaceRandomObjects(W, H);
    }

    private void ClearLevel()
    {
        parentObj = parentObjField.value as GameObject;
        if (parentObj == null) return;
        for (int i = parentObj.transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(parentObj.transform.GetChild(i).gameObject);
    }

    private void SaveLevelData()
    {
        if (placedPrefabs.Count == 0)
        {
            Debug.LogWarning("Önce bir level çizmelisiniz!");
            return;
        }

        var data = CreateInstance<LvlDataSO>();
        data.width = widthSlider.value;
        data.height = heightSlider.value;
        data.tilePrefab = tilePrefabField.value as GameObject;
        data.cornerPrefab = cornerPrefabField.value as GameObject;
        data.placedBlocksData = new List<Blocks>(placedPrefabs);

        string path = AssetDatabase.GenerateUniqueAssetPath($"Assets/Scripts/SO/SO_{widthSlider.value}x{heightSlider.value}.asset");
        PrefabUtility.SaveAsPrefabAsset(parentObj, path);

        AssetDatabase.CreateAsset(data, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void LoadLevelData()
    {
        var data = levelDataSOField.value as LvlDataSO;
        if (data == null)
        {
            Debug.LogWarning("Yüklenecek bir LevelData atanmamýþ!");
            return;
        }

        parentObj = parentObjField.value as GameObject;
        if (parentObj == null)
        {
            Debug.LogWarning("Parent objesi atayýn!");
            return;
        }

        widthSlider.value = data.width;
        heightSlider.value = data.height;
        tilePrefab = data.tilePrefab;
        cornerPrefab = data.cornerPrefab;

        ClearLevel();
        DrawBaseGrid(data.width, data.height);

        for (int i = 0; i < data.placedBlocksData.Count; i++)
        {
            var block = data.placedBlocksData[i];
            InstantiateAt(block.slotObject, new Vector3(block.index.x, 0, block.index.y));
        }
    }

    private bool ValidateBasics()
    {
        tilePrefab = tilePrefabField.value as GameObject;
        cornerPrefab = cornerPrefabField.value as GameObject;
        parentObj = parentObjField.value as GameObject;

        if (tilePrefab == null || cornerPrefab == null || parentObj == null)
        {
            Debug.LogWarning("Tile, Corner veya Parent objeleri atayýn!");
            return false;
        }
        return true;
    }

    private void DrawBaseGrid(int W, int H)
    {
        float halfX = W / 2f;
        float halfY = H / 2f;

        GameObject tile = Instantiate(tilePrefab, new Vector3(halfX, 0, halfY), Quaternion.identity, parentObj.transform);
        tile.transform.localScale = new Vector3(W, 1, H);

        for (int x = 0; x < W; x++)
        {
            InstantiateAt(cornerPrefab, new Vector3(x, 0, 0));
            InstantiateAt(cornerPrefab, new Vector3(x, 0, H - 1));
        }
        for (int y = 1; y < H - 1; y++)
        {
            InstantiateAt(cornerPrefab, new Vector3(0, 0, y));
            InstantiateAt(cornerPrefab, new Vector3(W - 1, 0, y));
        }
    }

    private void PlaceRandomObjects(int W, int H)
    {
        int innerW = W - 2, innerH = H - 2;
        int maxCount = Mathf.RoundToInt(innerW * innerH * 0.7f);

        HashSet<Vector2Int> reserved = new HashSet<Vector2Int>();
        foreach (var v in GetResponsivePositions())
        {
            Vector2Int center = new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
            reserved.Add(center);
            reserved.Add(center + Vector2Int.up);
            reserved.Add(center + Vector2Int.down);
            reserved.Add(center + Vector2Int.left);
            reserved.Add(center + Vector2Int.right);
        }

        var positions = new List<Vector2Int>();
        for (int x = 1; x < W - 1; x++)
        {
            for (int y = 1; y < H - 1; y++)
            {
                var pos = new Vector2Int(x, y);
                if (!reserved.Contains(pos))
                    positions.Add(pos);
            }
        }

        var rnd = new System.Random();

        for (int i = 0; i < maxCount && positions.Count > 0; i++)
        {
            int idx = rnd.Next(positions.Count);
            var p = positions[idx];
            positions.RemoveAt(idx);
            var valids = new List<GameObject>();

            for (int j = 1; j < slotFields.Length; j++)
            {
                if (slotFields[j].value is GameObject go) valids.Add(go);
            }

            if (valids.Count == 0) break;
            var prefab = valids[rnd.Next(valids.Count)];
            InstantiateAt(prefab, new Vector3(p.x, 0, p.y));
            placedPrefabs.Add(new Blocks { slotObject = prefab, index = new Vector2(p.x, p.y) });
        }
    }

    private void InstantiateAt(GameObject prefab, Vector3 rawPos)
    {
        float x = Mathf.Round(rawPos.x) + 0.5f;
        float z = Mathf.Round(rawPos.z) + 0.5f;
        var go = Instantiate(prefab);
        go.transform.SetParent(parentObj.transform, worldPositionStays: true);
        go.transform.position = new Vector3(x, 0, z);
    }
}
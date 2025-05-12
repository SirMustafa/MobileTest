using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class LevelEditor : EditorWindow
{
    [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;

    private SliderInt widthSlider;
    private SliderInt heightSlider;
    private Label widthLabel;
    private Label heightLabel;
    private ObjectField tilePrefabField;
    private ObjectField parentObjField;
    private ObjectField cornerPrefabField;
    private Button drawButton;
    private Button saveButton;
    private Button clearButton;

    private ObjectField[] slotFields = new ObjectField[5];
    private RadioButton[] slotRadios = new RadioButton[5];
    private int selectedSlot = -1;

    private GameObject tilePrefab;
    private GameObject parentObj;
    private GameObject cornerPrefab;

    [MenuItem("Window/Custom Tools/LevelEditor")]
    public static void ShowWindow()
    {
        var wnd = GetWindow<LevelEditor>();
        wnd.titleContent = new GUIContent("LevelEditor");
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void CreateGUI()
    {
        var root = rootVisualElement;
        root.Add(m_VisualTreeAsset.Instantiate());

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

        drawButton = root.Q<Button>("DrawButton"); drawButton.clicked += DrawLvl;
        clearButton = root.Q<Button>("ClearButton"); clearButton.clicked += ClearLvl;
        saveButton = root.Q<Button>("SaveButton"); saveButton.clicked += SaveLvl;
    }

    private void DrawLvl()
    {
        tilePrefab = tilePrefabField.value as GameObject;
        parentObj = parentObjField.value as GameObject;
        cornerPrefab = cornerPrefabField.value as GameObject;

        if (tilePrefab == null || parentObj == null)
        {
            Debug.LogWarning("Tile Prefab veya Parent Object eksik!");
            return;
        }
        if (cornerPrefab == null)
        {
            Debug.LogWarning("Kenar prefab eksik!");
            return;
        }

        ClearLvl();

        int W = widthSlider.value;
        int H = heightSlider.value;

        for (int x = 0; x < W; x++)
            for (int y = 0; y < H; y++)
                InstantiateAt(tilePrefab, new Vector3(x, 0, y));

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

    private void ClearLvl()
    {
        if (parentObj == null) return;
        for (int i = parentObj.transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(parentObj.transform.GetChild(i).gameObject);
    }

    private void SaveLvl()
    {
        var lvlData = CreateInstance<LvlData>();
        lvlData.width = widthSlider.value;
        lvlData.height = heightSlider.value;

        string path = AssetDatabase.GenerateUniqueAssetPath($"Assets/Scripts/SO_{lvlData.width}x{lvlData.height}.asset");
        AssetDatabase.CreateAsset(lvlData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
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

    private void InstantiateAt(GameObject prefab, Vector3 rawPos)
    {
        if (prefab == null || parentObj == null) return;

        float x = Mathf.Round(rawPos.x * 2f) / 2f;
        float z = Mathf.Round(rawPos.z * 2f) / 2f;
        var go = Instantiate(prefab);
        go.transform.position = new Vector3(x, 0f, z);
        go.transform.SetParent(parentObj.transform, true);
    }
    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
}
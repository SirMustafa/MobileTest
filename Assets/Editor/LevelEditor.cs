using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class LevelEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private SliderInt widthSlider;
    private SliderInt heightSlider;
    private Label widthLabel;
    private Label heightLabel;
    private ObjectField tilePrefabField;
    private ObjectField parentObjField;
    private Button drawButton;
    private Button saveButton;
    private Button clearButton;

    private GameObject tilePrefab;
    private GameObject parentObj;

    [MenuItem("Window/Custom Tools/LevelEditor")]
    public static void ShowWindow()
    {
        LevelEditor wnd = GetWindow<LevelEditor>();
        wnd.titleContent = new GUIContent("LevelEditor");
    }

    public void CreateGUI()
    {
        var root = rootVisualElement;
        var ui = m_VisualTreeAsset.Instantiate();
        root.Add(ui);

        widthSlider = root.Q<SliderInt>("WidthSlider");
        heightSlider = root.Q<SliderInt>("HeightSlider");
        widthLabel = root.Q<Label>("WidthValue");
        heightLabel = root.Q<Label>("HeightValue");
        tilePrefabField = root.Q<ObjectField>("TilePrefabField");
        parentObjField = root.Q<ObjectField>("ParentObjField");
        drawButton = root.Q<Button>("DrawButton");
        saveButton = root.Q<Button>("SaveButton");
        clearButton = root.Q<Button>("ClearButton");
        clearButton.SetEnabled(false);

        widthLabel.text = widthSlider.value.ToString();
        heightLabel.text = heightSlider.value.ToString();

        widthSlider.RegisterValueChangedCallback(evt =>
        {
            widthLabel.text = evt.newValue.ToString();
        });

        heightSlider.RegisterValueChangedCallback(evt =>
        {
            heightLabel.text = evt.newValue.ToString();
        });

        drawButton.clicked += DrawLvl;
        saveButton.clicked += SaveLvl;
        clearButton.clicked += ClearLvl;
    }

    void DrawLvl()
    {
        tilePrefab = tilePrefabField.value as GameObject;
        parentObj = parentObjField.value as GameObject;

        if (tilePrefab == null || parentObj == null)
        {
            Debug.LogWarning("Tile Prefab veya Parent Object eksik.");
            return;
        }

        ClearLvl();

        int width = widthSlider.value;
        int height = heightSlider.value;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                var tile = PrefabUtility.InstantiatePrefab(tilePrefab) as GameObject;
                tile.transform.position = new Vector3(x, 0, y);
                tile.transform.SetParent(parentObj.transform);
            }
    }

    void ClearLvl()
    {
        if (parentObj != null)
        {
            for (int i = parentObj.transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(parentObj.transform.GetChild(i).gameObject);
        }
    }

    void SaveLvl()
    {
        int width = widthSlider.value;
        int height = heightSlider.value;

        LvlData lvlData = CreateInstance<LvlData>();
        lvlData.width = width;
        lvlData.height = height;

        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Scripts/SO" + width + "x" + height + ".asset");

        AssetDatabase.CreateAsset(lvlData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void OnDisable()
    {
        ClearLvl();
    }
}
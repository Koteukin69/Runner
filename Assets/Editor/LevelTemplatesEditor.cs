using UnityEngine;
using UnityEditor;
using Level;

[CustomEditor(typeof(Level.LevelTemplates))]
public class LevelTemplatesEditor : Editor
{
    private SerializedProperty _levelObjectsProp;
    private SerializedProperty _templatesProp;

    private int _selectedBrush = -1;

    private static readonly Color ColorCoin = new Color(1.0f, 0.84f, 0.0f);
    private static readonly Color ColorBarrier = new Color(0.9f, 0.2f, 0.2f);
    private static readonly Color ColorJumpable = new Color(0.2f, 0.6f, 1.0f);
    private static readonly Color ColorRollable = new Color(0.2f, 0.85f, 0.2f);
    private static readonly Color ColorEmpty = new Color(0.25f, 0.25f, 0.25f);

    private static readonly string[] TypeLabels = { "Co", "Ba", "Ju", "Ro" };

    private void OnEnable()
    {
        _levelObjectsProp = serializedObject.FindProperty("_levelObjects");
        _templatesProp = serializedObject.FindProperty("_templates");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        uint lines;
        uint chunkSize;
        if (!TryGetDimensionsFromGameManager(out lines, out chunkSize))
        {
            EditorGUILayout.HelpBox(
                "GameManager not found in scene. Add a GameManager to configure Lines and ChunkSize.",
                MessageType.Error);
            EditorGUILayout.PropertyField(_levelObjectsProp, true);
            EditorGUILayout.PropertyField(_templatesProp, true);
            serializedObject.ApplyModifiedProperties();
            return;
        }

        EditorGUILayout.LabelField("Grid Dimensions (from GameManager)", EditorStyles.boldLabel);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.LabelField("Lines (Lanes)", lines.ToString());
        EditorGUILayout.LabelField("Chunk Size (Rows)", chunkSize.ToString());
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(4);
        DrawPaletteSection();
        EditorGUILayout.Space(4);
        DrawBrushToolbar();
        EditorGUILayout.Space(8);
        DrawTemplatesList(lines, chunkSize);

        serializedObject.ApplyModifiedProperties();
    }

    private bool TryGetDimensionsFromGameManager(out uint lines, out uint chunkSize)
    {
        lines = 0;
        chunkSize = 0;

        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm == null)
            return false;

        SerializedObject gmSO = new SerializedObject(gm);
        SerializedProperty linesProp = gmSO.FindProperty("_lines");
        SerializedProperty chunkSizeProp = gmSO.FindProperty("_chunkSize");

        if (linesProp == null || chunkSizeProp == null)
            return false;

        lines = (uint)linesProp.longValue;
        chunkSize = (uint)chunkSizeProp.longValue;
        return lines > 0 && chunkSize > 0;
    }

    private void DrawPaletteSection()
    {
        EditorGUILayout.LabelField("Level Object Palette", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_levelObjectsProp, true);
    }

    private void DrawBrushToolbar()
    {
        EditorGUILayout.LabelField("Brush", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();

        Color savedBg = GUI.backgroundColor;

        GUI.backgroundColor = _selectedBrush == -1 ? Color.white : new Color(0.5f, 0.5f, 0.5f);
        if (GUILayout.Button("X\nEraser", GUILayout.Width(50), GUILayout.Height(36)))
            _selectedBrush = -1;

        foreach (LevelObjectType type in System.Enum.GetValues(typeof(LevelObjectType)))
        {
            int typeInt = (int)type;
            Color color = GetColorForType(type);

            GUI.backgroundColor = _selectedBrush == typeInt ? color : color * 0.5f;
            if (GUILayout.Button(GetTypeLabel(type), GUILayout.Width(50), GUILayout.Height(36)))
                _selectedBrush = typeInt;
        }

        GUI.backgroundColor = savedBg;
        EditorGUILayout.EndHorizontal();
    }

    private void DrawTemplatesList(uint lines, uint chunkSize)
    {
        EditorGUILayout.LabelField("Templates", EditorStyles.boldLabel);

        if (GUILayout.Button("Migrate Old Templates (Index -> Type)"))
        {
            if (EditorUtility.DisplayDialog("Migrate Templates",
                "This converts grid cells from _levelObjects indices to LevelObjectType values. " +
                "Only run this ONCE after updating to type-based templates.",
                "Migrate", "Cancel"))
            {
                bool anyChanged = false;
                for (int t = 0; t < _templatesProp.arraySize; t++)
                {
                    SerializedProperty gridProp = _templatesProp.GetArrayElementAtIndex(t)
                        .FindPropertyRelative("Grid");

                    for (int i = 0; i < gridProp.arraySize; i++)
                    {
                        int cellValue = gridProp.GetArrayElementAtIndex(i).intValue;
                        if (cellValue < 0) continue;

                        if (cellValue < _levelObjectsProp.arraySize)
                        {
                            SerializedProperty elem = _levelObjectsProp.GetArrayElementAtIndex(cellValue);
                            int typeValue = elem.FindPropertyRelative("Type").enumValueIndex;
                            if (gridProp.GetArrayElementAtIndex(i).intValue != typeValue)
                            {
                                gridProp.GetArrayElementAtIndex(i).intValue = typeValue;
                                anyChanged = true;
                            }
                        }
                        else
                        {
                            gridProp.GetArrayElementAtIndex(i).intValue = -1;
                            anyChanged = true;
                        }
                    }
                }

                if (anyChanged)
                    Debug.Log("Migration complete: template grids converted from object indices to type values.");
                else
                    Debug.Log("Migration: no changes needed.");
            }
        }

        EditorGUILayout.Space(4);

        if (GUILayout.Button("+ Add Template"))
        {
            int size = (int)(lines * chunkSize);
            _templatesProp.arraySize++;
            SerializedProperty newElem = _templatesProp.GetArrayElementAtIndex(_templatesProp.arraySize - 1);
            SerializedProperty gridProp = newElem.FindPropertyRelative("Grid");
            gridProp.arraySize = size;
            for (int i = 0; i < size; i++)
                gridProp.GetArrayElementAtIndex(i).intValue = -1;
        }

        for (int t = 0; t < _templatesProp.arraySize; t++)
        {
            SerializedProperty templateProp = _templatesProp.GetArrayElementAtIndex(t);
            SerializedProperty gridProp = templateProp.FindPropertyRelative("Grid");

            EditorGUILayout.Space(4);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            templateProp.isExpanded = EditorGUILayout.Foldout(
                templateProp.isExpanded,
                "Template " + t,
                true);

            if (GUILayout.Button("Delete", GUILayout.Width(60)))
            {
                _templatesProp.DeleteArrayElementAtIndex(t);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                break;
            }

            EditorGUILayout.EndHorizontal();

            if (templateProp.isExpanded)
            {
                int expectedSize = (int)(lines * chunkSize);
                if (gridProp.arraySize != expectedSize)
                {
                    // Best-effort resize: assume old dims were the same lines count
                    uint oldRows = gridProp.arraySize > 0 ? (uint)(gridProp.arraySize / lines) : 0;
                    if (oldRows == 0) oldRows = 1;
                    ResizeGrid(gridProp, lines, oldRows, lines, chunkSize);
                }

                DrawGrid(gridProp, lines, chunkSize);
            }

            EditorGUILayout.EndVertical();
        }
    }

    private void DrawGrid(SerializedProperty gridProp, uint lines, uint chunkSize)
    {
        float cellSize = 32f;
        float labelWidth = 28f;

        Color savedBg = GUI.backgroundColor;

        EditorGUILayout.LabelField("Grid (bottom = closest to player)");

        for (int row = (int)chunkSize - 1; row >= 0; row--)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(row.ToString(),
                GUILayout.Width(labelWidth), GUILayout.Height(cellSize));

            for (int lane = 0; lane < (int)lines; lane++)
            {
                int index = (int)(row * lines) + lane;

                if (index >= gridProp.arraySize)
                {
                    GUILayout.Space(cellSize);
                    continue;
                }

                int cellValue = gridProp.GetArrayElementAtIndex(index).intValue;

                Color cellColor;
                string cellLabel;

                if (cellValue >= 0 && cellValue <= (int)LevelObjectType.Rollable)
                {
                    LevelObjectType type = (LevelObjectType)cellValue;
                    cellColor = GetColorForType(type);
                    cellLabel = GetTypeLabel(type);
                }
                else if (cellValue >= 0)
                {
                    cellColor = Color.magenta;
                    cellLabel = "??";
                }
                else
                {
                    cellColor = ColorEmpty;
                    cellLabel = "";
                }

                GUI.backgroundColor = cellColor;
                if (GUILayout.Button(cellLabel,
                    GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
                {
                    gridProp.GetArrayElementAtIndex(index).intValue = _selectedBrush;
                }
            }

            GUI.backgroundColor = savedBg;
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(labelWidth + 4);
        for (int lane = 0; lane < (int)lines; lane++)
        {
            EditorGUILayout.LabelField("L" + lane,
                EditorStyles.centeredGreyMiniLabel, GUILayout.Width(cellSize));
        }
        EditorGUILayout.EndHorizontal();
    }

    private static void ResizeGrid(SerializedProperty gridProp,
        uint oldLanes, uint oldRows, uint newLanes, uint newRows)
    {
        int[] oldValues = new int[gridProp.arraySize];
        for (int i = 0; i < oldValues.Length; i++)
            oldValues[i] = gridProp.GetArrayElementAtIndex(i).intValue;

        int newSize = (int)(newLanes * newRows);
        gridProp.arraySize = newSize;

        for (int i = 0; i < newSize; i++)
            gridProp.GetArrayElementAtIndex(i).intValue = -1;

        uint minLanes = System.Math.Min(oldLanes, newLanes);
        uint minRows = System.Math.Min(oldRows, newRows);

        for (uint row = 0; row < minRows; row++)
        {
            for (uint lane = 0; lane < minLanes; lane++)
            {
                int oi = (int)(row * oldLanes + lane);
                int ni = (int)(row * newLanes + lane);
                if (oi < oldValues.Length)
                    gridProp.GetArrayElementAtIndex(ni).intValue = oldValues[oi];
            }
        }
    }

    private static Color GetColorForType(LevelObjectType type)
    {
        switch (type)
        {
            case LevelObjectType.Coin: return ColorCoin;
            case LevelObjectType.Barrier: return ColorBarrier;
            case LevelObjectType.Jumpable: return ColorJumpable;
            case LevelObjectType.Rollable: return ColorRollable;
            default: return Color.gray;
        }
    }

    private static string GetTypeLabel(LevelObjectType type)
    {
        int idx = (int)type;
        if (idx >= 0 && idx < TypeLabels.Length)
            return TypeLabels[idx];
        return "?";
    }
}

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Scriptable Object/Unit")]
public class UnitData : ScriptableObject
{
    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private Faction _faction;
    public Faction Faction => _faction;

    [SerializeField] private Sprite _image;
    public Sprite Image => _image;

    [SerializeField] private BehaviorType _behaviorType;
    public BehaviorType BehaviorType => _behaviorType;

    [SerializeField] private Rarity _rarity;
    public Rarity Rarity => _rarity;

    public List<Vector2> Range => GetAttackRange();

    [SerializeField] private TargetType _targetType;
    public TargetType TargetType => _targetType;

    [SerializeField] private Stat _rawStat;
    public Stat RawStat => _rawStat;

    [SerializeField] private int _manaCost;
    public int ManaCost => _manaCost;

    [SerializeField] private int _firstManaCost;
    public int FirstManaCost => _firstManaCost;

    #region RangeEditor
    const int Arow = 5;
    const int Acolumn = 11;

    const int Mrow = 5;
    const int Mcolumn = 5;

    [SerializeField] [HideInInspector] public bool[] AttackRange = new bool[Arow * Acolumn];
    [SerializeField] [HideInInspector] public bool[] MoveRange = new bool[Mrow * Mcolumn];


    // 공격범위를 캐릭터의 위치를 (0, 0)으로 간주하고 리스트로 반환
    public List<Vector2> GetAttackRange()
    {
        List<Vector2> RangeList = new List<Vector2>();

        for (int i = 0; i < AttackRange.Length; i++)
        {
            if (AttackRange[i])
            {
                int x = (i % Acolumn) - (Acolumn >> 1);
                int y = (i / Acolumn) - (Arow >> 1);

                Vector2 vec = new Vector2(x, y);

                RangeList.Add(vec);
            }
        }

        return RangeList;
    }

    public List<Vector2> GetMoveRange()
    {
        List<Vector2> RangeList = new List<Vector2>();

        for (int i = 0; i < MoveRange.Length; i++)
        {
            if (MoveRange[i])
            {
                int x = (i % Mcolumn) - (Mcolumn >> 1);
                int y = -((i / Mcolumn) - (Mrow >> 1));

                Vector2 vec = new Vector2(x, y);

                RangeList.Add(vec);
            }
        }

        return RangeList;
    }
}

[CustomEditor(typeof(UnitData))]
public class RangeEditor : Editor
{
    #region AttackRange

    const int Arow = 5;
    const int Acolumn = 11;

    const int Mrow = 5;
    const int Mcolumn = 5;

    UnitData _range;
    bool[] atkRange;
    bool[] moveRange;

    private void OnEnable()
    {
        _range = target as UnitData;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("공격 범위");

        atkRange = _range.AttackRange;

        for (int i = 0; i < atkRange.Length; i++)
        {
            if (i % Acolumn == 0)
                GUILayout.BeginHorizontal();

            GUI.color = Color.white;
            if (atkRange[i])
                GUI.color = Color.red;

            if (i == Arow * Acolumn >> 1)
                GUI.color = Color.green;


            SerializedProperty a = serializedObject.FindProperty("AttackRange").GetArrayElementAtIndex(i);
            atkRange[i] = EditorGUILayout.Toggle(atkRange[i]);
            a.boolValue = atkRange[i];

            if (i % Acolumn == Acolumn - 1)
                GUILayout.EndHorizontal();
        }

        #endregion

        #region MoveRange

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("이동 범위");

        moveRange = _range.MoveRange;

        for (int i = 0; i < moveRange.Length; i++)
        {
            if (i % Mcolumn == 0)
                GUILayout.BeginHorizontal();

            GUI.color = Color.white;
            if (moveRange[i])
                GUI.color = Color.red;

            if (i == Mrow * Mcolumn >> 1)
                GUI.color = Color.green;


            SerializedProperty b = serializedObject.FindProperty("MoveRange").GetArrayElementAtIndex(i);
            moveRange[i] = EditorGUILayout.Toggle(moveRange[i]);
            b.boolValue = moveRange[i];

            if (i % Mcolumn == Mcolumn - 1)
                GUILayout.EndHorizontal();
        }

        #endregion

        serializedObject.ApplyModifiedProperties();
    }
}
#endregion
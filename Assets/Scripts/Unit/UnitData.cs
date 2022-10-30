using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit Data", menuName = "Scriptable Object/Unit Data", order = int.MaxValue)]
public class UnitData : ScriptableObject
{
    // 유닛 명칭 및 텍스트
    [SerializeField]
    private string unitName;
    public string UnitName { get { return unitName; } }
    [SerializeField]
    private string unitText;
    public string UnitText { get { return unitText; } }


    // 유닛 스킬
    [SerializeField] 
    private List<SkillData> skills = new List<SkillData>();
    public List<SkillData> Skills { get { return skills; } }

    // 기본 스탯 관련 수치들
    [SerializeField]
    private int atk = 1;
    public int Atk { get { return atk; } }
    [SerializeField]
    private int def = 1;
    public int Def { get { return def; } }
    [SerializeField]
    private int hp = 1;
    public int Hp { get { return hp; } }
    [SerializeField]
    private int maxhp = 1;
    public int Maxhp { get { return maxhp; } }
    [SerializeField]
    private int speed = 1;
    public int Speed { get { return speed; } }


    // 성장 스탯 관련 수치들
    [SerializeField]
    private int level = 1;
    public int Level { get { return level; } }
    [SerializeField]
    private int growth_atk = 1;
    public int growth_Atk { get { return growth_atk; } }
    [SerializeField]
    private int growth_def = 1;
    public int growth_Def { get { return growth_def; } }
    [SerializeField]
    private int growth_hp = 1;
    public int growth_Hp { get { return growth_hp; } }
    [SerializeField]
    private int growth_maxhp = 1;
    public int growth_Maxhp { get { return growth_maxhp; } }
    [SerializeField]
    private int growth_speed = 1;
    public int growth_Speed { get { return growth_speed; } }




    // 애니메이션 이미지

    [SerializeField] public SpriteRenderer character;

    
}
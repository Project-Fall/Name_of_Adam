using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitData : MonoBehaviour
{
    // 변경되는 값들

    public Stat Stat;
    public List<Stigma> Stigma;

    // 변경되지 않는 값들

    public string Name;
    public string Description;
    public Rarity Rarity;
    public Sprite Image;
    public AnimType AnimType;
    public int DarkEssenseDrop;
    public int DarkEssenseCost;
    public BehaviorType BehaviorType;

    const int Arow = 5;
    const int Acolumn = 11;

    const int Mrow = 5;
    const int Mcolumn = 5;

    public bool[] AttackRange = new bool[Arow * Acolumn];
    public bool[] MoveRange = new bool[Mrow * Mcolumn];
}
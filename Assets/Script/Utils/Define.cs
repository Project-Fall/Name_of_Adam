using System;
using System.Collections;
using UnityEngine;

[Serializable]
public struct Stat
{
    public int HP;
    public int ATK;
    public int SPD;
    public int Fall;

    public static Stat operator +(Stat lhs, Stat rhs)
    {
        Stat result = new Stat();
        result.HP = lhs.HP + rhs.HP;
        result.ATK = lhs.ATK + rhs.ATK;
        result.SPD = lhs.SPD + rhs.SPD;
        result.Fall = lhs.Fall + rhs.Fall;

        return result;
    }

    public static Stat operator -(Stat lhs, Stat rhs)
    {
        Stat result = new Stat();
        result.HP = lhs.HP - rhs.HP;
        result.ATK = lhs.ATK - rhs.ATK;
        result.SPD = lhs.SPD - rhs.SPD;
        result.Fall = lhs.Fall - rhs.Fall;

        return result;
    }
}

[Serializable]
public enum Team
{
    Player,
    Enemy,
}

[SerializeField]
public enum Faction
{
    날개의_기사단,
    바벨,
}

[SerializeField]
public enum BehaviorType
{
    원거리,
    근거리,
    서포터,
    탱커,
    전사,
    시즈,
    칼로스,
}

[SerializeField]
public enum Rarity
{
    레어,
    엘리트,
}

[SerializeField]
public enum TargetType
{
    Select,
    Range,
}

public enum Phase
{
    SetupField,
    SpawnEnemyUnit,
    Start,
    Prepare,
    Engage
}

public enum ClickType
{
    Nothing,
    
    Move,
    Before_Attack,
    Attack,
    Prepare
}

[SerializeField]
public enum Scene
{
    Battle,
}

[Serializable]
public struct TestUnit
{
    public GameObject Unit;
    public Vector2 Location;
    public Team Team;
}
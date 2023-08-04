using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayerSkill", menuName = "Scriptable Object/PlayerSkill")]

[Serializable]
public abstract class PlayerSkill : ScriptableObject
{
    public abstract string GetName();
    public abstract int GetManaCost();
    public abstract int GetDarkEssenceCost();

    public abstract string GetDescription();
    public abstract void Use(Vector2 coord);
    public abstract void CancelSelect();
    public abstract void OnSelect();
}
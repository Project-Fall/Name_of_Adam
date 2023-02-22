using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Object/Effect")]
public abstract class Effect : ScriptableObject
{
    public abstract void Use(BattleUnit caster, BattleUnit receiver);
}
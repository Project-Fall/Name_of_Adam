using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill Data", menuName = "Scriptable Object/Skill Data")]
public class SkillData : ScriptableObject
{
    public List<EffectData> effects = new List<EffectData>();

    public void Use(Unit caster, Unit target)
    {
        for (int i = 0; i < effects.Count; i++)
        {
            effects[i].Use(caster, target);
        }
    }
}
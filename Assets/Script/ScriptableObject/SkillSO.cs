using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable Object/Skill", order = 1)]
public class SkillSO : ScriptableObject
{
    [SerializeField] List<EffectSO> EffectList;

    // 이펙트리스트 안의 이펙트들을 순서대로 실행
    public void use(Character ch)
    {
        for(int i = 0; i < EffectList.Count; i++)
        {
            EffectList[i].Effect(ch);
        }
    }
}
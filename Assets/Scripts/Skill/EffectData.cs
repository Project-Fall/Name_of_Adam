using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect Data", menuName = "Scriptable Object/Effect Data")]
public class EffectData : ScriptableObject
{
    // 스킬 계수
    [SerializeField]
    private int percentage;
    public int Percentage { get { return percentage; } }

    // 스킬 지속시간 (일부 적용)
    [SerializeField]
    private int duration;
    public int Duration { get { return duration; } }

    // 추후 이펙트 자식클래스 가져와서 Use 적용하도록 코딩해야함



    virtual public void Use(Unit caster, Unit target)
    {
        
    }
}

public class Attack : EffectData
{
    public override void Use(Unit caster, Unit target)
    {
        base.Use(caster, target);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect Data", menuName = "Scriptable Object/Effect Data")]
public class EffectData : ScriptableObject
{
    // ��ų ���
    [SerializeField]
    private int percentage;
    public int Percentage { get { return percentage; } }

    // ��ų ���ӽð� (�Ϻ� ����)
    [SerializeField]
    private int duration;
    public int Duration { get { return duration; } }

    // ���� ����Ʈ �ڽ�Ŭ���� �����ͼ� Use �����ϵ��� �ڵ��ؾ���



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

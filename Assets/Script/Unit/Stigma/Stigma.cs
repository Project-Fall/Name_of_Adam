using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stigma : MonoBehaviour
{
    [SerializeField] private string _name;

    [SerializeField] private Sprite _sprite;
    public Sprite Sprite => _sprite;

    [SerializeField] private ActiveTiming _activeTiming;
    public ActiveTiming ActiveTiming => _activeTiming;

    [SerializeField] private int _tier;
    public int Tier => _tier;

    [SerializeField, TextArea] private string _description;
    public string Description => _description;

    [SerializeField] private bool _isSpecial = false;
    public bool IsSpecial => _isSpecial;

    [SerializeField] private bool _isLock = false;
    public bool IsLock => _isLock;


    // Memo : 사실 윗 부분만 있다면 SO로 떼서 관리하는 편이 나음...
    // Use의 내용이 달라서 Prefab으로 관리하는 건데 이를 개선할 방법은 없을까

    public virtual void Use(BattleUnit caster, BattleUnit receiver)
    {
        Vector3 pos = caster.transform.position + new Vector3(0, caster.transform.lossyScale.y * 0.5f, 0);
        GameManager.VisualEffect.StartStigmaEffect(Sprite, pos);
        Debug.Log(Sprite);
        return;
    }

    public string GetName()
    {
        return _name;
    }

    public string GetNameWithRomanNumber()
    {
        string name = _name;
        name = $"{_name} {Enum.GetName(typeof(RomanNumber), Tier)}";
        return name;
    }

    public bool Equals(Stigma other, bool isIncludeTier)
    {
        if (isIncludeTier && this.Tier != other.Tier)
            return false;

        if (this.GetType() != other.GetType())
            return false;

        return true;
    }
}
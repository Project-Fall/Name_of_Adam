using System;
using System.Collections;
using UnityEngine;

public class Stigma : MonoBehaviour
{
    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private Sprite _sprite;
    public Sprite Sprite => _sprite;

    [SerializeField, TextArea] private string _description;
    public string Description => _description;

    [SerializeField] private ActiveTiming _activeTiming;
    public ActiveTiming ActiveTiming => _activeTiming;

    [SerializeField] private StigmaTier _tier;
    public StigmaTier Tier => _tier;

    [SerializeField] private bool _isLock = false;
    public bool IsLock => _isLock;


    // Memo : 사실 윗 부분만 있다면 SO로 떼서 관리하는 편이 나음...
    // Use의 내용이 달라서 Prefab으로 관리하는 건데 이를 개선할 방법은 없을까

    public virtual void Use(BattleUnit caster, BattleUnit receiver)
    {
        StigmaEffect(caster);
        return;
    }

    private void StigmaEffect(BattleUnit caster)
    {
        Vector3 pos = caster.transform.position + new Vector3(0, caster.transform.lossyScale.y * 0.5f, 0);
        GameManager.VisualEffect.StartStigmaEffect(Sprite, pos);
        return;
    }
}
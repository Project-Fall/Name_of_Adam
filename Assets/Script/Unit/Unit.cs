using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public UnitData Data;
    
    [SerializeField] private Stat _stat;
    public Stat Stat => Data.RawStat; // Memo : 나중에 낙인, 버프 추가한 스탯으로 수정
    
    [SerializeField] private List<Passive> Stigmas = new List<Passive>();
}
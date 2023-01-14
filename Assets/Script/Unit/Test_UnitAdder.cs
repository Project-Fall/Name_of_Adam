using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_UnitAdder : MonoBehaviour
{
    [SerializeField] List<BattleUnitSO> unit;

    void OnMouseDown()
    {
        Debug.Log("ADD");
        DeckUnit d1 = new DeckUnit();
        d1.SetUnitSO(unit[0]); 
        GameManager.Instance.BattleMNG.BattleDataMNG.AddDeckUnit(d1);
    }
}
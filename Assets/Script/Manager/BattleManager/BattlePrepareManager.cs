using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePrepareManager : MonoBehaviour
{
    private BattleUnit _SelectedUnit;
    public BattleUnit SelectedUnit
    {
        get { return _SelectedUnit; }
        set { _SelectedUnit = value; }
    }

}

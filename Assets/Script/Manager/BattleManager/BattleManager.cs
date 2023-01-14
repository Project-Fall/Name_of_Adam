using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 전투를 담당하는 매니저
// 필드와 턴의 관리
// 필드에 올라와있는 캐릭터의 제어를 배틀매니저에서 담당

public class BattleManager : MonoBehaviour
{
    #region BattleDataManager
    private BattleDataManager _BattleDataMNG;
    public BattleDataManager BattleDataMNG => _BattleDataMNG;
    #endregion

    #region PrepareManager
    private BattlePrepareManager _PrepareMNG;
    public BattlePrepareManager PrepareMNG => _PrepareMNG;
    #endregion
    #region EngageMNG
    private BattleEngageManager _EngageMNG;
    public BattleEngageManager EngageMNG => _EngageMNG;
    #endregion
    #region CutSceneMNG
    private BattleCutSceneManager _CutSceneMNG;
    public BattleCutSceneManager CutSceneMNG => _CutSceneMNG;
    #endregion

    private void Awake()
    {
        _BattleDataMNG = new BattleDataManager();

        _PrepareMNG = GetComponent<BattlePrepareManager>();
        _EngageMNG = GetComponent < BattleEngageManager>();
        _CutSceneMNG = GetComponent<BattleCutSceneManager>();
    }

    public void TurnStart()
    {
        EngageMNG.TurnStart();
    }
}
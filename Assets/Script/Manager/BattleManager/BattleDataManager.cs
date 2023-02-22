using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDataManager : MonoBehaviour
{
    #region Turn Count
    private int _TurnCount = 1;
    public int TurnCount => _TurnCount;

    public void TurnPlus()
    {
        _TurnCount++;
    }
    #endregion

    private List<Unit> _playerDeck = new List<Unit>();
    public List<Unit> PlayerDeck => _playerDeck;

    private List<Unit> _playerHands = new List<Unit>();
    public List<Unit> PlayerHands => _playerHands;

    public void AddUnit(Unit unit) {
        PlayerDeck.Add(unit);
    }

    public void RemoveUnit(Unit unit) {
        PlayerDeck.Remove(unit);
    }

    public Unit GetRandomUnitFromDeck() {
        if (PlayerDeck.Count == 0)
        {
            return null;
        }
        int randNum = Random.Range(0, PlayerDeck.Count);
        
        Unit unit = PlayerDeck[randNum];
        _playerDeck.RemoveAt(randNum);

        return unit;
    }
    
    // 전투를 진행중인 캐릭터가 들어있는 리스트
    List<BattleUnit> _battleUnitList = new List<BattleUnit>();
    public List<BattleUnit> BattleUnitList => _battleUnitList;

    // 현재 리스트를 초기화
    public void UnitListClear() => BattleUnitList.Clear();

    // 리스트에 캐릭터를 추가 / 제거
    public void BattleUnitAdd(BattleUnit unit) => BattleUnitList.Add(unit);

    public void BattleUnitRemove(BattleUnit unit) => BattleUnitList.Remove(unit);
}
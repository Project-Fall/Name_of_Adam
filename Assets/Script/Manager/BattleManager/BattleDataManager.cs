using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleDataManager : MonoBehaviour
{
    private void Start()
    {
        Init();
    }

    [SerializeField] private List<DeckUnit> _playerDeck = new List<DeckUnit>();
    public List<DeckUnit> PlayerDeck => _playerDeck;

    [SerializeField] private List<DeckUnit> _playerHands = new List<DeckUnit>();
    public List<DeckUnit> PlayerHands => _playerHands;
    
    public List<BattleUnit> HitUnits;
    public List<BattleUnit> CorruptUnits;

    private void Init()
    {
        _playerDeck = GameManager.Data.GetDeck().ToList<DeckUnit>();
        foreach (DeckUnit unit in _playerDeck)//UI에서 정보를 표시하기 전에 미리 할인함
        {
            unit.FirstTurnDiscount();
        }
    }

    public void OnBattleOver()
    {
        Debug.Log("BattleDataManager Destroy");

        foreach (BattleUnit unit in BattleUnitList)
        {
            unit.DeckUnit.DeckUnitChangedStat.ClearStat();
            AddDeckUnit(unit.DeckUnit);
            Debug.Log(unit.Data.Name);
        }

        BattleUnitList.Clear();

        foreach (DeckUnit unit in PlayerHands)
        {
            unit.DeckUnitChangedStat.ClearStat();
            AddDeckUnit(unit);
            Debug.Log(unit.Data.Name);
        }

        PlayerHands.Clear();

        GameManager.Data.SetDeck(_playerDeck);
        GameManager.SaveManager.SaveGame();
    }

    private int _turnCount = 0;
    public int TurnCount => _turnCount;

    public void TurnPlus()
    {
        _turnCount++;
        BattleManager.BattleUI.UI_turnCount.Refresh();
    }

    public void DarkEssenseChage(int chage)
    {
        GameManager.Data.DarkEssenseChage(chage);
        BattleManager.BattleUI.UI_darkEssence.Refresh();
    }

    public void AddDeckUnit(DeckUnit unit) =>PlayerDeck.Add(unit);

    public void RemoveDeckUnit(DeckUnit unit) => PlayerDeck.Remove(unit);

    public DeckUnit GetRandomUnitFromDeck()
    {
        if (PlayerDeck.Count == 0)
        {
            return null;
        }
        int randNum = Random.Range(0, PlayerDeck.Count);

        DeckUnit unit = PlayerDeck[randNum];
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

    #region OrderedList
    private List<BattleUnit> _battleUnitOrderList = new List<BattleUnit>();
    public int OrderUnitCount => _battleUnitOrderList.Count;

    public void BattleUnitOrder()
    {
        _battleUnitOrderList.Clear();

        foreach (BattleUnit unit in BattleUnitList)
        {
            _battleUnitOrderList.Add(unit);
        }

        BattleOrderReplace();

        BattleManager.BattleUI.RefreshWaitingLine(_battleUnitOrderList);
    }

    private void BattleOrderReplace()
    {
        _battleUnitOrderList = _battleUnitOrderList.OrderByDescending(unit => unit.BattleUnitTotalStat.SPD)
            .ThenByDescending(unit => unit.Location.y)
            .ThenBy(unit => unit.Location.x)
            .ToList();
    }

    public void BattleOrderRemove(BattleUnit removedUnit)
    {
        BattleManager.BattleUI.UI_waitingLine.RemoveUnit(removedUnit);
        _battleUnitOrderList.Remove(removedUnit);
        BattleManager.BattleUI.RefreshWaitingLine(_battleUnitOrderList);
    }

    public BattleUnit GetNowUnit()
    {
        if (_battleUnitOrderList.Count > 0)
            return _battleUnitOrderList[0];
        return null;
    }
    #endregion
}
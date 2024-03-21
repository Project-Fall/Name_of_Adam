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

    [SerializeField] private List<DeckUnit> _playerDeck = new();
    public List<DeckUnit> PlayerDeck => _playerDeck;

    [SerializeField] private List<DeckUnit> _playerHands = new();
    public List<DeckUnit> PlayerHands => _playerHands;

    // 전투를 진행중인 캐릭터가 들어있는 리스트
    private List<BattleUnit> _battleUnitList = new();
    public List<BattleUnit> BattleUnitList => _battleUnitList;

    // 중복 타락을 처리하기 위한 팝업 리스트
    private List<UI_StigmaSelectButtonPopup> _corruptionPopups = new();
    public List<UI_StigmaSelectButtonPopup> CorruptionPopups => _corruptionPopups;

    [SerializeField] private BattleUnit incarnaUnit;
    public BattleUnit IncarnaUnit => incarnaUnit;

    private Dictionary<int, RewardUnit> _battlePrevUnitDict;
    public Dictionary<int, RewardUnit> BattlePrevUnitDict => _battlePrevUnitDict;
    
    private int _battlePrevDarkEssence;
    public int BattlePrevDarkEssence => _battlePrevDarkEssence;

    public bool isDiscount = false;

    public bool isGameDone = false;

    private void Init()
    {
        _playerDeck = GameManager.Data.GetDeck().ToList<DeckUnit>();
        foreach (DeckUnit unit in _playerDeck)//UI에서 정보를 표시하기 전에 미리 할인함
        {
            unit.FirstTurnDiscount();
        }

        _battlePrevUnitDict = new Dictionary<int, RewardUnit>();
        _battlePrevDarkEssence = GameManager.Data.DarkEssense;

        foreach (DeckUnit unit in _playerDeck)
        {
            _battlePrevUnitDict.Add(unit.UnitID, new RewardUnit(unit.Data.Name, unit.DeckUnitStat.FallCurrentCount, unit.Data.CorruptPortraitImage));
        }
    }

    public void OnBattleOver()
    {
        foreach (BattleUnit unit in _battleUnitList)
        {
            if (unit.IsConnectedUnit || unit.Data.IsBattleOnly)
                continue;

            unit.DeckUnit.DeckUnitChangedStat.ClearStat();
            AddDeckUnit(unit.DeckUnit);
        }

        _battleUnitList.Clear();

        foreach (DeckUnit unit in PlayerHands)
        {
            unit.DeckUnitChangedStat.ClearStat();
            AddDeckUnit(unit);
        }

        PlayerHands.Clear();

        if (GameManager.OutGameData.IsUnlockedItem(8))
        {
            StageData data = GameManager.Data.Map.GetCurrentStage();
            if (data.Name == StageName.EliteBattle)
            {
                foreach (DeckUnit unit in PlayerDeck)
                {
                    if (unit.DeckUnitStat.FallCurrentCount > 0)
                        unit.DeckUnitUpgradeStat.FallCurrentCount--;
                }
            }
        }

        GameManager.Data.SetDeck(_playerDeck);
        GameManager.Data.Map.ClearTileID.Add(GameManager.Data.Map.CurrentTileID);
        GameManager.OutGameData.SaveData();
        GameManager.SaveManager.SaveGame();
    }

    private int _turnCount = 0;
    public int TurnCount => _turnCount;

    public void TurnPlus()
    {
        _turnCount++;
        //BattleManager.BattleUI.UI_turnCount.Refresh();
    }

    public void DarkEssenseChage(int chage)
    {
        GameManager.Data.DarkEssenseChage(chage);
        BattleManager.BattleUI.UI_darkEssence.Refresh();
    }

    public void AddDeckUnit(DeckUnit unit) => PlayerDeck.Add(unit);

    public void RemoveDeckUnit(DeckUnit unit) => PlayerDeck.Remove(unit);

    public DeckUnit GetUnitFromDeck()
    {
        if (PlayerDeck.Count == 0)
        {
            return null;
        }
        DeckUnit unit = PlayerDeck[0];
        _playerDeck.RemoveAt(0);

        return unit;
    }

    public DeckUnit GetTutorialUnitFromDeck()
    {
        if (PlayerDeck.Count == 0)
        {
            return null;
        }

        DeckUnit unit;

        if (PlayerDeck.Count == 4)
        {
            unit = PlayerDeck[3];
            _playerDeck.RemoveAt(3);
        }
        else
        {
            unit = PlayerDeck[0];
            _playerDeck.RemoveAt(0);
        }

        return unit;
    }

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

    #region OrderedList
    private List<(BattleUnit, int?)> _battleUnitOrderUnits = new();
    private List<BattleUnit> _battleUnitOrderList = new();
    public int OrderUnitCount => _battleUnitOrderList.Count;

    public void BattleUnitOrderReplace()
    {
        if (!BattleManager.Phase.CurrentPhaseCheck(BattleManager.Phase.Prepare))
            return;

        _battleUnitOrderUnits.Clear();
        _battleUnitOrderList.Clear();

        foreach (BattleUnit unit in _battleUnitList)
        {
            if (unit.IsConnectedUnit ||
                unit.Data.UnitActionType == UnitActionType.UnitAction_None ||
                unit.Data.UnitActionType == UnitActionType.UnitAction_Horus_Egg
            )
                continue;

            _battleUnitOrderUnits.Add(new(unit, null));
            _battleUnitOrderList.Add(unit);
        }

        BattleUnitOrderSorting();
    }

    public void BattleUnitOrderSorting()
    {
        List<BattleUnit> prevOrderList = new(_battleUnitOrderList);

        _battleUnitOrderList = _battleUnitOrderUnits
            .OrderByDescending(unit => unit.Item2 ?? unit.Item1.BattleUnitTotalStat.SPD)
            .ThenBy(unit => unit.Item1.Team)
            .ThenByDescending(unit => unit.Item1.Location.y)
            .ThenBy(unit => unit.Item1.Location.x)
            .Select(unit => unit.Item1)
            .ToList();

        if (prevOrderList.Count != _battleUnitOrderList.Count)
        {
            BattleManager.BattleUI.RefreshWaitingLine(_battleUnitOrderList);
        }
        else
        {
            for (int i = 0; i < prevOrderList.Count; i++)
            {
                if (prevOrderList[i] != _battleUnitOrderList[i])
                {
                    BattleManager.BattleUI.RefreshWaitingLine(_battleUnitOrderList);
                }
            }
        }
    }

    public void BattleOrderRemove(BattleUnit removedUnit)
    {
        _battleUnitOrderList.Remove(removedUnit);
        BattleManager.BattleUI.RefreshWaitingLine(_battleUnitOrderList);
    }

    public void BattleOrderInsert(int index, BattleUnit addUnit, int? speed = null)
    {
        _battleUnitOrderList.Insert(index, addUnit);
        _battleUnitOrderUnits.Add(new(addUnit, speed));
        BattleManager.BattleUI.RefreshWaitingLine(_battleUnitOrderList);
    }

    public BattleUnit GetNowUnit()
    {
        if (_battleUnitOrderList.Count > 0)
            return _battleUnitOrderList[0];
        return null;
    }
    #endregion

    public UnitAction GetUnitAction(UnitActionType actionType)
    {
        if (actionType == UnitActionType.UnitAction)
        {
            return new UnitAction();
        }
        else if (actionType == UnitActionType.UnitAction_Laser)
        {
            return new UnitAction_Laser();
        }
        else if (actionType == UnitActionType.UnitAction_CenteredSplash)
        {
            return new UnitAction_CenteredSplash();
        }
        else if (actionType == UnitActionType.UnitAction_Iana)
        {
            return new UnitAction_Iana();
        }
        else if (actionType == UnitActionType.UnitAction_Phanuel)
        {
            return new UnitAction_Phanuel();
        }
        else if (actionType == UnitActionType.UnitAction_Appaim)
        {
            return new UnitAction_Appaim();
        }
        else if (actionType == UnitActionType.UnitAction_Tubalcain)
        {
            return new UnitAction_Tubalcain();
        }
        else if (actionType == UnitActionType.UnitAction_Horus)
        {
            return new UnitAction_Horus();
        }
        else if (actionType == UnitActionType.UnitAction_Horus_Egg)
        {
            return new UnitAction_Horus_Egg();
        }
        else if (actionType == UnitActionType.UnitAction_RaquelLeah)
        {
            return new UnitAction_RaquelLeah();
        }
        else
        {
            return new UnitAction_None();
        }
    }
}
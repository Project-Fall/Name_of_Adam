using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_EliteReward : UI_Popup
{
    private readonly int[] probabilityAsStage = new int[] { 99, 89 };

    [SerializeField]
    private GameObject backPanel;

    [SerializeField]
    private List<UI_EliteCard> uI_EliteCards;

    private int _stageAct;

    private DeckUnit GetRandomUnit(List<UnitDataSO> unitLists, List<int> selectedUnitNumbers)
    {
        DeckUnit deckUnit = new DeckUnit();

        int unitNumber = Random.Range(0, unitLists.Count);

        while (selectedUnitNumbers.Contains(unitNumber))
        {
            Debug.Log($"'{unitNumber}번 유닛'은(는) 중복된 유닛입니다.");
            unitNumber = Random.Range(0, unitLists.Count);
        }

        deckUnit.Data = unitLists[unitNumber];
        selectedUnitNumbers.Add(unitNumber);

        SetRandomStigmas(deckUnit);

        return deckUnit;
    }

    private void SetRandomStigmas(DeckUnit deckUnit)
    {
        int addStigmaNum = 1;

        switch (_stageAct)
        {
            case 0: addStigmaNum = 1; break;
            case 1: addStigmaNum = 2; break;
        }

        for (int i = 0; i < addStigmaNum; i++)
        {
            Stigma stigma = GameManager.Data.StigmaController.GetRandomStigmaAsUnit(probabilityAsStage, deckUnit.Data.name);
            deckUnit.AddStigma(stigma);
        }
    }

    public void EnablePanel(bool isEnable)
    => backPanel.SetActive(isEnable);

    public void SetRewardPanel()
    {
        List<UnitDataSO> normalUnits = new();
        List<int> selectedUnitNumbers = new();
        _stageAct = GameManager.Data.StageAct;

        var units = GameManager.Resource.LoadAll<UnitDataSO>($"ScriptableObject/UnitDataSO");
        foreach (var unit in units)
        {
            if (unit.Rarity == Rarity.Normal && !unit.IsBattleOnly)
            {
                #region Demo 전용
                if (unit.ID == "전령" || unit.ID == "집정관" || unit.ID == "도살자")
                {
                    Debug.Log($"'{unit.ID}'은(는) 데모 전용 유닛입니다.");
                    continue;
                }
                #endregion
                normalUnits.Add(unit);
            }
        }

        foreach (var card in uI_EliteCards)
        {
            DeckUnit randUnit = GetRandomUnit(normalUnits, selectedUnitNumbers);
            card.Init(randUnit);
        }

        EnablePanel(true);
    }
}

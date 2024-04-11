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

    private DeckUnit GetRandomNormalUnit(List<UnitDataSO> unitLists)
    {
        DeckUnit deckUnit = new DeckUnit();
        List<int> selectedUnitNumbers = new();

        int unitNumber = Random.Range(0, unitLists.Count);

        while (selectedUnitNumbers.Contains(unitNumber))
        {
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
        _stageAct = GameManager.Data.StageAct;

        var units = GameManager.Resource.LoadAll<UnitDataSO>($"ScriptableObject/UnitDataSO");
        foreach (var unit in units)
        {
            if (unit.Rarity == Rarity.Normal && !unit.IsBattleOnly)
            {
                normalUnits.Add(unit);
            }
        }

        foreach (var card in uI_EliteCards)
        {
            DeckUnit randUnit = GetRandomNormalUnit(normalUnits);
            card.Init(randUnit);
        }

        EnablePanel(true);
    }
}

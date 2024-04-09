using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Info : UI_Scene
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _stat;

    [SerializeField] private UI_HPBar _hpBar;
    [SerializeField] private UI_HoverImageBlock _stigma;
    [SerializeField] private Transform _stigmaGrid;

    [SerializeField] private Transform _rangeGrid;
    [SerializeField] private GameObject _squarePrefab;

    [SerializeField] private Transform _unitInfoFallGrid;
    [SerializeField] private GameObject _fallGaugePrefab;

    [SerializeField] private Transform _stigmaDescriptionGrid;
    [SerializeField] private GameObject _stigmaDescriptionPrefab;

    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private Image _unitImage;



    //색상은 UI에서 정해주는대로
    readonly Color goodColor = Color.yellow;
    readonly Color badColor = Color.red;
    readonly Color textColor = new Color(195f, 195f, 195f);
    readonly string goodColorStr = "yellow";
    readonly string badColorStr = "red";
    readonly string normalColorStr = "white";
    
    

    public void SetInfo(BattleUnit battleUnit)
    {
        //배틀 유닛의 경우
        DeckUnit unit = battleUnit.DeckUnit;

        _name.text = unit.Data.Name;
        Team team = battleUnit.Team;

        if (battleUnit.Team == Team.Player)
        {
            _unitImage.sprite = unit.Data.CorruptDiaPortraitImage;
        }
        else
        {
            _unitImage.sprite = unit.Data.DiaPortraitImage;
        }

        string statText;

        if (unit.Data.DarkEssenseCost > 0)
        {
                statText = GameManager.Locale.GetLocalizedUpgrade("Attack") + ": " + "<color=\"AttackColor\">" + battleUnit.BattleUnitTotalStat.ATK.ToString() + "</color>" + "\n" +
                        GameManager.Locale.GetLocalizedUpgrade("Speed") + ":  " + "<color=\"SpeedColor\">" + battleUnit.BattleUnitTotalStat.SPD.ToString() + "</color>" + "\n" +
                        GameManager.Locale.GetLocalizedUpgrade("Cost") + ":  " + "<color=\"CostColor\">" + battleUnit.BattleUnitTotalStat.ManaCost.ToString() + "/" + unit.Data.DarkEssenseCost.ToString() + "</color>";
        }
        else
        {
                statText = GameManager.Locale.GetLocalizedUpgrade("Attack") + ": " + "<color=\"AttackColor\">" + battleUnit.BattleUnitTotalStat.ATK.ToString() + "</color>" + "\n" +
                        GameManager.Locale.GetLocalizedUpgrade("Speed") + ":  " + "<color=\"SpeedColor\">" + battleUnit.BattleUnitTotalStat.SPD.ToString() + "</color>" + "\n" +
                        GameManager.Locale.GetLocalizedUpgrade("Cost") + ":  " + "<color=\"CostColor\">" + battleUnit.BattleUnitTotalStat.ManaCost.ToString() + "</color>";
        }

                                
        string hpText = battleUnit.BattleUnitTotalStat.CurrentHP.ToString() + "/" + battleUnit.BattleUnitTotalStat.MaxHP.ToString();

        if (unit.DeckUnitChangedStat.ATK > 0 || battleUnit.BattleUnitChangedStat.ATK > 0)
            statText = statText.Replace("AttackColor", goodColorStr);
        else if (unit.DeckUnitChangedStat.ATK < 0 || battleUnit.BattleUnitChangedStat.ATK < 0)
            statText = statText.Replace("AttackColor", badColorStr);
        else
            statText = statText.Replace("AttackColor", normalColorStr);

        if (unit.DeckUnitChangedStat.SPD > 0 || battleUnit.BattleUnitChangedStat.SPD > 0)
            statText = statText.Replace("SpeedColor", goodColorStr);
        else if (unit.DeckUnitChangedStat.SPD < 0 || battleUnit.BattleUnitChangedStat.SPD < 0)
            statText = statText.Replace("SpeedColor", badColorStr);
        else
            statText = statText.Replace("SpeedColor", normalColorStr);

        if (unit.DeckUnitChangedStat.ManaCost > 0 || battleUnit.BattleUnitChangedStat.ManaCost > 0)
            statText = statText.Replace("CostColor", goodColorStr);
        else if (unit.DeckUnitChangedStat.ManaCost < 0 || battleUnit.BattleUnitChangedStat.ManaCost < 0)
            statText = statText.Replace("CostColor", badColorStr);
        else
            statText = statText.Replace("CostColor", normalColorStr);



        _stat.text = statText;

        //_fallText.text = fallText;
        _hpText.text = hpText;

        _hpBar.SetHPBar(team);
        _hpBar.SetFallBar(unit);

        _hpBar.RefreshHPBar((float)battleUnit.BattleUnitTotalStat.CurrentHP / (float)battleUnit.BattleUnitTotalStat.MaxHP);
        _hpBar.RefreshFallBar(battleUnit.Fall.GetCurrentFallCount(), FallAnimType.AnimOff);

        //_stigmaDescriptionPrefab.SetStigma(battleUnit);

        foreach(Stigma stigma in battleUnit.StigmaList)
        {
            UI_StigmaDescription sd = GameObject.Instantiate(_stigmaDescriptionPrefab, _stigmaDescriptionGrid).GetComponent<UI_StigmaDescription>();
            sd.SetStigma(stigma);
        }

        List<int> rangeIntegerList = new();

        foreach (Vector2 vec in battleUnit.GetAttackRange())
        {
            int rangeInteger = 27 + (int)vec.x + (int)vec.y * 11;
            rangeIntegerList.Add(rangeInteger);
        }

        for (int i = 0; i < unit.Data.AttackRange.Length; i++)
        {
            Image block = GameObject.Instantiate(_squarePrefab, _rangeGrid).GetComponent<Image>();
            if (rangeIntegerList.Contains(i))
                block.color = Color.red;
            else
                block.color = Color.grey;
        }

        StartCoroutine(UpdateStigmaGridWithDelay());
    }

    public void SetInfo(DeckUnit unit, Team team)
    {
        //덱 유닛의 경우
        _name.text = unit.Data.Name;

        _unitImage.sprite = unit.Data.Image;

        if (team == Team.Player)
        {
            _unitImage.sprite = unit.Data.CorruptDiaPortraitImage;
        }
        else
        {
            _unitImage.sprite = unit.Data.DiaPortraitImage;
        }

        string statText;

        if (unit.Data.DarkEssenseCost > 0)
        {
            statText = GameManager.Locale.GetLocalizedUpgrade("Attack") + ": " + "<color=\"AttackColor\">" + unit.DeckUnitTotalStat.ATK.ToString() + "</color>" + "\n" +
                    GameManager.Locale.GetLocalizedUpgrade("Speed") + ":  " + "<color=\"SpeedColor\">" + unit.DeckUnitTotalStat.SPD.ToString() + "</color>" + "\n" +
                    GameManager.Locale.GetLocalizedUpgrade("Cost") + ":  " + "<color=\"CostColor\">" + unit.DeckUnitTotalStat.ManaCost.ToString() + "/" + unit.Data.DarkEssenseCost.ToString() + "</color>";
        }
        else
        {
            statText = GameManager.Locale.GetLocalizedUpgrade("Attack") + ": " + "<color=\"AttackColor\">" + unit.DeckUnitTotalStat.ATK.ToString() + "</color>" + "\n" +
                    GameManager.Locale.GetLocalizedUpgrade("Speed") + ":  " + "<color=\"SpeedColor\">" + unit.DeckUnitTotalStat.SPD.ToString() + "</color>" + "\n" +
                    GameManager.Locale.GetLocalizedUpgrade("Cost") + ":  " + "<color=\"CostColor\">" + unit.DeckUnitTotalStat.ManaCost.ToString() + "</color>";
        }

        string hpText = unit.DeckUnitTotalStat.CurrentHP.ToString() + "/" + unit.DeckUnitTotalStat.MaxHP.ToString();

        if (unit.DeckUnitChangedStat.ATK > 0)
            statText = statText.Replace("AttackColor", goodColorStr);
        else if (unit.DeckUnitChangedStat.ATK < 0)
            statText = statText.Replace("AttackColor", badColorStr);
        else
            statText = statText.Replace("AttackColor", normalColorStr);

        if (unit.DeckUnitChangedStat.SPD > 0)
            statText = statText.Replace("SpeedColor", goodColorStr);
        else if (unit.DeckUnitChangedStat.SPD < 0)
            statText = statText.Replace("SpeedColor", badColorStr);
        else
            statText = statText.Replace("SpeedColor", normalColorStr);

        if (unit.DeckUnitChangedStat.ManaCost > 0)
            statText = statText.Replace("CostColor", goodColorStr);
        else if (unit.DeckUnitChangedStat.ManaCost < 0)
            statText = statText.Replace("CostColor", badColorStr);
        else
            statText = statText.Replace("CostColor", normalColorStr);

        //fallText = fallText.Replace("textColor" , )

        _stat.text = statText;
        _hpText.text = hpText;

        _hpBar.SetHPBar(team);
        _hpBar.SetFallBar(unit);

        _hpBar.RefreshHPBar((float)unit.DeckUnitTotalStat.CurrentHP / (float)unit.DeckUnitTotalStat.MaxHP);
        _hpBar.RefreshFallBar(unit.DeckUnitTotalStat.FallCurrentCount, FallAnimType.AnimOff);

        //_stigmaDescriptionPrefab.SetStigma(unit);


        foreach (Stigma stigma in unit.GetStigma())
        {
            UI_StigmaDescription sd = GameObject.Instantiate(_stigmaDescriptionPrefab, _stigmaDescriptionGrid).GetComponent<UI_StigmaDescription>();
            sd.SetStigma(stigma);
        }

        foreach (bool range in unit.Data.AttackRange)
        {
            Image block = GameObject.Instantiate(_squarePrefab, _rangeGrid).GetComponent<Image>();
            if (range)
                block.color = Color.red;
            else
                block.color = Color.grey;
        }

        StartCoroutine(UpdateStigmaGridWithDelay());
    }

    IEnumerator UpdateStigmaGridWithDelay()
    {
        yield return null;

        VerticalLayoutGroup group = _stigmaGrid.GetComponent<VerticalLayoutGroup>();
        group.childForceExpandWidth = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(group.GetComponent<RectTransform>());
    }

    public void InfoDestroy()
    {
        GameManager.Resource.Destroy(this.gameObject);
    }
}

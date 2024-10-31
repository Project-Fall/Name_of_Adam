using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UI_HallCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text _slotName;
    [SerializeField] private TMP_Text _slotDescription;

    [SerializeField] private GameObject _infoButton;
    [SerializeField] private List<GameObject> _frameList;
    [SerializeField] private TMP_Text _nameText;

    [SerializeField] private Image _unitImage;

    [SerializeField] private List<GameObject> _stigmataFrame;
    [SerializeField] private List<Image> _stigmataImage;

    [SerializeField] private GameObject _highlight;

    [SerializeField] public int _hallSlotID;

    [SerializeField] private Image _insignia;

    [SerializeField] private Image _apostleInsignia;
    [SerializeField] private Image _eliteInsignia;
    [SerializeField] private Image _bossInsignia;

    readonly Color _playerInsigniaColor = new(0.35f, 0.09f, 0.05f);
    readonly Color _enemyInsigniaColor = new(0.54f, 0.5f, 0.34f);

    private SlotRank _slotRank;

    private List<HallUnit> _hallUnitList;
    private List<DeckUnit> _mainDeck;

    private DeckUnit _deckUnit;

    public void Init()
    {
        if (GameManager.OutGameData.IsUnlockedItem(SanctumUnlock.UnlockingTheDivineHall2))
        {
            _slotRank = (_hallSlotID == 0) ? SlotRank.Divine : SlotRank.Advanced;
        }
        else if (GameManager.OutGameData.IsUnlockedItem(SanctumUnlock.UnlockingTheDivineHall1))
        {
            _slotRank = (_hallSlotID == 0 || _hallSlotID == 1) ? SlotRank.Advanced : SlotRank.Normal;
        }
        else
        {
            _slotRank = (_hallSlotID == 0) ? SlotRank.Advanced : SlotRank.Normal;
        }

        _slotName.text = GameManager.Locale.GetLocalizedEventScene(_slotRank.ToString() + "_Name");
        _slotDescription.text = GameManager.Locale.GetLocalizedEventScene(_slotRank.ToString() + "_Description");

        _slotDescription.GetComponent<Animator>().speed = 3f;

        _hallUnitList = GameManager.OutGameData.FindHallUnitList();
        _mainDeck = GameManager.Data.GameDataMain.DeckUnits;
        _mainDeck.Sort((x, y) => x.HallUnitID.CompareTo(y.HallUnitID));

        _deckUnit = _mainDeck.Find(x => x.HallUnitID == _hallSlotID);
        if (_deckUnit == null)
        {
            DisableUI();
            return;
        }

        _frameList[0].SetActive(_deckUnit.Data.Rarity == Rarity.Normal);
        _frameList[1].SetActive(_deckUnit.Data.Rarity != Rarity.Normal);

        _unitImage.sprite = _deckUnit.Data.CorruptImage;
        _unitImage.gameObject.SetActive(true);

        _nameText.SetText(_deckUnit.Data.Name);
        _infoButton.SetActive(true);

        List<Stigma> stigmaList = _deckUnit.GetStigma();
        for (int i = 0; i < _stigmataImage.Count; i++)
        {
            if (i < stigmaList.Count)
            {
                _stigmataFrame[i].GetComponent<UI_StigmaHover>().SetStigma(stigmaList[i]);
                _stigmataImage[i].sprite = stigmaList[i].Sprite_28;
                _stigmataImage[i].gameObject.SetActive(true);
            }
            else
            {
                _stigmataFrame[i].GetComponent<UI_StigmaHover>().SetEnable(false);
                _stigmataImage[i].gameObject.SetActive(false);
            }
        }

        _highlight.SetActive(false);

        _insignia.color = _playerInsigniaColor;

        _apostleInsignia.gameObject.SetActive(_deckUnit.Data.Rarity == Rarity.Original);
        _eliteInsignia.gameObject.SetActive(_deckUnit.Data.Rarity == Rarity.Elite);
        _bossInsignia.gameObject.SetActive(_deckUnit.Data.Rarity == Rarity.Boss);
    }

    private void DisableUI()
    {
        _unitImage.gameObject.SetActive(false);
        _nameText.SetText("");
        _infoButton.SetActive(false);
        _insignia.gameObject.SetActive(false);
        foreach (var frame in _stigmataFrame)
        {
            frame.SetActive(false);
        }
    }

    public void OnClick()
    {
        GameManager.Sound.Play("UI/UISFX/UISelectSFX");

        if (_hallSlotID == 1 || _hallSlotID == 2)
        {
            GameManager.UI.ShowPopup<UI_SystemInfo>().Init("DemoClear", "DemoClearToolTip");
            return;
        }

        GameManager.UI.ShowPopup<UI_MyDeck>().HallDeckInit(_slotRank, OnSelect);
    }

    //������ ���� GameDataMain �� ����ǰ� �ϱ�
    public void OnSelect(DeckUnit unit)
    {
        DeckUnit afterDeckUnit = unit;
        HallUnit afterHallUnit = _hallUnitList.Find(x => x.ID == afterDeckUnit.HallUnitID);

        if (_mainDeck.Find(x => x.HallUnitID == _hallSlotID) == null)
        {
            // 신규 유닛이면 추가
            if (_mainDeck.Count < _hallSlotID)
                _mainDeck.Add(unit);
            else
                _mainDeck.Insert(_hallSlotID, unit);
        }
        else
        {
            // 이전 유닛이 있다면 스왑
            DeckUnit beforeDeckUnit = GameManager.Data.GetDeck().Find(x => x.HallUnitID == _hallSlotID);
            HallUnit beforeHallUnit = _hallUnitList.Find(x => x.ID == beforeDeckUnit.HallUnitID);

            beforeDeckUnit.IsMainDeck = false;
            beforeHallUnit.IsMainDeck = false;
            beforeDeckUnit.HallUnitID = afterDeckUnit.HallUnitID;
            beforeHallUnit.ID = afterHallUnit.ID;

            _mainDeck[_hallSlotID] = unit;
        }

        // GameData Deck 수정
        afterDeckUnit.IsMainDeck = true;
        afterHallUnit.IsMainDeck = true;
        afterDeckUnit.HallUnitID = _hallSlotID;
        afterHallUnit.ID = _hallSlotID;

        GameManager.UI.ClosePopup();
        GameManager.UI.ClosePopup();

        Init();
    }

    public void OnInfoButton()
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");
        UI_UnitInfo ui = GameManager.UI.ShowPopup<UI_UnitInfo>();

        ui.SetUnit(_mainDeck[_hallSlotID]);
        ui.Init();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _highlight.SetActive(true);
        _slotDescription.gameObject.SetActive(true);
        _slotDescription.GetComponent<Animator>().SetBool("isFadeIn", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _highlight.SetActive(false);
        _slotDescription.GetComponent<Animator>().SetBool("isFadeIn", false);
    }
}

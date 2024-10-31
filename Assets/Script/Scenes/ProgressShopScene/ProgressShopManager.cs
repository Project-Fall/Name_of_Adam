using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ProgressShopManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI info_name;
    [SerializeField] private TextMeshProUGUI info_description;
    [SerializeField] private TextMeshProUGUI active_info_cost;
    [SerializeField] private TextMeshProUGUI disabled_info_cost;
    [SerializeField] private GameObject activeBtn;
    [SerializeField] private GameObject disabledBtn;
    [SerializeField] private TMP_Text ProgressCoin;
    private int selectedID;

    [SerializeField] public List<ShopNode> ShopNodes;
    [SerializeField] public GameObject ItemInfo;
    [SerializeField] public GameObject ItemInfoSoul;

    public void Start()
    {
        selectedID = 0;
        ProgressCoin.text = GameManager.OutGameData.Data.ProgressCoin.ToString();
        SetNodeImage();

        if (!GameManager.OutGameData.Data.IsOnTooltipForSanctum)
        {
            GameManager.OutGameData.Data.IsOnTooltipForSanctum = true;
            UI_SystemInfo systemInfo = GameManager.UI.ShowPopup<UI_SystemInfo>();
            systemInfo.Init("TooltipForSanctum", "");

            GameManager.OutGameData.SaveData();
        }
    }

    public void OnClickShopNode(int id)
    {
        GameManager.Sound.Play("UI/UISFX/UISelectSFX");

        ShopNode foundNode = ShopNodes.FirstOrDefault(node => node.ItemID == selectedID);
        ShopNode newfoundNode = ShopNodes.FirstOrDefault(node => node.ItemID == id);

        if (selectedID == id && ItemInfo.activeSelf == true)
        {
            newfoundNode.Highlighted.SetActive(false);
            ItemInfo.SetActive(false);
        }
        else if (selectedID == id && ItemInfo.activeSelf == false)
        {
            newfoundNode.Highlighted.SetActive(true);
            ItemInfo.SetActive(true);
        }
        else
        {
            foundNode.Highlighted.SetActive(false);
            newfoundNode.Highlighted.SetActive(true);

            selectedID = id;

            ItemInfo.SetActive(true);

            info_name.text = GameManager.Locale.GetLocalizedProgress(GameManager.OutGameData.GetProgressItem(id).Name);
            info_description.text = GameManager.Locale.GetLocalizedProgress(GameManager.OutGameData.GetProgressItem(id).Description).Replace("\\n", "\n");

            if (!GameManager.OutGameData.GetBuyable(id) && GameManager.OutGameData.IsUnlockedItem(id))
            {
                ChangeBtnImage(false);
                disabled_info_cost.text = GameManager.Locale.GetLocalizedProgress("Purchased");
                ItemInfoSoul.SetActive(false);
            }
            else if (!GameManager.OutGameData.GetBuyable(id) && !GameManager.OutGameData.IsUnlockedItem(id))
            {
                ChangeBtnImage(false);
                disabled_info_cost.text = GameManager.OutGameData.GetProgressItem(id).Cost.ToString();
                ItemInfoSoul.SetActive(true);
            }
            else if (GameManager.OutGameData.Data.ProgressCoin < GameManager.OutGameData.GetProgressItem(id).Cost)
            {
                ChangeBtnImage(false);
                disabled_info_cost.text = GameManager.OutGameData.GetProgressItem(id).Cost.ToString();
                disabled_info_cost.color = Color.gray;
                ItemInfoSoul.SetActive(true);
            }
            else
            {
                ChangeBtnImage(true);
                active_info_cost.text = GameManager.OutGameData.GetProgressItem(id).Cost.ToString();
                ItemInfoSoul.SetActive(true);
            }

            #region Demo 전용
            if (selectedID == 11 || selectedID == 53 || selectedID == 61 || selectedID == 71)
            {
                GameManager.UI.ShowPopup<UI_SystemInfo>().Init("DemoClear", "DemoClearToolTip");
                return;
            }
            #endregion
        }
    }

    public void OnBuyBtnClick()
    {
        if (!GameManager.OutGameData.GetBuyable(selectedID) || GameManager.OutGameData.Data.ProgressCoin < GameManager.OutGameData.GetProgressItem(selectedID).Cost)
        {
            return;
        }

        GameManager.Sound.Play("UI/UISFX/UISuccessSFX");

        GameManager.OutGameData.BuyProgressItem(selectedID);
        ProgressCoin.text = GameManager.OutGameData.Data.ProgressCoin.ToString();
        ChangeBtnImage(false);
        ItemInfoSoul.SetActive(false);
        disabled_info_cost.text = GameManager.Locale.GetLocalizedProgress("Purchased");
        SetNodeImage();

        // 스팀 해금 체크
        if (selectedID == 52 || selectedID == 53 || selectedID == 54)
            if (GameManager.Steam.IsDoneIncarnaUnlock01())
                GameManager.Steam.IncreaseAchievement(SteamAchievementType.UNLOCK_INCARNA_1);

        if (selectedID == 71 || selectedID == 72 || selectedID == 73 || selectedID == 74)
            if (GameManager.Steam.IsDoneIncarnaUnlock02())
                GameManager.Steam.IncreaseAchievement(SteamAchievementType.UNLOCK_INCARNA_2);

        if (selectedID == 61 || selectedID == 62 || selectedID == 63 || selectedID == 64)
            if (GameManager.Steam.IsDoneIncarnaUnlock03())
                GameManager.Steam.IncreaseAchievement(SteamAchievementType.UNLOCK_INCARNA_3);
    }

    public void ChangeBtnImage(bool isActive)
    {
        if (isActive)
        {
            activeBtn.SetActive(true);
            disabledBtn.SetActive(false);
        }
        else
        {
            activeBtn.SetActive(false);
            disabledBtn.SetActive(true);
        }
    }

    public void OnDisabledBtnClick()
    {
        GameManager.Sound.Play("UI/UISFX/UIFailSFX");
    }

    public void OnMenuBtnClick()
    {
        ShopNode foundNode = ShopNodes.FirstOrDefault(node => node.ItemID == selectedID);
        foundNode.Highlighted.SetActive(false);
    }

    public void SetNodeImage()
    {
        foreach(ShopNode node in ShopNodes)
        {
            node.SetImage();
        }
    }
}

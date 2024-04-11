using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private static GameManager s_instance;
    public static GameManager Instance { get { Init(); return s_instance; } }

    [SerializeField] private UIManager _ui;
    public static UIManager UI => Instance._ui;

    [SerializeField] DataManager _data;
    public static DataManager Data => Instance._data;

    [SerializeField] private SoundManager _sound;
    public static SoundManager Sound => Instance._sound;

    private ResourceManager _resource = new ResourceManager();
    public static ResourceManager Resource => Instance._resource;

    [SerializeField] private InputManager _input;
    public static InputManager InputManager => Instance._input;

    [SerializeField] private VisualEffectManager _visualEffect;
    public static VisualEffectManager VisualEffect => Instance._visualEffect;

    [SerializeField] private SaveController _saveController;
    public static SaveController SaveManager => Instance._saveController;

    [SerializeField] private OutGameDataContainer _outGameData;
    public static OutGameDataContainer OutGameData => Instance._outGameData;

    [SerializeField] private LocaleManager _locale;
    public static LocaleManager Locale => Instance._locale;

    private static bool _onGM = false;

    void Awake()
    {
        if (s_instance != null)
            Destroy(gameObject); // 이미 GameManager가 있으면 이 오브젝트를 제거
        else
        {
            SaveManager.Init();
            OutGameData.Init();
            Data.Init();
            Sound.Init();
            VisualEffect.Init();
            Locale.Init();
        }

        /*
        if (s_instance != null)
            return;

        SaveManager.Init();
        Data.Init();
        Sound.Init();
        VisualEffect.Init();
        */
    }

    private static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@GameManager");

            if (go == null)
            {
                go = new GameObject("@GameManager");
                go.AddComponent<GameManager>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<GameManager>();

            SaveManager.Init();
            OutGameData.Init();
            Data.Init();
            Sound.Init();
            VisualEffect.Init();
        }
    }

    private void Update()
    {
        if (!_onGM) // GM 모드가 꺼져있다면 리턴
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            Time.timeScale = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            Time.timeScale = 2;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            Time.timeScale = 4;
        if (Input.GetKeyDown(KeyCode.Alpha4))
            Time.timeScale = 0.2f;

        if (Input.GetKeyDown(KeyCode.O))
        {
            while (true)
            {
                BattleUnit unit = BattleManager.Data.BattleUnitList.Find(x => x.Team == Team.Enemy);
                if (unit == null)
                    break;

                unit.UnitDiedEvent();
                //GameManager.Data.GameData.NpcQuest.UpgradeQuest++;
            }
            BattleManager.Instance.BattleOverCheck();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameManager.VisualEffect.StartBenedictionEffect(BattleManager.Data.GetNowUnit());
        }
        if (Input.GetKeyDown(KeyCode.Z))
            GameManager.SaveManager.DeleteSaveData();

        if (Input.GetKeyDown(KeyCode.D))
        {
            Data.MainDeckLayoutSet();
            GameManager.OutGameData.DeleteAllData();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Data.GameData.NpcQuest.UpgradeQuest += 26;
            Data.GameData.NpcQuest.StigmaQuest += 13;
            Data.GameData.NpcQuest.DarkshopQuest += 8;

            Debug.Log($"타락도 조정: {Data.GameData.NpcQuest.UpgradeQuest}|{Data.GameData.NpcQuest.StigmaQuest}|{Data.GameData.NpcQuest.DarkshopQuest}");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            UI_StigmaSelectButtonPopup stigmaPopup = GameObject.FindObjectOfType<UI_StigmaSelectButtonPopup>();
            if (stigmaPopup != null)
                stigmaPopup.ResetStigmaSelectButtons();

            UI_UpgradeSelectButton upgradePopup = GameObject.FindObjectOfType<UI_UpgradeSelectButton>();
            if (upgradePopup != null)
                upgradePopup.ResetUpgradeSelectButtons();

            UI_EliteReward uI_EliteReward = GameObject.FindObjectOfType<UI_EliteReward>();
            if (uI_EliteReward != null)
                uI_EliteReward.SetRewardPanel();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (BattleManager.Data == null)
                return;

            foreach (var unit in BattleManager.Data.BattleUnitList)
            {
                if (unit.Team == Team.Player)
                {
                    unit.ChangeHP(1000);
                    unit.ChangeFall(-4);
                }
            }
        }
    }
}
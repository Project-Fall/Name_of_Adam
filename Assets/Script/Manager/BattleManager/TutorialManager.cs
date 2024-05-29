using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    /// <summary>
    /// 툴팁에 출력될 문자열
    /// [CTRL]로 끝나는 문자열은 유저가 직접 액션을 하는 단계를 의미
    /// 즉, 유저의 특정 행동으로 다음 튜토리얼 진행 가능
    /// </summary>
    private readonly string[][] TooltipTexts =
    {
        // 영문
        new string[] {
        // 튜토리얼 1 시작
        "During the <color=#FF9696>player turn<color=white>, you can summon units or use skills.",
        "<color=#FF9696>Mana<color=white> is required for summoning units or using skills.\nMana recovers by <color=#FF9696>30<color=white> each player turn",
        "These are the currently summonable units.\n<color=#FF9696>On the first player turn<color=white>, you can summon units using only <color=#FF9696>half of<color=white> the required mana.",
        "These are the skills that aid you in combat.",
        "Summon a Gravekeeper.[CTRL]",
        "Summon a Gravekeeper.[CTRL]",
        "When the player turn ends, the <color=#FF9696>unit turn<color=white> comes.[CTRL]",
        "During the <color=#FF9696>unit turn<color=white>, units on the field move according to their speed.\nUnits at the top of the <color=#FF9696>speed bar<color=white> on the right act first.",
        "Each unit can move one step and then attack the enemies.\nMove the Gravekeeper one step forward.[CTRL]",
        "Attack the Swordsman.[CTRL]",

        // 튜토리얼 2 시작
        "This is the <color=#FF9696>dark essence<color=white> needed for using specific units or skills.\nDark essence is obtained by defeating enemies",
        "The <color=#FF9696>dark knight<color=white> is a powerful unit that consumes both mana and <color=#FF9696>Dark Essence.<color=white>\nSummon the Dark Knight.[CTRL]",
        "The <color=#FF9696>dark knight<color=white> is a powerful unit that consumes both mana and <color=#FF9696>Dark Essence.<color=white>\nSummon the Dark Knight.[CTRL]",
        "The <color=#FF9696>malevolence buff<color=white> reduces an enemy's <color=#FF9696>faith<color=white> when attacking.\nThe Dark Knight has the stigmata that provides the malevolence <color=#FF9696>buff twice.<color=white>\nEffectively utilize these instructions to corrupt enemies.",
        "Click the <color=#FF9696>'Turn End' <color=white>button to move to the unit turn.[CTRL]",
        "Press the <color=#FF9696>Turn End button<color=white> to skip to the next turn when moving is unnecessary.[CTRL]",
        "Attack the swordsman to reduce <color=#FF9696>faith.<color=white>[CTRL]",
        "Use the skill <color=#FF9696>Whisper<color=white> to reduce the enemy's faith and  corrupt them.[CTRL]",
        "Use the skill <color=#FF9696>Whisper<color=white> to reduce the enemy's faith and  corrupt them.[CTRL]",
        "When corrupting an enemy, you can choose a <color=#FF9696>stigmata<color=white> to apply and convert them into an ally.\nSelect a stigmata to bestow upon the swordsman.[CTRL]",
        "The swordsman has become your unit. Now, click 'Turn End'.[CTRL]",
        "When a unit moves to a position where an ally already exists, the two units change places.\nMove the dark knight.[CTRL]",
        "Attack the Nun to remove her invincibility buff.[CTRL]",
        "Move the Swordsman.[CTRL]",
        "Finish off the Nun, now that the invincibility buff has disappeared.[CTRL]",
        "",
        },

        // 한국
        new string[] {
        // 튜토리얼 1 시작
        "<color=#FF9696>플레이어 턴<color=white>에는 유닛을 소환하거나 스킬을 쓸 수 있습니다.",
        "유닛을 소환하거나 스킬을 사용할때 필요한 <color=#FF9696>마나<color=white>입니다.\n플레이어 턴이 될 때마다 <color=#FF9696>30<color=white>씩 회복합니다.",
        "현재 소환할 수 있는 유닛들입니다.\n<color=#FF9696>첫번째 플레이어 턴<color=white>에는 <color=#FF9696>절반의 마나<color=white>를 사용하여 유닛을 소환할 수 있습니다.",
        "전투를 보조하는 스킬들입니다.",
        "묘지기를 소환해보세요.[CTRL]",
        "묘지기를 소환해보세요.[CTRL]",
        "턴을 종료하면 <color=#FF9696>유닛 턴<color=white>으로 넘어갑니다.[CTRL]",
        "<color=#FF9696>유닛 턴<color=white>에는 필드에 있는 각 유닛들이 속도에 따라 움직입니다.\n우측의 <color=#FF9696>속도표<color=white>에서 상단에 있는 유닛일수록 먼저 행동합니다.",
        "각 유닛은 한칸 이동 후 적을 공격할 수 있습니다.\n묘지기를 앞으로 한칸 이동시켜보세요.[CTRL]",
        "검병을 공격해보세요.[CTRL]",

        // 튜토리얼 2 시작
        "특정 유닛을 사용하거나 스킬을 사용할 때 필요한 <color=#FF9696>검은 정수<color=white>입니다.\n적을 처치할 때마다 하나씩 얻을 수 있습니다.",
        "흑기사는 적을 <color=#FF9696>타락<color=white>하는데 유용한 성흔을 지닌 강한 유닛이며 마나뿐만 아니라 <color=#FF9696>검은 정수<color=white>까지 소모합니다.\n흑기사를 선택하세요.[CTRL]",
        "흑기사는 적을 <color=#FF9696>타락<color=white>하는데 유용한 성흔을 지닌 강한 유닛이며 마나뿐만 아니라 <color=#FF9696>검은 정수<color=white>까지 소모합니다.\n흑기사를 선택하세요.[CTRL]",
        "공격 시 적의 <color=#FF9696>신앙<color=white>을 떨어뜨리는 <color=#FF9696>악성 버프<color=white>입니다.\n흑기사는 이 악성 버프를 <color=#FF9696>2회<color=white> 얻는 성흔을 가지고 있습니다.\n잘 활용하여 적을 타락시켜보세요.",
        "<color=#FF9696>턴 종료 버튼<color=white>을 눌러 유닛 턴으로 넘어가세요.[CTRL]",
        "이동이 필요가 없는 경우 <color=#FF9696>턴 종료 버튼<color=white>을 눌러 턴을 넘길 수 있어요.[CTRL]",
        "검병을 공격하여 <color=#FF9696>신앙<color=white>을 떨어뜨리세요.[CTRL]",
        "<color=#FF9696>신앙<color=white>을 떨어뜨리는 스킬 <color=#FF9696>속삭임<color=white>을 사용하여 적을 타락시켜 보세요.[CTRL]",
        "<color=#FF9696>신앙<color=white>을 떨어뜨리는 스킬 <color=#FF9696>속삭임<color=white>을 사용하여 적을 타락시켜 보세요.[CTRL]",
        "적을 타락시킬 경우 해당 적을 아군으로 만들며 <color=#FF9696>성흔<color=white>을 부여할 수 있습니다.\n검병에게 부여할 성흔을 선택하세요.[CTRL]",
        "이제 검병은 당신의 유닛이 되었습니다.\n이제 턴 종료를 누르세요.[CTRL]",
        "아군이 이미 있는 위치로 이동할 시 두 유닛은 서로 위치를 바꿉니다.\n흑기사를 이동시키세요.[CTRL]",
        "수녀를 공격하여 무적 버프를 없애보세요.[CTRL]",
        "검병을 이동시키세요.[CTRL]",
        "부적 버프가 사라진 수녀를 마무리하세요.[CTRL]",
        "",
        },

        //일본어
        new string[] {
        // 튜토리얼 1 시작
        "<color=#FF9696>プレイヤーターン<color=white>にはユニットを召喚したり、スキルを使うことができます。",
        "ユニットを召喚したり、スキルを使うときに必要な<color=#FF9696>マナ<color=white>です。\nプレイヤーターンごとに <color=#FF9696>30<color=white>ずつ回復します。",
        "現在召喚できるユニットです.\n<color=#FF9696>最初のプレイヤーターン<color=white>には、<color=#FF9696>半分のマナ<color=white>を使用してユニットを召喚することができます。",
        "戦闘を補助するスキルです。",
        "墓守を召喚してみてください。[CTRL]",
        "墓守を召喚してみてください。[CTRL]",
        "ターンを終了すると<color=#FF9696>ユニットターン<color=white>に移ります[CTRL]",
        "<color=#FF9696>ユニットターン<color=white>では、フィールドにある各ユニットが速度に応じて移動します。\n右側の<color=#FF9696>速度表<color=white>で上位のユニットが先に行動します.",
        "各ユニットは1マス移動後、敵を攻撃することができます。\n墓守を前に1マス移動させてみてください。[CTRL]",
        "剣兵を攻撃してみてください。[CTRL]",

        // 튜토리얼 2 시작
        "特定のユニットを使用したり、スキルを使用する際に必要な<color=#FF9696>ダーク・エッセンス<color=white>です.\n敵を倒すたびに1つずつ獲得できます。",
        "黒騎士は敵を<color=#FF9696>堕落<color=white>に有用な聖痕を持つ強力なユニットであり、マナだけでなく<color=#FF9696>ダーク・エッセンス<color=white>まで消費します\n黒騎士を選択してください。[CTRL]",
        "黒騎士は敵を<color=#FF9696>堕落<color=white>に有用な聖痕を持つ強力なユニットであり、マナだけでなく<color=#FF9696>ダーク・エッセンス<color=white>まで消費します\n黒騎士を選択してください。[CTRL]",
        "攻撃時に敵の<color=#FF9696>信仰<color=white>を低下させる<color=#FF9696>悪性バフ<color=white>です。\n黒騎士はこの悪性バフを<color=#FF9696>2回<color=white>得られる聖痕を持っています\nうまく活用して敵を堕落させましょう。",
        "<color=#FF9696>ターン終了ボタン<color=white>を押してユニットターンに進みましょう。[CTRL]",
        "移動の必要がない場合 <color=#FF9696>ターン終了ボタン<color=white>を押すことでターンを切り替えることができます。[CTRL]",
        "剣兵を攻撃して <color=#FF9696>信仰<color=white>を落としてください。[CTRL]",
        "<color=#FF9696>信仰<color=white>を落とすスキル <color=#FF9696>ささやき<color=white>を使用して敵を堕落させましょう。[CTRL]",
        "<color=#FF9696>信仰<color=white>を落とすスキル <color=#FF9696>ささやき<color=white>を使用して敵を堕落させましょう。[CTRL]",
        "敵を堕落させる場合、その敵を味方にすることで<color=#FF9696>聖痕<color=white>を付与することができます\n剣兵に付与する聖痕を選択してください。[CTRL]",
        "これで剣兵はあなたのユニットになりました。\nターン終了を押してください。[CTRL]",
        "味方が既にいる位置に移動する場合、2つのユニットはお互いの位置を入れ替えます\n黒騎士を移動させてください。[CTRL]",
        "修道女を攻撃して無敵バフを解除してください。[CTRL]",
        "剣瓶を移動させてください。[CTRL]",
        "無敵バフが消えた修道女を仕留めてください。[CTRL]",
        "",
        },
    };

    public const int STEP_BOUNDARY = 100;

    private const float RECLICK_TIME = 0.5f;

    private static TutorialManager _instance;
    public static TutorialManager Instance
    {
        set
        {
            if (_instance == null)
                _instance = value;
        }
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    private UI_Tutorial UI;

    [SerializeField]
    private TutorialStep _step;

    public TutorialStep Step => _step;

    private TooltipData currentTooltip;

    public bool IsTutorialactive;
    private bool isEnable;
    private bool isCanClick;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        int curID = GameManager.Data.Map.CurrentTileID;

        switch (curID)
        {
            case 1: _step = TutorialStep.UI_PlayerTurn; break;
            case 2: _step = TutorialStep.UI_FallSystem; break;
            case 3: _step = TutorialStep.UI_Defeat; break;
        }

        isCanClick = true;

        //디버그용
        //var deckUnits = GameManager.Data.GameData.DeckUnits;
        //var fallenUnits = GameManager.Data.GameData.FallenUnits;

        //Debug.Log("=====================================");
        //foreach (var unit in deckUnits)
        //{
        //    //unit의 메모리 주소
        //    Debug.Log($"DeckUnit / {unit.UnitID} / {unit.Data.Name} / {unit.GetStigmaCount()} / {unit.GetUpgradeData().Count}");
        //}
        //Debug.Log("=====================================");
        //foreach (var unit in fallenUnits)
        //{
        //    //unit의 메모리 주소
        //    Debug.Log($"FallenUnit / {unit.UnitID} / {unit.Data.Name}/ {unit.GetStigmaCount()} / {unit.GetUpgradeData().Count}");
        //}
        //Debug.Log("=====================================");
    }

    private void Update()
    {
        if (!isEnable)
            return;

        if (UI.ValidToPassTooltip)
        {
            if (isCanClick && !GameManager.UI.IsOnESCOption && GameManager.InputManager.Click)
            {
                StartCoroutine(ClickCoolTime());
                ShowNextTutorial();
            }
        }
    }

    public void ShowNextTutorial()
    {
        if (CheckStep(TutorialStep.UI_Devine) || CheckStep(TutorialStep.UI_Last))
            return; // 마지막 UI 튜토리얼 관련 Step은 조건부 동작이기 때문에 예외 처리

        SetNextStep();
        ShowTutorial();
    }

    public void ShowPreviousTutorial()
    {
        SetPreviousStep();
        ShowTutorial();
    }

    public bool IsEnable()
        => !GameManager.OutGameData.IsTutorialClear() && isEnable;

    public void SetNextStep()
    {
        TutorialStep[] steps = (TutorialStep[])Enum.GetValues(typeof(TutorialStep));
        int next = Array.IndexOf(steps, _step) + 1;
        _step = (steps.Length == next) ? steps[0] : steps[next];
    }

    private void SetPreviousStep()
    {
        TutorialStep[] steps = (TutorialStep[])Enum.GetValues(typeof(TutorialStep));
        int next = Array.IndexOf(steps, _step) - 1;
        _step = (steps.Length == -1) ? steps[steps.Length - 1] : steps[next];
    }

    private bool IsToolTip(TutorialStep step)
        => (int)step % STEP_BOUNDARY != 0;

    private TooltipData AnalyzeTooltip(TutorialStep step)
    {
        TooltipData tooltip = new TooltipData();
        int indexToTooltip = (int)step % STEP_BOUNDARY - 1;

        tooltip.Step = step;
        tooltip.Info = TooltipTexts[GameManager.OutGameData.GetLanguage()][indexToTooltip].Replace("[CTRL]", "");
        tooltip.IndexToTooltip = indexToTooltip;
        tooltip.IsCtrl = TooltipTexts[GameManager.OutGameData.GetLanguage()][indexToTooltip].Contains("[CTRL]");
        tooltip.IsEnd = false;

        if (CheckStep(TutorialStep.Tutorial_End_1) || 
            CheckStep(TutorialStep.Tutorial_End_2) || 
            CheckStep(TutorialStep.Tutorial_End_3))
            tooltip.IsEnd = true;

        return tooltip;
    }

    private int AnalyzeUI(TutorialStep step) => (int)step / STEP_BOUNDARY - 1;

    public bool CheckStep(TutorialStep step) => this.Step == step;

    public void ShowTutorial()
    {
        Debug.Log(_step);

        if (IsToolTip(_step))
        {
            // Tooltip 모드
            currentTooltip = AnalyzeTooltip(_step);
            if (currentTooltip.IsEnd)
                DisableToolTip();
            else
                EnableToolTip(currentTooltip);
        }
        else
        {
            // UI 모드
            int indexToUI = AnalyzeUI(_step);
            isEnable = true;
            UI.TutorialActive(indexToUI);
        }
    }

    public void DisableToolTip()
    {
        UI.CloseToolTip();
        UI.SetUIMask(-1);
        UI.SetValidToPassToolTip(false);
        SetActiveAllTiles(true);
        isEnable = false;
    }

    public void EnableToolTip(TooltipData data)
    {
        UI.ShowTooltip(data.Info, data.IndexToTooltip);
        UI.SetUIMask(data.IndexToTooltip);
        UI.SetValidToPassToolTip(!data.IsCtrl);
        SetTutorialField(data.Step);
        isEnable = true;
    }

    private void SetTutorialField(TutorialStep step)
    {
        SetActiveAllTiles(false);

        switch (step)
        {
            case TutorialStep.Tooltip_UnitSpawnSelect:
                BattleManager.Field.TileDict[new Vector2(1, 1)].SetActiveCollider(true);
                break;
            case TutorialStep.Tooltip_UnitMove:
                BattleManager.Field.TileDict[new Vector2(2, 1)].SetActiveCollider(true);
                break;
            case TutorialStep.Tooltip_UnitAttack:
                BattleManager.Field.TileDict[new Vector2(3, 1)].SetActiveCollider(true);
                break;
            case TutorialStep.Tooltip_BlackKnightSpawn:
                BattleManager.Field.TileDict[new Vector2(2, 1)].SetActiveCollider(true);
                break;
            case TutorialStep.Tooltip_UnitAttack_2:
                BattleManager.Field.TileDict[new Vector2(3, 1)].SetActiveCollider(true);
                break;
            case TutorialStep.Tooltip_PlayerSkillUse:
                BattleManager.Field.TileDict[new Vector2(3, 1)].SetActiveCollider(true);
                break;
            case TutorialStep.Tooltip_UnitSwap:
                BattleManager.Field.TileDict[new Vector2(3, 1)].SetActiveCollider(true);
                break;
            case TutorialStep.Tooltip_UnitAttack_3:
                BattleManager.Field.TileDict[new Vector2(4, 1)].SetActiveCollider(true);
                break;
            case TutorialStep.Tooltip_UnitSwap_2:
                BattleManager.Field.TileDict[new Vector2(3, 1)].SetActiveCollider(true);
                break;
            case TutorialStep.Tooltip_UnitAttack_4:
                BattleManager.Field.TileDict[new Vector2(4, 1)].SetActiveCollider(true);
                break;
        }
    }

    private void SetActiveAllTiles(bool isActive)
    {
        foreach (Tile tile in BattleManager.Field.TileDict.Values)
            tile.SetActiveCollider(isActive);
    }

    private IEnumerator ClickCoolTime()
    {
        isCanClick = false;
        yield return new WaitForSeconds(RECLICK_TIME);
        isCanClick = true;
    }
}

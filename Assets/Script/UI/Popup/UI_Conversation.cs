using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Conversation : UI_Popup
{
    private float _typingSpeed = 0.04f;
    private Coroutine co_typing = null;
    private List<Script> scripts = new();
    private bool _battleConversation;

    //[SerializeField] private Text _conversationText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _conversation;
    [SerializeField] private Image _unitImage;
    [SerializeField] private GameObject _panel;

    public event Action ConversationEnded;

    // autoStart = false면 따로 실행해줘야 함
    public void Init(List<Script> scripts, bool autoStart = true, bool battleConversation= false)
    {
        this.scripts = scripts;
        _panel.SetActive(battleConversation);

        _battleConversation = battleConversation;

        if (!_battleConversation)
        {
            _unitImage.gameObject.SetActive(false);
        }
        else
        {
            _conversation.rectTransform.sizeDelta = new(1100, _conversation.rectTransform.sizeDelta.y);
            _conversation.transform.localPosition = new(-200, 0, 0);
        }

        if (autoStart)
            StartCoroutine(PrintScript());
    }

    public IEnumerator PrintScript()
    {
        // 이벤트 스크립트 본문 출력
        foreach (Script script in scripts)
        {
            string name = GameManager.Locale.GetLocalizedScriptName(script.name);
            string dialog = GameManager.Locale.GetLocalizedScriptInfo(script.script);

            co_typing = StartCoroutine(TypingEffect(dialog));
            if (_battleConversation)
            {
                _unitImage.sprite = GameManager.Resource.Load<Sprite>($"Arts/Conversation/" + name);
            }

            _nameText.text = name;

            yield return new WaitUntil(() => (co_typing == null || GameManager.InputManager.Click));

            if (co_typing != null)
            {
                StopCoroutine(co_typing);
                co_typing = null;
            }
            _conversation.text = dialog;

            yield return new WaitUntil(() => GameManager.InputManager.Click);

            GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");
        }

        if (_battleConversation)
            BattleManager.Phase.ChangePhase(BattleManager.Phase.Prepare);

        ConversationEnded?.Invoke();

        GameManager.UI.ClosePopup(this);
    }

    private IEnumerator TypingEffect(string script)
    {
        _conversation.text = "";

        foreach (char c in script.ToCharArray())
        {
            _conversation.text += c;
            yield return new WaitForSeconds(_typingSpeed);
        }

        _conversation.text = script;
        co_typing = null;
    }

}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UI_TurnChangeButton : UI_Scene, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button _changeButton;
    [SerializeField] private Image _buttonImage;
    [SerializeField] private Image _lightImage;

    private readonly float _lightMaxAlpha = 0.7f;
    private readonly float _lightMinAlpha = 0.2f;
    private readonly float _lightFadeSpeed = 0.5f;

    private readonly Vector3 _initSize = Vector3.one;
    private readonly Vector3 _goalSize = Vector3.one * 1.1f;

    private bool _isCanCtrl;

    public void SetEnable(bool enable)
    {
        _isCanCtrl = enable;
        Debug.Log("Turn Change Button : " + enable);
        // �Ͽ��� �� �Ͻ� ���
        //_lightImage.color = new Color(1, 1, 1, _lightMinAlpha);
        //_lightImage.transform.localScale = _initSize;

        //if (_isCanCtrl)
        //{
        //    StartCoroutine(nameof(FadeLight));
        //    StartCoroutine(nameof(SizeUp));
        //}
        //else
        //{
        //    StopCoroutine(nameof(FadeLight));
        //    StopCoroutine(nameof(SizeUp));
        //}
    }


    public void TurnChange()
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");

        if (_isCanCtrl)
        {
            PhaseController _phase = BattleManager.Phase;
            _isCanCtrl = false;

            if (_phase.CurrentPhaseCheck(_phase.Prepare))
                _phase.ChangePhase(_phase.Engage);
            else if (_phase.CurrentPhaseCheck(_phase.Move))
                _phase.ChangePhase(_phase.Action);
            else if (_phase.CurrentPhaseCheck(_phase.Action))
            {
                BattleManager.Data.BattleOrderRemove(BattleManager.Data.GetNowUnitOrder());
                _phase.ChangePhase(_phase.Engage);
            }
        }
    }
    /*
    IEnumerator SizeUp()
    {
        float time = 0.0f;

        while (time < 1.0f)
        {
            time += Time.deltaTime;
            _lightImage.transform.localScale = Vector3.Lerp(_initSize, _goalSize, time);
            yield return null;
        }

        _lightImage.transform.localScale = _goalSize;
        time = 1.0f;

        while (time > 0.0f)
        {
            time -= Time.deltaTime;
            _lightImage.transform.localScale = Vector3.Lerp(_initSize, _goalSize, time);
            yield return null;
        }

        _lightImage.transform.localScale = _initSize;
        
        StartCoroutine(nameof(SizeUp));
    }

    IEnumerator FadeLight()
    {
        Color color = _lightImage.color;

        while (color.a < _lightMaxAlpha)
        {
            color.a += Time.deltaTime * _lightFadeSpeed;
            _lightImage.color = color;
            yield return null;
        }

        color.a = _lightMaxAlpha;
        _lightImage.color = color;
        
        while (color.a > _lightMinAlpha)
        {
            color.a -= Time.deltaTime * _lightFadeSpeed;
            _lightImage.color = color;
            yield return null;
        }

        color.a = _lightMinAlpha;
        _lightImage.color = color;

        StartCoroutine(nameof(FadeLight));
    }
    */
    private bool _isHover = false;
    private bool _isHoverMessegeOn = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHover = true;
        BattleManager.Instance.PlayAfterCoroutine(() => {
            if (_isHover)
            {
                _isHoverMessegeOn = true;
                GameManager.UI.ShowHover<UI_TextHover>().SetText(
                    $"{GameManager.Locale.GetLocalizedBattleScene("TurnChange UI Info")}", Input.mousePosition);
            }
        }, 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHover = false;

        if (_isHoverMessegeOn)
        {
            _isHoverMessegeOn = false;
            GameManager.UI.CloseHover();
        }
    }
}

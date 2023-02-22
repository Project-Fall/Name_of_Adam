using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private int _order = 10;
    private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    public GameObject Root
    {
        get
        {   // Find 사용하지 않도록 변경하고 싶음
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject("@UI_Root");
            return root;
        }
    }

    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    public T ShowScene<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = GameManager.Resource.Instantiate($"UI/Scene/{name}", Root.transform);
        T scene = go.GetOrAddComponent<T>();
        SetCanvas(go, false);

        return scene;
    }

    /// <summary>
    /// Popup UI를 띄울 때 사용합니다.
    /// (UI의 컴포넌트와 Prefab의 이름이 다를 시 반드시 name 필수)
    /// </summary>
    /// <typeparam name="T">Popup으로 띄울 UI의 컴포넌트 이름</typeparam>
    /// <param name="name">UI Prefab의 이름</param>
    /// <returns></returns>
    public T ShowPopup<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = GameManager.Resource.Instantiate($"UI/Popup/{name}", Root.transform);
        T popup = go.GetOrAddComponent<T>();
        SetCanvas(go, true);
        _popupStack.Push(popup);

        return popup;
    }

    /// <summary>
    /// Popup UI를 닫을 때 사용합니다. 주어진 UI가 닫힐 차례인지 확인합니다. (안전한 버전)
    /// </summary>
    /// <param name="popup"></param>
    public void ClosePopup(UI_Popup popup)
    {
        if (_popupStack.Count <= 0)
            return;

        if (_popupStack.Peek() != popup)
        {
            Debug.Log($"Failed to close popup : {popup.name}, Now : {_popupStack.Peek().name}");
            return;
        }

        ClosePopup();
    }

    /// <summary>
    /// Popup UI를 닫을 때 사용합니다.
    /// </summary>
    public void ClosePopup()
    {
        if (_popupStack.Count <= 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        GameManager.Resource.Destroy(popup.gameObject);
        popup = null;

        _order--;
    }

    /// <summary>
    /// 현재 띄워진 모든 Popup UI를 닫을 때 사용합니다.
    /// </summary>
    public void CloseAllPopup()
    {
        while (_popupStack.Count > 0)
            ClosePopup();
    }

    public UI_Popup GetLastPopup()
    {
        if (_popupStack.Count <= 0)
            return null;

        return _popupStack.Peek();
    }
}
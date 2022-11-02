using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 싱글턴이 정의된 클래스입니다. 이 클래스를 상속받은 클래스는 싱글턴을 사용할 수 있습니다.
/// </summary>
/// <typeparam name="T"> MonoBehaviour, ISingleton </typeparam>

public abstract class Singleton<T> : MonoBehaviour, ISingleton
    where T : MonoBehaviour, ISingleton
{
    private static T s_instance;

    public static T Instance
    {
        get
        {
            if (s_instance != null)
                return s_instance;
            Create();
            return s_instance;
        }
    }
    protected virtual void Awake()
    {
        if (s_instance == null)
            Create();
    }

    private static void Create()
    {
        Find();
        if (s_instance == null)
        {
            var className = typeof(T).ToString(); // 클래스 이름 가져오기
            var managerObj = new GameObject(className); // 게임 오브젝트 새로 만들기
            s_instance = managerObj.AddComponent<T>(); // 클래스 붙이기
            DontDestroyOnLoad(s_instance);
        }
    }

    private static void Find()
    {
        s_instance = FindObjectOfType<T>(); // 현재 씬에 클래스가 있는지 확인
    }

    public static bool IsExist()
    {
        Find();
        if (s_instance == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}

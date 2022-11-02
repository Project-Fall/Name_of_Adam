using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 매니저들의 부모 클래스입니다. 싱글톤을 상속받습니다.
/// </summary>
public abstract class Manager<T> : Singleton<T> where T : Manager<T>
{
    protected Manager() { }
}

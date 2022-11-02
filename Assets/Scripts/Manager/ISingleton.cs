using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 매니저의 인터페이스입니다. 매니저들 싱글턴이 구현되어 있습니다.
/// </summary>
public interface ISingleton
{
    public static ISingleton Instance { get; set; }

    private static void Create()
    {
        throw new System.NotImplementedException();
    }
    private static void Find()
    {
        throw new System.NotImplementedException();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] Camera MainCamera;
    [SerializeField] Camera CutSceneCamera;

    CutSceneManager _CutSceneMNG;
    Field _field;

    private void Start()
    {
        _CutSceneMNG = GameManager.CutScene;
        _field = GameManager.Battle.Field;

        SetMainCamera();
    }

    #region ActiveCameraSet

    // 컷씬 전용 카메라로 전환
    void SetCutSceneCamera()
    {
        CutSceneCamera.enabled = true;
        MainCamera.enabled = false;

        CutSceneCamera.transform.position = MainCamera.transform.position;
        CutSceneCamera.fieldOfView = MainCamera.fieldOfView;
    }
    // 메인카메라로 전환
    void SetMainCamera()
    {
        MainCamera.enabled = true;
        CutSceneCamera.enabled = false;
    }

    #endregion

    #region BattleCutScene

    // 배틀 컷씬 시작
    public void CutSceneZoomIn(CutSceneData CSData)
    {
        SetCutSceneCamera();

        StartCoroutine(ZoomIn(CSData));
    }
    // 화면 줌 인
    IEnumerator ZoomIn(CutSceneData CSData)
    {
        float time = 0;

        while (time <= CSData.ZoomTime)
        {
            time += Time.deltaTime;
            float t = time / CSData.ZoomTime;

            CutSceneCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, CSData.ZoomLocation, t);
            CutSceneCamera.fieldOfView = Mathf.Lerp(CSData.DefaultZoomSize, CSData.ZoomSize, t);
            _CutSceneMNG.MoveUnitZoomIn(CSData, t);

            yield return null;
        }
        
        StartCoroutine(_CutSceneMNG.AttackCutScene(CSData));
    }

    // 컷씬 중 카메라 회전
    public IEnumerator CameraLotate(float time)
    {
        float t = 0;

        while (t < time)
        {
            t += Time.deltaTime;

            Vector3 vec = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, 0, 2), t);
            CutSceneCamera.transform.rotation = Quaternion.Euler(vec);

            yield return null;
        }
    }

    // 컷씬 후 화면 줌 아웃
    public IEnumerator CutSceneZoomOut(CutSceneData CSData)
    {
        float time = 0;

        while (time <= CSData.ZoomTime)
        {
            time += Time.deltaTime;
            float t = time / CSData.ZoomTime;

            CutSceneCamera.transform.position = Vector3.Lerp(CSData.ZoomLocation, MainCamera.transform.position, t);
            CutSceneCamera.fieldOfView = Mathf.Lerp(CSData.ZoomSize, CSData.DefaultZoomSize, t);
            _CutSceneMNG.MoveUnitZoomOut(CSData, t);

            Vector3 vec = Vector3.Lerp(new Vector3(0, 0, 2), new Vector3(0, 0, 0), t);
            CutSceneCamera.transform.rotation = Quaternion.Euler(vec);
            yield return null;
        }
        SetMainCamera();

        _field.ClearAllColor();
        yield return new WaitForSeconds(0.2f);

        _CutSceneMNG.EndAttack();
    }

    #endregion


    public RaycastHit2D[] CameraRayCast()
    {
        Vector3 mos = Input.mousePosition;
        mos.z = MainCamera.farClipPlane;

        Vector3 dir = MainCamera.ScreenToWorldPoint(mos);
        StartCoroutine(ray(dir));
        return Physics2D.RaycastAll(MainCamera.transform.position, dir);
    }

    IEnumerator ray(Vector3 dir)
    {
        float time = 0;

        while (time <= 3)
        {
            time += Time.deltaTime;

            Debug.DrawRay(MainCamera.transform.position, dir, Color.red, 100);

            yield return null;
        }
    }
}
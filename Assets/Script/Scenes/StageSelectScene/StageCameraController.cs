using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageCameraController : MonoBehaviour
{
    [SerializeField] Transform MapTransform;
    private float _cameraSpeed = 4.0f; // 증가할 때 카메라 이동 속도 느려짐
    private Vector3 _topPosition = new(0, 20, -10);
    private Vector3 _bottomPosition = new(0, -2, -10);

    const int _wheelSpeed = 200;

    private void Start()
    {
        StartCoroutine(MapScanMove());
    }

    private void Update()
    {
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        MoveCamera(wheel);

        float mousePosition = Input.mousePosition.y;

        if (mousePosition > Screen.height * 0.99f)
        {
            MoveCamera(0.03f);
        }
        else if (mousePosition < Screen.height * 0.01f)
        {
            MoveCamera(-0.03f);
        }
    }

    private Vector3 _velocity = Vector3.zero; // 초기 속도값
    
    void MoveCamera(float num)
    {
        if ((_velocity.y == 0 && num == 0) || EventSystem.current.IsPointerOverGameObject())
            return;

        Vector3 desiredMove = new(0, transform.position.y + num * _wheelSpeed, -10);
        Vector3 movePosition = Vector3.SmoothDamp(transform.position, desiredMove, ref _velocity, 0.3f);

        movePosition.y = Mathf.Clamp(movePosition.y, _bottomPosition.y, _topPosition.y);
        transform.position = movePosition;
    }


    public void SetLocate(float y)
    {
        transform.localPosition = new Vector3(0, y, -10);
    }

    IEnumerator MapScanMove()
    {
        if (GameManager.Data.StageAct == 2)
        {
            _topPosition = new Vector3(0, 12, -10);
        }

        if (GameManager.Data.Map.GetCurrentStage().Type == StageType.Tutorial && GameManager.Data.Map.CurrentTileID == 3)
        {
            _bottomPosition = new Vector3(0, 8, -10);
        }
        else if (GameManager.Data.Map.CurrentTileID != 0 || !GameManager.OutGameData.Data.TutorialClear)
        {
            yield break;
        }

        transform.position = _topPosition;

        yield return new WaitForSeconds(0.8f);

        float elapsedTime = 0;

        while (elapsedTime < _cameraSpeed)
        {
            float t = elapsedTime / _cameraSpeed;
            t = Mathf.Sin((t * Mathf.PI) / 2);

            transform.position = Vector3.Lerp(_topPosition, _bottomPosition, t);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = _bottomPosition;
        yield return null;

        if (GameManager.Data.StageAct == 1)
        {
            GameManager.UI.ShowPopup<UI_SystemInfo>().Init("DemoClear", "DemoClearToolTip", () =>
            {
                GameManager.OutGameData.Data.TutorialClear = false;
                GameManager.OutGameData.SetCutSceneData(CutSceneType.Main, false);
                GameManager.OutGameData.SetCutSceneData(CutSceneType.Tubalcain_Enter, false);
                GameManager.OutGameData.SetCutSceneData(CutSceneType.RahelLea_Enter, false);
                GameManager.OutGameData.SetCutSceneData(CutSceneType.Phanuel_Enter, false);
                GameManager.OutGameData.SaveData();
                GameManager.SaveManager.DeleteSaveData();
                SceneChanger.SceneChange("MainScene");
            });

        }
    }
}

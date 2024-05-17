using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubSceneController : MonoBehaviour
{
    void Start()
    {
        Init();
    }

    private void Init()
    {

    }

    public void GameStart()
    {
        if (GameManager.OutGameData.IsTutorialClear())
        {
            GameManager.Data.HallDeckSet();
            SceneChanger.SceneChange("DifficultySelectScene");
        }
        else
        {
            SceneChanger.SceneChangeToCutScene(CutSceneType.Main);
        }
    }

    public void ProgressStore()
    {
        SceneChanger.SceneChange("ProgressScene");
    }

    public void Hall()
    {
        GameManager.Data.HallDeckSet();
        GameManager.UI.ShowPopup<UI_MyDeck>().Init();
    }

    public void Dictionary()
    {
        SceneChanger.SceneChange("DictionaryScene");
    }

    public void Exit()
    {
        
    }

}

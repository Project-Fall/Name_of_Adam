using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CutSceneController : MonoBehaviour
{
    [SerializeField] private VideoPlayer video;

    private VideoClip videoClip;
    private CutSceneType cutSceneToDisplay;

    private void Start()
    {
        GameManager.Sound.Clear();
        GameManager.Sound.Play("UI/ClickSFX/UIClick2");

        string language = "EN";
        if (GameManager.OutGameData.GetLanguage() == 1)
            language = "KR";
        else if (GameManager.OutGameData.GetLanguage() == 2)
            language = "JA";

        cutSceneToDisplay = GameManager.Data.CutSceneToDisplay;
        videoClip = GameManager.Resource.Load<VideoClip>($"Video/VideoClip/{cutSceneToDisplay}_{language}");

        GameManager.Sound.Play($"CutScene/{cutSceneToDisplay}", Sounds.BGM);
        video.clip = videoClip;
        video.loopPointReached += EndReached;
        video.Play();

        Debug.Log($"{cutSceneToDisplay}_{language} ƒ∆æ¿ Ω√¿€");
    }

    public void SceneChange()
    {
        // ESC∏¶ ƒ“ ªÛ≈¬∂Û∏È ESC∏¶ ≤Ù∞Ì æ¿¿ª ∫Ø∞Ê«’¥œ¥Ÿ.
        if (GameManager.UI.IsOnESCOption)
            GameManager.UI.CloseAllOption();

        if (cutSceneToDisplay == CutSceneType.NPC_Upgrade_Corrupt || cutSceneToDisplay == CutSceneType.NPC_Stigma_Corrupt || cutSceneToDisplay == CutSceneType.NPC_Harlot_Corrupt)
            SceneChanger.SceneChange("EventScene");
        else
            SceneChanger.SceneChange("StageSelectScene");
    }

    public void SkipButton()
    {
        GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");
        EndReached(video);
    }

    private void EndReached(VideoPlayer vp)
    {
#if UNITY_EDITOR
        Debug.Log($"End CutScene: {cutSceneToDisplay}");
#endif
        GameManager.OutGameData.SetCutSceneData(cutSceneToDisplay, true);
        GameManager.Sound.Clear();
        SceneChange();
    }
}

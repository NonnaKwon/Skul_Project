using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class SceneManager : Singleton<SceneManager>
{
    [SerializeField] GameObject loadingText;
    [SerializeField] Image fade;
    [SerializeField] Slider loadingBar;
    [SerializeField] float fadeTime;
    [SerializeField] UI_Story dialog;

    private BaseScene curScene;

    public BaseScene GetCurScene()
    {
        if (curScene == null)
        {
            curScene = FindObjectOfType<BaseScene>();
        }
        return curScene;
    }

    public T GetCurScene<T>() where T : BaseScene
    {
        if (curScene == null)
        {
            curScene = FindObjectOfType<BaseScene>();
        }
        return curScene as T;
    }

    public void LoadScene(Define.Scene scene)
    {
        StartCoroutine(LoadingRoutine(Enum.GetName(typeof(Define.Scene),scene)));
    }

    public void StoryLoad(string id)
    {
        if(dialog.gameObject.activeSelf == false)
            dialog.gameObject.SetActive(true);
        dialog.Load(id);
    }

    public void StoryUnload()
    {
        dialog.Unload();
    }

    public void LoadNextStory(Transform loadTransform)
    {
        StartCoroutine(CoLoadNextStory(loadTransform));
    }

    IEnumerator CoLoadNextStory(Transform loadTransform)
    {
        fade.gameObject.SetActive(true);
        yield return FadeOut(Color.black);

        Manager.Game.Player.transform.position = loadTransform.position;
        FindObjectOfType<BackgroundController>().gameObject.transform.position 
            = new Vector2(loadTransform.position.x, loadTransform.position.y + 11f);

        yield return new WaitForSeconds(1f);
        yield return FadeIn(Color.black);
        fade.gameObject.SetActive(false);
    }


    IEnumerator LoadingRoutine(string scene)
    {
        fade.gameObject.SetActive(true);
        yield return FadeOut(Color.white);
        loadingText.SetActive(true);

        Manager.Pool.ClearPool();
        Manager.Sound.StopSFX();
        Manager.UI.ClearPopUpUI();
        Manager.UI.CloseInGameUI();

        Time.timeScale = 0f;
        loadingBar.gameObject.SetActive(true);

        AsyncOperation oper = UnitySceneManager.LoadSceneAsync(scene);
        while (oper.isDone == false)
        {
            loadingBar.value = oper.progress;
            yield return null;
        }

        Manager.UI.EnsureEventSystem();

        BaseScene curScene = GetCurScene();
        yield return curScene.LoadingRoutine();

        loadingBar.gameObject.SetActive(false);
        Time.timeScale = 1f;

        loadingText.SetActive(false);
        yield return FadeIn(Color.white);
        fade.gameObject.SetActive(false);
    }

    IEnumerator FadeOut(Color color)
    {
        float rate = 0;
        Color fadeOutColor = new Color(color.r, color.g, color.b, 1f);
        Color fadeInColor = new Color(color.r, color.g, color.b, 0f);

        while (rate <= 1)
        {
            rate += Time.deltaTime / fadeTime;
            fade.color = Color.Lerp(fadeInColor, fadeOutColor, rate);
            yield return null;
        }
    }

    IEnumerator FadeIn(Color color)
    {
        float rate = 0;
        Color fadeOutColor = new Color(color.r, color.g, color.b, 1f);
        Color fadeInColor = new Color(color.r, color.g, color.b, 0f);

        while (rate <= 1)
        {
            rate += Time.deltaTime / fadeTime;
            fade.color = Color.Lerp(fadeOutColor, fadeInColor, rate);
            yield return null;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] Canvas popUpCanvas;
    [SerializeField] Canvas windowCanvas;
    [SerializeField] Canvas inGameCanvas;

    [SerializeField] Image popUpBlocker;
    [SerializeField] Button inGameBlocker;

    private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    private float prevTimeScale;
    private UI_Scene curInGameUI;

    private void Start()
    {
        EnsureEventSystem();
    }

    public void EnsureEventSystem()
    {
        if (EventSystem.current != null)
            return;

        EventSystem eventSystem = Resources.Load<EventSystem>("UI/EventSystem");
        Instantiate(eventSystem);
    }

    public T ShowPopUpUI<T>(T popUpUI) where T : UI_Popup
    {
        if (_popupStack.Count > 0)
        {
            UI_Popup topUI = _popupStack.Peek();
            topUI.gameObject.SetActive(false);
        }
        else
        {
            popUpBlocker.gameObject.SetActive(true);
            prevTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        T ui = Instantiate(popUpUI, popUpCanvas.transform);
        _popupStack.Push(ui);
        return ui;
    }

    public void ClosePopupUI()
    {
        UI_Popup ui = _popupStack.Pop();
        Destroy(ui.gameObject);

        if (_popupStack.Count > 0)
        {
            UI_Popup topUI = _popupStack.Peek();
            topUI.gameObject.SetActive(true);
        }
        else
        {
            popUpBlocker.gameObject.SetActive(false);
            Time.timeScale = prevTimeScale;
        }
    }

    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
            return;

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }

        ClosePopupUI();
    }

    public void ClearPopUpUI()
    {
        while (_popupStack.Count > 0)
        {
            ClosePopupUI();
        }
    }



    public T ShowInGameUI<T>(T inGameUI) where T : UI_Scene
    {
        if (curInGameUI != null)
        {
            Destroy(curInGameUI.gameObject);
        }

        T ui = Instantiate(inGameUI, inGameCanvas.transform);
        curInGameUI = ui;
        inGameBlocker.gameObject.SetActive(true);
        return ui;
    }

    public void CloseInGameUI()
    {
        if (curInGameUI == null)
            return;

        inGameBlocker.gameObject.SetActive(false);
        Destroy(curInGameUI.gameObject);
        curInGameUI = null;
    }
}

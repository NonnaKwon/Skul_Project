using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Story : UI_Popup
{
    private string[] _scriptArr;
    private int _index = 0;
    Animator _animator;
    enum Texts
    {
        NameText,
        StoryText
    }

    enum Buttons
    {
        Next
    }

    public override bool Init()
    {
        base.Init();
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.Next).onClick.AddListener(NextScript);
        _animator = GetComponent<Animator>();
        return true;
    }

    public void Load(string id)
    {
        GameScene scene = Manager.Scene.GetCurScene<GameScene>();
        if (scene != null)
            scene.UI_GameScene.gameObject.SetActive(false);

        Manager.Game.Player.StateMachine.ChangeState(Define.PlayerState.Interact);
        _index = 0;
        ScriptData script = ScriptDataList.GetData(id);
        GetText((int)Texts.NameText).text = script.Talker;
        _scriptArr = script.Script.Split('/');
        NextScript();
    }

    public void Unload()
    {
        Debug.Log(_index);
        StartCoroutine(CoUnLoad());
    }

    private void NextScript()
    {
        if (_index >= _scriptArr.Length)
        {
            Unload();
            return;
        }
        GetText((int)Texts.StoryText).text = _scriptArr[_index];
        _index++;
    }
    
    IEnumerator CoUnLoad()
    {
        _animator.Play("DownScript");
        Manager.Game.Player.StateMachine.ChangeState(Define.PlayerState.Idle);
        yield return new WaitForSeconds(0.5f);

        GameScene scene = Manager.Scene.GetCurScene<GameScene>();
        if (scene != null)
            scene.UI_GameScene.gameObject.SetActive(true);
        gameObject.SetActive(false);
        
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameScene : UI_Scene
{
    FightController playerFightData;
    enum Texts
    {
        MaxHp,
        CurrentHp,
        MonsterCount
    }

    enum GameObjects
    {
        HP,
        Head,
        SkillA,
        SkillS,
        CoolTimeA,
        CoolTimeS
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts)); 
        BindObject(typeof(GameObjects));
        GetObject((int)GameObjects.CoolTimeA).SetActive(false);
        GetObject((int)GameObjects.CoolTimeS).SetActive(false);
        return true;
    }

    private void Start()
    {

    }

    public void InitHPBar()
    {
        playerFightData = Manager.Game.Player.gameObject.GetComponent<FightController>();        
        Slider slider = GetObject((int)GameObjects.HP).GetComponent<Slider>();
        Debug.Log(playerFightData);
        slider.maxValue = playerFightData.MaxHp;
        slider.value = playerFightData.MaxHp;

        GetText((int)Texts.MaxHp).text = playerFightData.MaxHp.ToString();
        GetText((int)Texts.CurrentHp).text = playerFightData.MaxHp.ToString();
    }

    public void DecreaseHP(float damage)
    {
        GetObject((int)GameObjects.HP).GetComponent<Slider>().value -= damage;
        GetText((int)Texts.CurrentHp).text = playerFightData.Hp.ToString();
    }


    public void SkillCoolTime(char skill)
    {
        float coolTime = 0;
        Image ui = null;
        switch (skill)
        {
            case 'A':
                coolTime = Manager.Game.Player.CurrentHead.Data.coolTimeA;
                ui = GetObject((int)GameObjects.CoolTimeA).GetComponent<Image>();
                break;
            case 'S':
                coolTime = Manager.Game.Player.CurrentHead.Data.coolTimeS;
                ui = GetObject((int)GameObjects.CoolTimeS).GetComponent<Image>();
                break;
        }
        if (ui.gameObject.activeSelf == true)
            return;
        StartCoroutine(CoSkillCoolTime(coolTime,ui));
    }

    public void MonsterCountUpdate(int count)
    {
        GetText((int)Texts.MonsterCount).text = count.ToString();
    }

    IEnumerator CoSkillCoolTime(float coolTime,Image ui)
    {
        ui.gameObject.SetActive(true);
        Debug.Log(ui);

        float time = coolTime;
        while(time > 0)
        {
            time -= Time.deltaTime;
            ui.fillAmount = time / coolTime;
            yield return new WaitForFixedUpdate();
        }

        ui.gameObject.SetActive(false);
    }
}

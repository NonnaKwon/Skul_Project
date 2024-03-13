using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameScene : UI_Scene
{
    FightController playerFightData;
    enum Texts
    {
        MaxHp,
        CurrentHp
    }

    enum GameObjects
    {
        HP,
        Head,
        Skill
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts)); 
        BindObject(typeof(GameObjects));
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
}

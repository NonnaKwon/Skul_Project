using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_BossMap : UI_Scene
{
    public float TimeUI;

    enum Texts
    {
        hour,
        min,
        sec
    }

    enum GameObjects
    {
        HP
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindObject(typeof(GameObjects));

        return true;
    }



    private void Update()
    {
        int time = (int)TimeUI;
        GetText((int)Texts.sec).text =  (time % 60).ToString("D2");
        GetText((int)Texts.min).text = ((time / 60) % 60).ToString("D2");
        GetText((int)Texts.hour).text = (time / 3600).ToString("D2");
    }

    public void InitHPBar(float maxHp)
    {
        Slider slider = GetObject((int)GameObjects.HP).GetComponent<Slider>();
        slider.maxValue = maxHp;
        slider.value = maxHp;
    }

    public void DecreaseHP(float damage)
    {
        GetObject((int)GameObjects.HP).GetComponent<Slider>().value -= damage;
    }



}

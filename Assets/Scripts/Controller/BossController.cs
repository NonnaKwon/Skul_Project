using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BossController : MonoBehaviour
{

    private int _phase;
    private float _time;
    private float _maxTime;
    private float _minTIme;
    private float[] _hpArr;
    private float _currentHp;
    private float _maxHp;


    void Start()
    {
        _phase = 1;
        _time = 0;
        _maxTime = 5;
        _minTIme = 3;
        _hpArr = new float[BOSS1_PHASE_COUNT] { 60, 40, 30 };
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}

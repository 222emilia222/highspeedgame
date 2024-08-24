using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private playerController pc;
    private GameManager gm;
    private AudioManager am;

    public TMP_Text[] controlTexts;
    public TMP_Text peopleCounter;

    public RectTransform closer;
    [Header("Sliders")]
    public Slider[] sliders;
    public float initSegmentTime;
    public float segmentTime;
    private float _sliderFill;
    [HideInInspector]
    public float SliderFill
    {
        get
        {
            return _sliderFill;
        }
        set
        {
            if (value < initSegmentTime + segmentTime * pc.PeopleNum)
            { 
                if (value < 0) { value = 0; }
                _sliderFill = value; 
            }
        }
    }
    public float allTime;
    [HideInInspector]
    public float timer;

    private void Start()
    {
        pc = FindAnyObjectByType<playerController>();
        gm = GetComponent<GameManager>();
        am = GetComponent<AudioManager>();
        timer = 0;

        for (int i = 0; i < sliders.Length; i++)
        {
            sliders[i].value = 0;
            if (i > 0)
            {
                sliders[i].minValue = initSegmentTime + segmentTime * (i - 1);
                sliders[i].maxValue = initSegmentTime + segmentTime * (i);
            }
            else { sliders[i].minValue = 0; sliders[i].maxValue = initSegmentTime; }
        }
        allTime = initSegmentTime + segmentTime * (sliders.Length-1);

        for (int i = 0; i < controlTexts.Length; i++)
        {
            controlTexts[i].enabled = false;
        }
    }
    private void Update()
    {
        for (int i = 0;i <= pc.PeopleNum; i++)
        {
            if (!pc.crashTimedOut)
            {
                SliderFill += Time.deltaTime;
            }
            sliders[i].value = SliderFill;
        }
    }
}

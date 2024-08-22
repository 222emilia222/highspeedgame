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

    public TMP_Text peopleCounter;
    public RectTransform closer;
    [Header("Sliders")]
    public Slider[] sliders;
    [SerializeField]
    private float initSegmentTime;
    [SerializeField]
    private float segmentTime;
    [HideInInspector]
    public int slideNum = 0;
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
            if (value < initSegmentTime + segmentTime * pc.peopleNum)
            { 
                if (value < 0) { value = 0; }
                _sliderFill = value; 
            }
        }
    }
    public float allTime;

    private void Start()
    {
        pc = FindAnyObjectByType<playerController>();
        gm = GetComponent<GameManager>();
        am = GetComponent<AudioManager>();

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
    }
    private void Update()
    {
        //People Pick Up Counter
        peopleCounter.text = pc.peopleNum + 1 + "/" + gm.maxPplNum + " Bikers";

        //Slider time filler
        for (int i = 0;i <= slideNum; i++)
        {
            if (!pc.crashTimedOut)
            {
                SliderFill += Time.deltaTime;
            }
            sliders[i].value = SliderFill;
        }
    }
    public void MoveCloser()
    {
        //move the closer image
    }
}

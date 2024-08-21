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
    [HideInInspector]
    public float sliderFill = 0;
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
                break;
            }
            else { sliders[i].minValue = 0; sliders[i].maxValue = initSegmentTime; }
        }
        allTime = initSegmentTime + segmentTime * sliders.Length;
    }
    private void Update()
    {
        //People Pick Up Counter
        peopleCounter.text = pc.peopleNum + 1 + "/" + gm.maxPplNum + " Bikers";

        //Slider time filler
        for (int i = 0;i <= slideNum; i++)
        {
            sliderFill += Time.deltaTime;
            sliders[i].value = sliderFill;
        }
    }
    public void MoveCloser()
    {
        //move the closer image
    }
}

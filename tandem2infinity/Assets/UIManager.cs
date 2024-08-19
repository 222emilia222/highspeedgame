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

    public Slider[] sliders;
    [HideInInspector]
    public int slideNum = 0;
    public TMP_Text peopleCounter;
    [HideInInspector]
    public float sliderFill = 0;
    public RectTransform closer;

    private void Start()
    {
        pc = FindAnyObjectByType<playerController>().GetComponent<playerController>();
        gm = GetComponent<GameManager>();
        am = GetComponent<AudioManager>();

        for (int i = 0; i < sliders.Length; i++)
        {
            sliders[i].value = 0;
        }
    }
    private void Update()
    {
        //People Pick Up Counter
        peopleCounter.text = pc.peopleNum + "/" + gm.maxPplNum + " Bikers";

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

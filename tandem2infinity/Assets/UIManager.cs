using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class UIManager : MonoBehaviour
{
    private playerController pc;
    private GameManager gm;
    private AudioManager am;

    public Slider speedSlider;
    public TMP_Text peopleCounter;

    private void Start()
    {
        pc = FindAnyObjectByType<playerController>().GetComponent<playerController>();
        gm = GetComponent<GameManager>();
        am = GetComponent<AudioManager>();
    }
    private void Update()
    {
        //People Pick Up Counter
        peopleCounter.text = pc.peopleNum + "/" + gm.maxPplNum + " Bikers";

        //Slider
        speedSlider.value = pc.currSpeed;
    }
}

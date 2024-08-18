using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class UIManager : MonoBehaviour
{
    private playerController pc;
    private GameManager gm;

    public Slider speedSlider;
    public TMP_Text peopleCounter;
    public int peopleNum = 1;

    private void Start()
    {
        pc = FindAnyObjectByType<playerController>().GetComponent<playerController>();
        gm = FindAnyObjectByType<GameManager>().GetComponent<GameManager>();
    }
    private void Update()
    {
        //People Pick Up Counter
        peopleCounter.text = peopleNum + "/" + gm.maxPplNum + " Bikers";
    }
}

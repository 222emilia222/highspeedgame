using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    private playerController pc;
    private GameManager gm;
    private AudioManager am;

    public TMP_Text[] controlTexts;
    public TMP_Text bikerCounter;

    public Image closer;
    [Header("Sliders")]
    public Image[] segments;
    public Image filler;
    public float initSegmentTime;
    public float segmentTime;
    private float _sliderFill;
    [HideInInspector]
    public float currFill
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
    [HideInInspector]
    public float allTime;
    private float startScale;

    public TMP_Text timer;
    public GameObject placeholderWinScreen;
    private void Start()
    {
        pc = FindAnyObjectByType<playerController>();
        gm = GetComponent<GameManager>();
        am = GetComponent<AudioManager>();
        placeholderWinScreen.SetActive(false);

        filler.fillAmount = 0;
        closer.enabled = false;
        for (int i = 1; i < segments.Length; i++)
        {
            segments[i].enabled = false;
        }
        allTime = initSegmentTime + segmentTime * (segments.Length-1);
        startScale = segments[1].rectTransform.localScale.x;

        for (int i = 0; i < controlTexts.Length; i++)
        {
            controlTexts[i].enabled = false;
        }
        bikerCounter.enabled = false;

        StartCoroutine(IntroText());
    }
    private void FixedUpdate()
    {
        if (!pc.crashTimedOut)
        {
            currFill += Time.fixedDeltaTime;
        }
        filler.fillAmount = currFill / allTime;
    }
    public void NewSegmentUnlock(int i)
    {
        segments[i].enabled = true;
        if (i == 8) { closer.enabled = true; }
        segments[i].rectTransform.localScale *= 3f;
        segments[i].rectTransform.LeanScale(new Vector2(startScale, startScale), 0.5f);
        segments[i].GetComponent<CanvasGroup>().alpha = 0;
        segments[i].GetComponent<CanvasGroup>().LeanAlpha(1, 0.2f);

        StartCoroutine(FlashText(1.8f, bikerCounter));
    }
    public IEnumerator FlashText(float seconds, TMP_Text text)
    {
        text.enabled = true;
        yield return new WaitForSeconds(seconds/2);
        text.GetComponent<CanvasGroup>().LeanAlpha(0, seconds / 2);
        yield return new WaitForSeconds(seconds/2);
        text.enabled = false;
    }
    public IEnumerator IntroText()
    {
        yield return new WaitForSeconds(1.2f);
        controlTexts[0].enabled = true;
        yield return new WaitForSeconds(0.9f);
        controlTexts[0].enabled = false;
        yield return new WaitForSeconds(0.2f);
        controlTexts[1].enabled = true;
        yield return new WaitForSeconds(0.9f);
        controlTexts[1].enabled = false;
        yield return new WaitForSeconds(0.2f);
        controlTexts[2].enabled = true;
        yield return new WaitForSeconds(0.9f);
        controlTexts[2].enabled = false;
        yield return new WaitForSeconds(2f);
        StartCoroutine(FlashText(3f, controlTexts[3]));
    }
}

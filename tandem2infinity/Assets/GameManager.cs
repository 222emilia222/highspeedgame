using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private UIManager um;

    public int maxPplNum;

    public Transform globe;
    [HideInInspector]
    public Vector3 globeCenter;
    [HideInInspector]
    public bool pressedQuit=false;
    private GameObject[] pickups;
    [HideInInspector]
    public TimeSpan finTime;
    private TimeSpan rTime;
    private DateTime startTime;

    private void Start()
    {
        um = GetComponent<UIManager>();
        pickups = GameObject.FindGameObjectsWithTag("Biker");
        globeCenter = globe.position;
        maxPplNum = um.segments.Length;
        startTime = DateTime.Now;
        StartCoroutine(ConsoleLog());
    }
    private IEnumerator ConsoleLog()
    {
        yield return null;
        print("Minimum Time: " + um.allTime + "s; Globeradius: " + globeCenter.magnitude * 2 + "; Biker PickUps: " + (pickups.Length + 1) + "/" + maxPplNum + ";");
    }
    private void Update()
    {
        rTime = DateTime.Now - startTime;

        if (Input.GetKey(KeyCode.Escape))
        {
            StartCoroutine(QuitGame());
        }
    }
    private IEnumerator QuitGame()
    {
        pressedQuit = true;
        yield return new WaitForSeconds(3.5f);
        //show stern32 logo anim
        yield return new WaitForSeconds(1);
        Application.Quit();
        print("Quit Game!");
        Debug.Break();
    }
    public IEnumerator FinishGame()
    {
        finTime = rTime;
        um.timer.text = "Time: " + finTime.ToString(@"mm\:ss\:ff");
        yield return new WaitForSeconds(1.5f);
        Time.timeScale = 0;
        um.placeholderWinScreen.SetActive(true);
    }
}

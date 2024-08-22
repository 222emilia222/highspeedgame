using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private UIManager um;

    public int maxPplNum;

    [SerializeField]
    Transform globe;
    [HideInInspector]
    public Vector3 globeCenter;
    [HideInInspector]
    public bool pressedQuit=false;

    private void Start()
    {
        um = GetComponent<UIManager>();
        globeCenter = globe.position;
        maxPplNum = um.sliders.Length + 1;
        StartCoroutine(ConsoleLog());
    }
    private IEnumerator ConsoleLog()
    {
        yield return null;
        print("Minimum Time: " + um.allTime + "; Globeradius: " + globeCenter.magnitude + ";");
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            //Quit Event
            StartCoroutine(QuitGame());
        }
    }
    private IEnumerator QuitGame()
    {
        pressedQuit = true;
        yield return new WaitForSeconds(3);
        //show stern32 logo
        yield return new WaitForSeconds(1);
        Application.Quit();
        print("Quit Game!");
        Debug.Break();
    }
}

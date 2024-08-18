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

    private void Start()
    {
        globeCenter = globe.position;
    }
}

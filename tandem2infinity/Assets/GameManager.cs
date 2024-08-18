using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public Vector3 globeCenter;
    [SerializeField]
    Transform globe;

    private void Start()
    {
        globeCenter = globe.position;
    }
}

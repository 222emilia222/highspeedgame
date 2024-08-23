using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carController : MonoBehaviour
{
    public GameObject[] wheels;
    public float animSpeed;
    public float stopTime;
    private GameManager gm;
    private bool isMoving;

    private void Start()
    {
        gm = FindAnyObjectByType<GameManager>();
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {


            wheels[0].transform.rotation *= Quaternion.Euler(0, 0, (Time.fixedDeltaTime * animSpeed) % 360);
            wheels[1].transform.rotation *= Quaternion.Euler(0, 0, (Time.fixedDeltaTime * animSpeed) % 360);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isMoving && other.CompareTag("Player"))
        {
            StartCoroutine(StopAndHonk());
        }
    }
    private IEnumerator StopAndHonk()
    {
        isMoving = false;
        //honk sfx
        yield return new WaitForSeconds(stopTime);
        isMoving = true;
    }
}

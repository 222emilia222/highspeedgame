using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carController : MonoBehaviour
{
    private AudioSource audioSrc;
    public GameObject[] wheels;
    public float animSpeed;
    public float speed;
    public float stopTime;
    private GameManager gm;
    private bool isMoving = true;
    private GameObject anchor;
    public AudioClip honk1;

    private void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        gm = FindAnyObjectByType<GameManager>();
        anchor = new GameObject("Car Anchor");
        anchor.transform.SetPositionAndRotation(gm.globe.position, this.transform.rotation);
        transform.SetParent(anchor.transform);
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            anchor.transform.rotation *= Quaternion.Euler((Time.fixedDeltaTime * speed) % 360, 0,0);

            wheels[0].transform.rotation *= Quaternion.Euler((Time.fixedDeltaTime * animSpeed) % 360, 0, 0);
            wheels[1].transform.rotation *= Quaternion.Euler((Time.fixedDeltaTime * animSpeed) % 360, 0, 0);
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
        audioSrc.PlayOneShot(honk1);
        yield return new WaitForSeconds(stopTime);
        isMoving = true;
    }
}

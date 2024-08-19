using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    GameManager gm;
    public float spinSpeed;
    public Transform glowTf;

    private void Start()
    {
        gm = FindAnyObjectByType<GameManager>().GetComponent<GameManager>();
    }
    private void FixedUpdate()
    {
        var normalToCenter = gm.globeCenter - transform.position;
        Quaternion targetRotation = Quaternion.FromToRotation(-transform.up, normalToCenter.normalized) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 200);

        glowTf.rotation *= Quaternion.Euler(0, 0, (Time.fixedDeltaTime * spinSpeed) % 360);
        glowTf.localScale = Vector3.one * Mathf.Lerp(0.8f, 1.25f, (Mathf.Sin(Time.timeSinceLevelLoad) + 1) /2 );

        transform.LookAt(Camera.main.transform);
    }
}

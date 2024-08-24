using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    GameManager gm;
    public float spinSpeed;
    public Transform iconTransform;
    public Transform glowTransform;
    private bool onlyOnce = true;

    private void Start()
    {
        gm = FindAnyObjectByType<GameManager>();
    }
    private void FixedUpdate()
    {
        var normalToCenter = gm.globeCenter - transform.position;
        Quaternion targetRotation = Quaternion.FromToRotation(-transform.up, normalToCenter.normalized) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 200);

        //iconTransform.rotation *= Quaternion.Euler(0, (Time.fixedDeltaTime * spinSpeed) % 360, 0);

        glowTransform.rotation *= Quaternion.Euler(0, 0, (Time.fixedDeltaTime * spinSpeed) % 360);
        glowTransform.localScale = Vector3.one * Mathf.Lerp(0.8f, 1.25f, (Mathf.Sin(Time.timeSinceLevelLoad) + 1) /2 );

        transform.LookAt(Camera.main.transform.parent.parent);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && onlyOnce)
        {
            onlyOnce = false;
            other.GetComponentInParent<playerController>().BikerPickedUp();
            Destroy(this.gameObject);
        }
    }
}

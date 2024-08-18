using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
    GameManager gm;
    Rigidbody rb;

    public float speedMod;
    public float rotMod;
    public float gravityRotAdjust;
    [HideInInspector]
    public float currSpeed;
    float distToCenter;

    float translation;
    float rotation;
    
    void Start()
    {
        gm = FindAnyObjectByType<GameManager>().GetComponent<GameManager>();
        distToCenter = Vector3.Distance(gm.globeCenter, this.transform.position);
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        translation = Input.GetAxis("Vertical") * speedMod;

        rotation = Input.GetAxis("Horizontal") * rotMod;

        var normalToCenter = gm.globeCenter - transform.position;
        var rotationDelta = Quaternion.LookRotation(transform.forward, normalToCenter) * Quaternion.FromToRotation(-transform.up, normalToCenter);

        rb.AddTorque(rotationDelta * -transform.up * gravityRotAdjust * Time.fixedDeltaTime);
        
        rb.AddForce(normalToCenter.normalized * Physics.gravity.magnitude + transform.forward * translation * Time.fixedDeltaTime, ForceMode.Force);
    }
}

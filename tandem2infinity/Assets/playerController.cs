using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
    GameManager gm;
    Rigidbody rb;

    public float currSpeed;

    public float baseSpeedMod;
    public float rotMod;
    public float gravityRotAdjust;
    float distToCenter;

    float rotation;
    
    void Start()
    {
        gm = FindAnyObjectByType<GameManager>().GetComponent<GameManager>();
        rb = GetComponent<Rigidbody>();

    }

    void FixedUpdate()
    {
        //player rotation
        rotation = Input.GetAxis("Horizontal") * rotMod;

        //globe rotation
        distToCenter = Vector3.Distance(gm.globeCenter, transform.position);
        var normalToCenter = gm.globeCenter - transform.position;
        Quaternion targetRotation = Quaternion.FromToRotation(-transform.up, normalToCenter.normalized) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, gravityRotAdjust);
        
        //forward movement
        rb.AddForce(normalToCenter.normalized * Physics.gravity.magnitude + transform.forward * baseSpeedMod * Time.fixedDeltaTime, ForceMode.Force);

        //values for managers
        currSpeed = rb.velocity.magnitude;
    }
}

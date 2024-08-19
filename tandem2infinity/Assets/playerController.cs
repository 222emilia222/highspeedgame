using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
    GameManager gm;
    AudioManager am;
    private UIManager um;
    Rigidbody rb;
    public Collider coll;

    public int peopleNum = 1;

    [HideInInspector]
    public float crashSpeed;
    bool crashTimedOut = false;
    public float crashTime;

    public float speed;
    public float currSpeed;
    public float baseSpeedMod;
    public float rotMod;
    public float gravityRotAdjust;
    float distToCenter;

    float rotation;
    
    void Start()
    {
        gm = FindAnyObjectByType<GameManager>().GetComponent<GameManager>();
        am = FindAnyObjectByType<AudioManager>().GetComponent<AudioManager>();
        um = FindAnyObjectByType<UIManager>().GetComponent<UIManager>();
        rb = GetComponent<Rigidbody>();

    }

    private void Update()
    {
        speed = peopleNum;
    }
    void FixedUpdate()
    {
        //globe rotation
        distToCenter = Vector3.Distance(gm.globeCenter, transform.position);
        var normalToCenter = gm.globeCenter - transform.position;
        Quaternion targetRotation = Quaternion.FromToRotation(-transform.up, normalToCenter.normalized) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, gravityRotAdjust);

        //player rotation
        rotation = Input.GetAxis("Horizontal") * rotMod * Time.fixedDeltaTime;
        rb.AddTorque(new Vector3(0, rotation, 0), ForceMode.Force);

        //forward movement
        if (!crashTimedOut) {
            rb.AddForce(normalToCenter.normalized * Physics.gravity.magnitude + transform.forward * baseSpeedMod * Time.fixedDeltaTime * speed, ForceMode.Force);
        }

        //values for managers
        currSpeed = rb.velocity.magnitude;
    }

    //Crashhandler
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            StartCoroutine(CrashTimeOut());
            crashSpeed = rb.velocity.magnitude;
            rb.velocity = Vector3.zero;
            rb.AddForce(-transform.forward + transform.up, ForceMode.Impulse);
                //fx
                ContactPoint contact = collision.contacts[0];
                Quaternion fxRot = Quaternion.FromToRotation(Vector3.up, contact.normal);
                Vector3 fxPos = contact.point;
            //remove bikers

        }
    }
    IEnumerator CrashTimeOut()
    {
        crashTimedOut = true;
        yield return new WaitForSeconds(crashTime);
        crashTimedOut = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Biker"))
        {
            //New Biker !
            Destroy(other.gameObject);
            peopleNum++;
        }
    }
}

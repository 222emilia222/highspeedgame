using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    public float crashForce;

    public float speed;
    public float currSpeed;
    public float baseSpeedMod;
    public float rotMod;
    public float gravityRotAdjust;
    public float gravMod;

    float rotation;
    
    void Start()
    {
        gm = FindAnyObjectByType<GameManager>().GetComponent<GameManager>();
        am = FindAnyObjectByType<AudioManager>().GetComponent<AudioManager>();
        um = FindAnyObjectByType<UIManager>().GetComponent<UIManager>();
        rb = GetComponent<Rigidbody>();
        speed = peopleNum;

    }
    private void Update()
    {
        if (crashTimedOut) { speed = 0f; }
    }
    void FixedUpdate()
    {
        //globe rotation
        var normalToCenter = gm.globeCenter - transform.position;
        Quaternion targetRotation = Quaternion.FromToRotation(-transform.up, normalToCenter.normalized) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, gravityRotAdjust);

        //player rotation
        rotation = Input.GetAxis("Horizontal") * rotMod * Time.fixedDeltaTime;
        rb.AddTorque(transform.rotation * new Vector3(0, rotation, 0), ForceMode.Force);

        //forward movement
        rb.AddForce(normalToCenter.normalized * Physics.gravity.magnitude * gravMod + transform.forward * baseSpeedMod * Time.fixedDeltaTime * speed, ForceMode.Force);

        //values for managers
        currSpeed = rb.velocity.magnitude;
    }

    //Crashhandler
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            if (!crashTimedOut) { StartCoroutine(CrashTimeOut()); }
            //fx
            ContactPoint contact = collision.contacts[0];
            Quaternion fxRot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 fxPos = contact.point;
        }
    }
    IEnumerator CrashTimeOut()
    {
        crashTimedOut = true;
        crashSpeed = speed;
        rb.velocity = Vector3.zero;
        rb.AddForce((transform.up - transform.forward * 2) * crashForce, ForceMode.Impulse);
        //remove max 2 bikers

        yield return new WaitForSeconds(crashTime);

        crashTimedOut = false;
        speed = crashSpeed;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Biker"))
        {
            //New Biker !
            Destroy(other.gameObject);
            peopleNum++;
            speed *= 1.5f;
            rotMod *= 1.25f;
            //UI
            um.slideNum++;
            um.sliderFill += 5;
            um.sliders[um.slideNum].transform.Find("Border2").gameObject.GetComponent<Image>().color = Color.white;
            
        }
    }
}

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
    public BoxCollider coll;

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

    [Header("Animation")]
    private float animSpeed;
    public GameObject[] wheels = new GameObject[2];
    public GameObject pedals;
    public GameObject frontFrame;
    public GameObject handle;
    
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


        //bike animation
        animSpeed = speed * 100;
        wheels[0].transform.rotation *= Quaternion.Euler(0, 0, (Time.fixedDeltaTime * animSpeed) % 360);
        wheels[1].transform.rotation *= Quaternion.Euler(0, 0, (Time.fixedDeltaTime * animSpeed) % 360);
        pedals.transform.rotation *= Quaternion.Euler(0, 0, (Time.fixedDeltaTime * animSpeed) % 360);

        
    }

    //Crashhandler
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
        if (other.gameObject.CompareTag("Obstacle"))
        {
            if (!crashTimedOut) { StartCoroutine(CrashTimeOut()); }
        }
    }
}

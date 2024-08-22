using Cinemachine;
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

    public int peopleNum;

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
    public GameObject frontFrame;
    public GameObject handle;
    public GameObject[] pedals = new GameObject[9];
    public GameObject[] wheels = new GameObject[2];
    private float animSpeed;

    [Header("Tandem Loop")]
    public GameObject loopFrame;
    public GameObject backFrame;
    public Vector3 loopOffset;
    private GameObject[] loops = new GameObject[9];
    public Color[] shirtColors;

    [Header("Camera")]
    public Vector3[] upgradeAngleChange;
    private Transform cam;

    Quaternion startHandle;
    Quaternion startFrontFrame;

    void Start()
    {
        gm = FindAnyObjectByType<GameManager>();
        am = FindAnyObjectByType<AudioManager>();
        um = FindAnyObjectByType<UIManager>();
        rb = GetComponent<Rigidbody>();
        speed = peopleNum;
        cam = Camera.main.transform;

        peopleNum = 0;

        startHandle = handle.transform.localRotation;
        startFrontFrame = frontFrame.transform.localRotation;
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
        for (int i = 0; i <= peopleNum; i++) { pedals[i].transform.rotation *= Quaternion.Euler(0, 0, (Time.fixedDeltaTime * animSpeed) % 360);}

        float rotAnim = Input.GetAxis("Horizontal");
        handle.transform.localRotation = Quaternion.Lerp(handle.transform.localRotation, startHandle * Quaternion.Euler(0, 0, rotAnim * 30), Time.fixedTime * 2);
        frontFrame.transform.localRotation = Quaternion.Lerp(frontFrame.transform.localRotation, startFrontFrame * Quaternion.Euler(0, 0, rotAnim * 30), Time.fixedTime * 2);
    }

    //CRASHHANDLER
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
        //CRASH CHECK
        if (other.gameObject.CompareTag("Obstacle"))
        {
            if (!crashTimedOut) { StartCoroutine(CrashTimeOut()); }
        }
    }
    public void BikerPickedUp()
    {
        if (!gm.pressedQuit) { 
            //3D Model
            loops[peopleNum] = Instantiate(loopFrame, GetComponentInChildren<BoxCollider>().transform);
            pedals[peopleNum+1] = loops[peopleNum].transform.Find("PedalCenter").gameObject;
            loops[peopleNum].transform.localPosition -= loopOffset * (peopleNum+1);
            backFrame.transform.localPosition -= loopOffset;
            //UI
            um.slideNum++;
            um.sliderFill += 5;
            um.sliders[um.slideNum].transform.Find("Border2").gameObject.GetComponent<Image>().color = Color.white;
            //Camera
            var camController = cam.gameObject.GetComponent<cameraController>();
            camController.moveTo += upgradeAngleChange[peopleNum];
            camController.MoveCam();
            //Parameters
            peopleNum++;
            speed *= 1.5f;
            rotMod *= 1.25f;
        }
    }
}

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

    public float currSpeed;
    private float maxSpeed;
    public float baseSpeedMod;
    public float rotMod;
    public float gravityRotAdjust;
    public float gravMod;

    float rotation;

    [Header("Crash")]
    public float crashTime;
    public float crashForce;
    [SerializeField]
    private float crashPenalty;
    [SerializeField]
    private float pickUpBonus;
    [HideInInspector]
    public float crashSpeed;
    [HideInInspector]
    public bool crashTimedOut = false;

    [Header("Animation")]
    public float animSpeed;
    public GameObject frontFrame;
    public GameObject handle;
    public GameObject pedalCenter;
    private GameObject[] pedals = new GameObject[9];
    public GameObject[] wheels = new GameObject[2];

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
        currSpeed = peopleNum;
        cam = Camera.main.transform;

        peopleNum = 0;

        startHandle = handle.transform.localRotation;
        startFrontFrame = frontFrame.transform.localRotation;
        pedals[0] = pedalCenter;
    }
    private void Update()
    {
        if (crashTimedOut) { currSpeed = 0f; }
    }
    void FixedUpdate()
    {

        //globe rotation & forward movement
        var normalToCenter = gm.globeCenter - transform.position;

        rb.AddForce(normalToCenter.normalized * Physics.gravity.magnitude * gravMod + transform.forward * baseSpeedMod * Time.fixedDeltaTime * currSpeed, ForceMode.Force);

        Quaternion targetRotation = Quaternion.FromToRotation(-transform.up, normalToCenter.normalized) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, gravityRotAdjust);

        //

        //player rotation
        rotation = Input.GetAxis("Horizontal") * rotMod * Time.fixedDeltaTime;
        rb.AddTorque(transform.rotation * new Vector3(0, rotation, 0), ForceMode.Force);

        //bike animation
        wheels[0].transform.rotation *= Quaternion.Euler(0, 0, (Time.fixedDeltaTime * animSpeed * currSpeed) % 360);
        wheels[1].transform.rotation *= Quaternion.Euler(0, 0, (Time.fixedDeltaTime * animSpeed * currSpeed) % 360);
        for (int i = 0; i <= peopleNum; i++) { pedals[i].transform.rotation *= Quaternion.Euler(0, 0, (Time.fixedDeltaTime * animSpeed * currSpeed) % 360);}

        float rotAnim = Input.GetAxis("Horizontal");
        handle.transform.localRotation = Quaternion.Lerp(handle.transform.localRotation, startHandle * Quaternion.Euler(0, 0, rotAnim * 30), Time.fixedTime * 2);
        frontFrame.transform.localRotation = Quaternion.Lerp(frontFrame.transform.localRotation, startFrontFrame * Quaternion.Euler(0, 0, rotAnim * 30), Time.fixedTime * 2);
    }

    //CRASHHANDLER
    IEnumerator CrashTimeOut()
    {
        crashTimedOut = true;
        crashSpeed = currSpeed;
        rb.velocity = Vector3.zero;
        rb.AddForce((transform.up - transform.forward * 2) * crashForce, ForceMode.Impulse);

        um.SliderFill -= crashPenalty;

        yield return new WaitForSeconds(crashTime);

        crashTimedOut = false;
        currSpeed = crashSpeed;
    }
    private void OnCollisionEnter(Collision collision)
    {
        //CRASH CHECK
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (!crashTimedOut) { StartCoroutine(CrashTimeOut()); }
        }
    }
    public void BikerPickedUp()
    {
        if (!gm.pressedQuit) { 
            //3D Model
            loops[peopleNum] = Instantiate(loopFrame, GetComponentInChildren<CapsuleCollider>().transform);
            pedals[peopleNum+1] = loops[peopleNum].transform.Find("PedalCenter").gameObject;
            loops[peopleNum].transform.localPosition -= loopOffset * (peopleNum+1);
            backFrame.transform.localPosition -= loopOffset;
            //UI
            um.slideNum++;
            um.SliderFill += pickUpBonus;
            um.sliders[um.slideNum].transform.Find("Border2").gameObject.GetComponent<Image>().color = Color.white;
            //Camera
            var camController = cam.gameObject.GetComponent<cameraController>();
            camController.moveTo += upgradeAngleChange[peopleNum];
            camController.MoveCam();
            //Parameters
            peopleNum++;
            currSpeed *= 1.5f;
            rotMod *= 1.25f;
        }
    }
}

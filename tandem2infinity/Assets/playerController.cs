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
    public LayerMask layerMask;
    public BoxCollider coll;

    public int PeopleNum
    {
        get { return _peopleNum; }
        set { _peopleNum = value; um.peopleCounter.text = value + 1 + "/" + gm.maxPplNum; }
    }
    private int _peopleNum;

    public float currSpeed;
    private float maxSpeed;
    public float baseSpeedMod;
    public float speedUpgradeMod;
    public float speedOnetimeBonus;
    public float rotMod;
    public float gravityRotAdjust;
    public float gravMod;
    private Vector3 down;

    float rotation;
    private Vector3 slopeDir;
    private Vector3 lastDir;

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
        cam = Camera.main.transform;

        PeopleNum = 0;
        maxSpeed = 1;
        currSpeed = 0.75f;

        lastDir = transform.forward;

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
        //add speed
        if (maxSpeed > currSpeed)
        {
            if (um.SliderFill < um.initSegmentTime) { currSpeed += maxSpeed * Time.fixedDeltaTime / um.initSegmentTime; }
            else { currSpeed += maxSpeed * Time.fixedDeltaTime / um.segmentTime; }
        }

        //globe rotation & forward movement
        var normalToCenter = gm.globeCenter - transform.position;
        //slopeDir = Vector3.ProjectOnPlane(lastDir, slopeHit.normal);
        //transform.rotation = Quaternion.LookRotation(slopeDir);
        //lastDir = slopeDir;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 10, layerMask))
        {
            print("Ground Detected!");
            transform.rotation = Quaternion.LookRotation(transform.forward, hit.normal);
            down = hit.point - transform.position;
        }
        else {
            Quaternion targetRotation = Quaternion.FromToRotation(-transform.up, normalToCenter.normalized) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, gravityRotAdjust);
            down = normalToCenter.normalized;
        }
        rb.AddForce(down * Physics.gravity.magnitude * gravMod + transform.forward * Time.fixedDeltaTime * baseSpeedMod * currSpeed, ForceMode.Force);
        Debug.DrawRay(transform.position, -transform.up * 10, Color.yellow);


        //player rotation
        rotation = Input.GetAxis("Horizontal") * rotMod * Time.fixedDeltaTime;
        rb.AddTorque(transform.rotation * new Vector3(0, rotation, 0), ForceMode.Force);

        //bike animation
        wheels[0].transform.rotation *= Quaternion.Euler(0, 0, (Time.fixedDeltaTime * animSpeed * currSpeed) % 360);
        wheels[1].transform.rotation *= Quaternion.Euler(0, 0, (Time.fixedDeltaTime * animSpeed * currSpeed) % 360);
        for (int i = 0; i <= PeopleNum; i++) { pedals[i].transform.rotation *= Quaternion.Euler(0, 0, (Time.fixedDeltaTime * animSpeed * currSpeed) % 360);}

        float rotAnim = Input.GetAxis("Horizontal");
        handle.transform.localRotation = Quaternion.Lerp(handle.transform.localRotation, startHandle * Quaternion.Euler(0, 0, rotAnim * 30), Time.fixedTime * 2);
        frontFrame.transform.localRotation = Quaternion.Lerp(frontFrame.transform.localRotation, startFrontFrame * Quaternion.Euler(0, 0, rotAnim * 30), Time.fixedTime * 2);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //CRASH CHECK
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (!crashTimedOut) { StartCoroutine(CrashTimeOut()); }
        }
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
    public void BikerPickedUp()
    {
        if (!gm.pressedQuit) { 
            //3D Model
            loops[PeopleNum] = Instantiate(loopFrame, GetComponentInChildren<CapsuleCollider>().transform);
            pedals[PeopleNum+1] = loops[PeopleNum].transform.Find("PedalCenter").gameObject;
            loops[PeopleNum].transform.localPosition -= loopOffset * (PeopleNum+1);
            backFrame.transform.localPosition -= loopOffset;
            //UI
            um.SliderFill += pickUpBonus;
            um.sliders[PeopleNum].transform.Find("Border2").gameObject.GetComponent<Image>().color = Color.white;
            //Camera
            if(PeopleNum != gm.maxPplNum) { 
                var camController = cam.gameObject.GetComponent<cameraController>();
                camController.moveTo += upgradeAngleChange[PeopleNum];
                camController.MoveCam();
            }
            else
            {
                //cam on side
            }
            //Parameters
            PeopleNum++;
            currSpeed += speedOnetimeBonus;
            maxSpeed = currSpeed * speedUpgradeMod;
            rotMod *= 1.25f;
        }
    }
}

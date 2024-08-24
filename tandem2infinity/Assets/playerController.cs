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

        startHandle = handle.transform.localRotation;
        startFrontFrame = frontFrame.transform.localRotation;
        pedals[0] = pedalCenter;
    }
    private void Update()
    {
        //print("PplNum: " + PeopleNum + ";");
    }
    void FixedUpdate()
    {
        if (maxSpeed > currSpeed)
        {
            if (um.currFill < um.initSegmentTime) { currSpeed += maxSpeed * Time.fixedDeltaTime / um.initSegmentTime; }
            else { currSpeed += maxSpeed * Time.fixedDeltaTime / um.segmentTime; }
        }

        var normalToCenter = gm.globeCenter - transform.position;
        Vector3 grav;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 5, layerMask))
        {
            print("Ground Detected!");
            transform.rotation = Quaternion.LookRotation(hit.normal);
            down = hit.point - transform.position;
            down = down.normalized;
            //grav = down * Physics.gravity.magnitude * gravMod;
            grav = Vector3.zero;
        }
        else {
            Quaternion targetRotation = Quaternion.FromToRotation(-transform.up, normalToCenter.normalized) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, gravityRotAdjust);
            down = normalToCenter.normalized;
            grav = down * Physics.gravity.magnitude * gravMod;
        }
        Vector3 mov = transform.forward * Time.fixedDeltaTime * baseSpeedMod * currSpeed;
        rb.velocity = mov + grav;
        Debug.DrawRay(transform.position, -transform.up * 5, Color.yellow);

        rotation = Input.GetAxis("Horizontal") * rotMod * Time.fixedDeltaTime;
        rb.AddTorque(transform.rotation * new Vector3(0, rotation, 0), ForceMode.Force);

        wheels[0].transform.rotation *= Quaternion.Euler(0, 0, (Time.fixedDeltaTime * animSpeed * currSpeed) % 360);
        wheels[1].transform.rotation *= Quaternion.Euler(0, 0, (Time.fixedDeltaTime * animSpeed * currSpeed) % 360);
        for (int i = 0; i <= PeopleNum; i++) { pedals[i].transform.rotation *= Quaternion.Euler(0, 0, (Time.fixedDeltaTime * animSpeed * currSpeed) % 360);}

        float rotAnim = Input.GetAxis("Horizontal");
        handle.transform.localRotation = Quaternion.Lerp(handle.transform.localRotation, startHandle * Quaternion.Euler(0, 0, rotAnim * 30), Time.fixedTime * 2);
        frontFrame.transform.localRotation = Quaternion.Lerp(frontFrame.transform.localRotation, startFrontFrame * Quaternion.Euler(0, 0, rotAnim * 30), Time.fixedTime * 2);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (!crashTimedOut) { StartCoroutine(CrashTimeOut()); }
        }
    }
    IEnumerator CrashTimeOut()
    {
        crashTimedOut = true;
        crashSpeed = currSpeed;
        currSpeed = 0f;
        rb.velocity = Vector3.zero;
        rb.AddForce((transform.up - transform.forward * 2) * crashForce, ForceMode.Impulse);

        um.currFill -= crashPenalty;

        yield return new WaitForSeconds(crashTime);

        crashTimedOut = false;
        currSpeed = crashSpeed;
    }
    public void BikerPickedUp()
    {
        if (!gm.pressedQuit) { 
            loops[PeopleNum] = Instantiate(loopFrame, GetComponentInChildren<CapsuleCollider>().transform);
            pedals[PeopleNum+1] = loops[PeopleNum].transform.Find("PedalCenter").gameObject;
            loops[PeopleNum].transform.localPosition -= loopOffset * (PeopleNum+1);
            backFrame.transform.localPosition -= loopOffset;

            um.currFill += pickUpBonus;
            um.NewSegmentUnlock(PeopleNum + 1);

            if(PeopleNum != gm.maxPplNum) { 
                var camController = cam.gameObject.GetComponent<cameraController>();
                camController.moveTo += upgradeAngleChange[PeopleNum];
                camController.MoveCam();
            }
            else
            {
                //cam on side
            }

            PeopleNum++;
            currSpeed += speedOnetimeBonus;
            maxSpeed = currSpeed * speedUpgradeMod;
            rotMod *= 1.25f;
        }
    }
}

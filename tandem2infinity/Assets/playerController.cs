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
    private Animator[] anims = new Animator[9];

    private bool onlyOnce = true;

    public int PeopleNum
    {
        get { return _peopleNum; }
        set { _peopleNum = value; um.bikerCounter.text = value + 1 + "/" + gm.maxPplNum; }
    }
    private int _peopleNum;

    public float currSpeed;
    private float maxSpeed;
    public float baseSpeedMod;
    public float speedUpgradeMod;
    public float speedOnetimeBonus;
    public float rotMod;
    public float rotAdjust;
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
    public float animSpeed2D;
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
    private float startTime;

    void Start()
    {
        gm = FindAnyObjectByType<GameManager>();
        am = FindAnyObjectByType<AudioManager>();
        um = FindAnyObjectByType<UIManager>();
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
        anims[0] = GetComponentInChildren<Animator>();

        PeopleNum = 0;
        maxSpeed = 1;
        currSpeed = 0.75f;

        startHandle = handle.transform.localRotation;
        startFrontFrame = frontFrame.transform.localRotation;
        pedals[0] = pedalCenter;
        startTime = Time.time;
    }
    private void Update()
    {
        //print("PplNum: " + PeopleNum + ";");
        for (int i = 0; i < anims.Length; i++)
        {
            if (anims[i] == null) { break; }
            anims[i].SetFloat("AnimSpeedMod", currSpeed * animSpeed2D);
        }
    }
    void FixedUpdate()
    {
        if (maxSpeed > currSpeed)
        {
            if (um.currFill < um.initSegmentTime) { currSpeed += maxSpeed * Time.fixedDeltaTime / um.initSegmentTime; }
            else { currSpeed += maxSpeed * Time.fixedDeltaTime / um.segmentTime; }
        }

        rotation = Input.GetAxis("Horizontal") * rotMod * Time.fixedDeltaTime;
        transform.rotation *= Quaternion.Euler(new Vector3(0, rotation, 0));

        var normalToCenter = gm.globeCenter - transform.position;
        Vector3 grav = Vector3.zero;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 5, layerMask))
        {
            //Vector3 normalToSlope = hit.collider.gameObject.transform.position - transform.position;
            //Quaternion targetRotation = Quaternion.FromToRotation(-transform.up, normalToSlope.normalized) * transform.rotation;
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, gravityRotAdjust);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.forward, hit.normal), rotAdjust);

            down = hit.point - transform.position;
            down = down.normalized;
            grav = down * Physics.gravity.magnitude * gravMod / 5;
        }
        else {
            Quaternion targetRotation = Quaternion.FromToRotation(-transform.up, normalToCenter.normalized) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotAdjust);
            down = normalToCenter.normalized;
            grav = down * Physics.gravity.magnitude * gravMod;
        }

        rb.velocity = transform.forward * Time.fixedDeltaTime * baseSpeedMod * currSpeed + grav;
        Debug.DrawRay(transform.position, -transform.up * 5, Color.yellow);

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
            pedals[PeopleNum + 1] = loops[PeopleNum].transform.Find("PedalCenter").gameObject;
            loops[PeopleNum].transform.localPosition -= loopOffset * (PeopleNum + 1);
            backFrame.transform.localPosition -= loopOffset;
            anims[PeopleNum + 1] = loops[PeopleNum].GetComponentInChildren<Animator>();

            um.currFill += pickUpBonus;
            um.NewSegmentUnlock(PeopleNum + 1);

            if (PeopleNum != gm.maxPplNum) {
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

            if(onlyOnce) { um.FlashText(2, um.controlTexts[4]); }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class cameraController : MonoBehaviour
{
    private GameManager gm;
    private Transform player;
    public Transform lookAtTarget;
    public Transform cameraOrigin;
    [HideInInspector]
    public Vector3 moveTo = Vector3.zero;
    [SerializeField]
    private float transSpeed; 
    private void Start()
    {
        gm = FindAnyObjectByType<GameManager>();
        player = transform.parent;
    }

    public void MoveCam()
    {
        StartCoroutine(MoveCamAnim());
    }

    IEnumerator MoveCamAnim()
    {
        float animTime = 0;

        var startVector = transform.localPosition;
        while (animTime <= 1)
        {
            transform.localPosition = Vector3.Lerp(startVector, moveTo, animTime);
            animTime += Time.deltaTime * transSpeed;
            yield return null;
        }

        transform.localPosition = moveTo;
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(lookAtTarget.transform.position - transform.position, player.up);
        if(gm.pressedQuit) { transform.position += (transform.position - moveTo) * Time.deltaTime / 1.5f; }
    }
}

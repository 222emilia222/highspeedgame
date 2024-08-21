using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class cameraController : MonoBehaviour
{
    private Transform player;
    public Transform lookAtTarget;
    public Transform cameraOrigin;
    public Vector3 moveTo = Vector3.zero;
    private void Start()
    {
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
            animTime += Time.deltaTime * 2;
            yield return null;
        }

        transform.localPosition = moveTo;
    }
        

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(lookAtTarget.transform.position - transform.position, player.up);
       // transform.position += (transform.position - moveTo) * Time.deltaTime;
    }
}

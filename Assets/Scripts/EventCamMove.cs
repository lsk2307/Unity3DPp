using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCamMove : MonoBehaviour
{
    public GameObject target;
    public float camMoveSpeed;
    public float shakeTime;

    bool rotateScene;

    float time;

    Vector3 originPos;

    private void Start()
    {
        originPos = transform.position;

        StartCoroutine("Shake");

    }

    private void Update()
    {
        if(!rotateScene)
        {
            time += Time.deltaTime;
        }

        if(time > shakeTime)
        {
            StopCoroutine("Shake");
            rotateScene = true;


            transform.position = new Vector3(-15, 15, 43);
            transform.rotation = Quaternion.Euler(30, 65, 0);

            time = 0;
        }

        if(rotateScene)
        {
            transform.RotateAround(target.transform.position, Vector3.down, camMoveSpeed * Time.deltaTime);
        }
       
        
    }

    IEnumerator Shake()
    {
        transform.localPosition = transform.position + (Vector3)Random.insideUnitCircle * 0.3f;
        yield return null;

        if(!rotateScene)
        {
            transform.position = originPos;
            StartCoroutine("Shake");
        }
    }
}

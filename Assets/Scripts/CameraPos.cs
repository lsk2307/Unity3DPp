﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPos : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    private void Update()
    {
        transform.position = target.position + offset;
    }

    public void ShakingCam()
    {
        StartCoroutine("Shake");
    }

    IEnumerator Shake()
    {
        transform.localPosition = transform.position + (Vector3)Random.insideUnitCircle * 0.5f;
        yield return null;

        transform.position = target.position + offset;

    }
}

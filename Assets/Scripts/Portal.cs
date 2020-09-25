using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public int sceneIndex;
    public GameManager gm;

    private void OnTriggerEnter(Collider other)
    {
        gm.SceneChange(true, sceneIndex);
    }
}

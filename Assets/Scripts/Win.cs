using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Win : MonoBehaviour
{
    public GameObject winUI;
    public Player p;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            p.PlayerStop(true);
            winUI.SetActive(true);

        }
    }
}

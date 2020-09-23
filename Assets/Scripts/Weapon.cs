using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public TrailRenderer trailEffect;
    public int damage;
    public BoxCollider boxCollider;
    public GameObject damageEff;
    public AudioSource audioSwing;
    ParticleSystem pt;
    AudioSource audioS;

    private void Awake()
    {
        audioS = GetComponent<AudioSource>();
    }

    public void swing(float time)
    {
        //휘두르는 이펙트
        StartCoroutine(Effect(time));

        pt = damageEff.GetComponent<ParticleSystem>();
        audioSwing.Play();
    }

    IEnumerator Effect(float time)
    {
        
        trailEffect.enabled = true;
        yield return new WaitForSeconds(time);
        trailEffect.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            pt.Play();
            audioS.Play();
        }
       
    }

}

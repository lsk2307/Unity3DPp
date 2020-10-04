using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public GameObject moveCam;
    public GameObject dust;
    public GameObject player;
    public GameObject uiManager;
    public GameObject wall;
    public GameObject uiHP;
    public GameObject winObj;
    public GameObject[] attackRange;
    public GameObject[] attackBox;
    public GameObject[] attackEffect;
    public AudioSource[] attackSound;
    public JoyStick js;
    public GameManager gm;
    public Image hpBar;

    public int maxHP;
    public int currentHP;

    Rigidbody rigid;
    AudioSource audio;
    NavMeshAgent nav;
    Animation anim;
    SkinnedMeshRenderer mesh;
    BoxCollider bossBox;

    bool bossStart;
    bool stageStart;
    bool bossScene;
    bool isMove;
    bool isAttack;
    bool isDamage;
    bool isDie;
    bool attacking;

    float attackCool;

    float angleRange = 90f;
    float distance = 10f;
    bool isCollision = false;
    float dotValue;

    int attackNum = 0;

    Vector3 direction;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animation>();
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        bossBox = GetComponent<BoxCollider>();

        anim["Damage"].speed = 0.1f;
    }

    private void Update()
    {
        if (!bossScene)
            BossScene();
        else if(bossScene && !isDie)
        {
            if(attackCool > 5 && Vector3.Distance(transform.position, player.transform.position) > 8 && !isAttack)
            {
                attackNum = 2;
            }

            attackCool += Time.deltaTime;

            Chase();
            switch (attackNum)
            {
                case 0:
                    Attack1();
                    break;
                case 1:
                    Attack2();
                    break;
                case 2:
                    Attack3();
                    break;
            }

            if (player.transform.position.y < 4)
            {
                player.transform.position = new Vector3(player.transform.position.x, 4, player.transform.position.z);
            }
        }
       
        
    }

    void BossScene()
    {
        if (!stageStart && !bossStart)
        {
            if (player.transform.position.z > 14)
            {
                Player p = player.GetComponent<Player>();
                p.PlayerStop(true);

                stageStart = true;
                rigid.useGravity = true;
                rigid.AddForce(Vector3.down * 100, ForceMode.Impulse);

                wall.SetActive(true);
                moveCam.SetActive(true);

                js.DragEnd();

                uiManager.SetActive(false);

                gm.AudioSourceChange(1);
                uiHP.SetActive(true);
            }

        }

        if (stageStart && !bossStart)
        {
            if (transform.position.y <= 4)
            {
                dust.SetActive(true);
                audio.Play();

                rigid.isKinematic = true;

                EventCamMove ecm = moveCam.GetComponent<EventCamMove>();
                ecm.enabled = true;
                bossStart = true;

                StartCoroutine("BossSceneEnd");
            }
        }
    }

    IEnumerator BossSceneEnd()
    {
        yield return new WaitForSeconds(5f);

        bossScene = true;
        moveCam.SetActive(false);
        uiManager.SetActive(true);

        Player p = player.GetComponent<Player>();
        p.PlayerStop(false);
    }

    void Chase()
    {
        if (isAttack) return;

        if (Vector3.Distance(transform.position, player.transform.position) <= 8)
        {
            anim.CrossFade("Idle");
            nav.enabled = false;
            isMove = false;
        }

        if(!isMove && Vector3.Distance(transform.position, player.transform.position) > 8)
            MoveStart();

        if(isMove)
        {
            nav.SetDestination(player.transform.position);
        }
    }

    void MoveStart()
    {
        isMove = true;
        anim.Play("Run");
        nav.enabled = true;
    }

    void Attack1()
    {
        if (attackCool < 3f) return;

        if (!isAttack && Vector3.Distance(transform.position, player.transform.position) <= 8)
        {
            nav.enabled = false;
            isMove = false;
            isAttack = true;
            attackRange[0].SetActive(true);
            Vector3 t = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.LookAt(t);
            anim.CrossFade("Idle");
        }

        if(isAttack)
        {
            if (anim.IsPlaying("Idle") && attacking)
            {
                isAttack = false;
                attackRange[0].transform.localScale = new Vector3(1, 1, 0);
                attackRange[0].SetActive(false);
                attackBox[0].SetActive(false);
                attacking = false;
                attackCool = 0;
                attackNum = Random.Range(0, 2);
            }

            if (attackRange[0].transform.localScale.z < 1f && !attacking)
            {
                attackRange[0].transform.localScale += new Vector3(0, 0, 0.05f);
                if (attackRange[0].transform.localScale.z >= 1f)
                    StartCoroutine("Attacking1");
            }

        }
    }

    IEnumerator Attacking1()
    {
        yield return new WaitForSeconds(1f);

        attackBox[0].SetActive(true);
        anim.Play("Attack");
        attackSound[0].Play();
        anim.PlayQueued("Idle");
        attacking = true;
    }

    //private void OnDrawGizmos()
    //{
    //    Handles.color = isCollision ? Color.red : Color.blue;
    //    Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angleRange / 2, distance);
    //    Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angleRange / 2, distance);
    //}

    void Attack2()
    {
        if (attackCool < 3f) return;


        if (!isAttack && Vector3.Distance(transform.position, player.transform.position) <= 8)
        {
            nav.enabled = false;
            isMove = false;
            isAttack = true;
            attackRange[1].SetActive(true);
            Vector3 t = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.LookAt(t);
            anim.CrossFade("Idle");
        }


        if (attacking)
        {
            Player p = player.GetComponent<Player>();
            dotValue = Mathf.Cos(Mathf.Deg2Rad * (angleRange / 2));
            direction = player.transform.position - transform.position;
            if (direction.magnitude < distance)
            {
                if (Vector3.Dot(direction.normalized, transform.forward) > dotValue)
                    p.Damaging(40);
            }
        }

        if (isAttack)
        {
            if (anim.IsPlaying("Idle") && attacking)
            {
                attackRange[1].transform.localScale = new Vector3(0, 1, 0);
                attackRange[1].SetActive(false);
                isAttack = false;
                attacking = false;
                attackEffect[0].SetActive(false);
                attackCool = 0;
                attackNum = Random.Range(0, 2);
            }

            if (attackRange[1].transform.localScale.z < 1f && !attacking)
            {
                attackRange[1].transform.localScale += new Vector3(0.01f, 0, 0.01f);
                if(attackRange[1].transform.localScale.z >= 1f)
                    StartCoroutine("Attacking2");
            }
        }
    }

    IEnumerator Attacking2()
    {
        yield return new WaitForSeconds(1f);

        anim.Play("Damage");
        attackSound[1].Play();
        anim.PlayQueued("Idle");
        attacking = true;
        attackEffect[0].SetActive(true);
    }

    void Attack3()
    {
        if (!isAttack)
        {
            attackEffect[2].SetActive(false);
            nav.enabled = false;
            isMove = false;
            isAttack = true;
            Vector3 t = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.LookAt(t);
            anim.CrossFade("Idle");

            StartCoroutine("Attacking3");
        }

        if (isAttack && attacking && transform.position.y <= 4)
        {
            isAttack = false;
            attackRange[2].transform.localScale = new Vector3(1, 1, 0);
            attackRange[2].SetActive(false);
            attackBox[1].SetActive(false);
            attackEffect[1].SetActive(false);
            attackEffect[2].SetActive(true);
            attacking = false;
            attackCool = 0;
            attackNum = Random.Range(0, 2);
            audio.Play();
            rigid.isKinematic = true;
            bossBox.isTrigger = false;
        }
    }

    IEnumerator Attacking3()
    {
        Vector3 t = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(t);
        yield return new WaitForSeconds(1f);

        attackEffect[1].SetActive(true);
        rigid.isKinematic = false;
        rigid.AddForce(Vector3.up * 100, ForceMode.Impulse);
        attackSound[2].Play();

        yield return new WaitForSeconds(2f);

        attackRange[2].SetActive(true);
        attackRange[2].transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.01f, player.transform.position.z);

        yield return new WaitForSeconds(2f);

        rigid.velocity = Vector3.zero;
        transform.position = new Vector3(attackRange[2].transform.position.x, 30, attackRange[2].transform.position.z);
        rigid.AddForce(Vector3.down * 100, ForceMode.Impulse);
        attackBox[1].SetActive(true);
        attacking = true;
        bossBox.isTrigger = true;

    }

    void ResetBoss()
    {
        currentHP = maxHP;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!isDamage && other.tag == "PlayerAttack")
        {
            Player p = player.GetComponent<Player>();
            Weapon w = other.GetComponent<Weapon>();
            currentHP -= w.damage + p.atk;
            isDamage = true;
           
            hpBar.fillAmount = (float)currentHP / maxHP;

            if (currentHP > 0)
            {
                StartCoroutine("Damaged");
                print("ddd");
            }
            else
            {
                isDie = true;
                attackSound[3].Play();
                StopAllCoroutines();
                isAttack = false;
                gameObject.layer = 12;

                foreach (GameObject range in attackRange)
                {
                    range.SetActive(false);
                }
                foreach (GameObject box in attackBox)
                {
                    box.SetActive(false);
                }
                foreach (GameObject effect in attackEffect)
                {
                    effect.SetActive(false);
                }

                mesh.material.color = Color.gray;
                anim.CrossFade("Death");
                Destroy(gameObject, 4f);
                uiHP.SetActive(false);
                winObj.SetActive(true);
            }

        }
    }

    IEnumerator Damaged()
    {
        mesh.material.color = Color.red;

        yield return new WaitForSeconds(0.05f);

        mesh.material.color = Color.white;
        isDamage = false;
    }
}

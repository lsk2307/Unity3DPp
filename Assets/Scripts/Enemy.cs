using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int maxHp;
    public int currentHp;
    public int moveDistance;
    public float attackDelay;

    float currentTime;
    float damagedAngle;

    public GameObject attackRect;
    public GameObject dropItem;
    public GameObject KillUI;
    public GameObject player;
    Rigidbody rigid;
    BoxCollider boxCollider;
    SkinnedMeshRenderer[] mat;
    NavMeshAgent nav;

    AudioSource damagedAudio;

    Vector3 originPos;
    Quaternion originRot;

    Animator anim;


    bool reset;
    bool isMove;
    bool isAttack;
    bool isAttacking;
    public bool isDamage;
    public bool isDie;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentsInChildren<SkinnedMeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        damagedAudio = GetComponent<AudioSource>();

        originPos = transform.position;
        originRot = transform.rotation;
    }

    void MoveStart()
    {
        //이동 애니메이션
        isMove = true;
        anim.SetBool("doMove", true);
    }

    private void Update()
    {
        //플레이어가 일정 범위 이내에 오면 이동
        if (!reset && !isMove && Vector3.Distance(player.transform.position, transform.position) < moveDistance)
            MoveStart();


        //네비 온
        if (isMove && !isDie)
        {
            if (!nav.enabled)
                nav.enabled = true;
            nav.stoppingDistance = 2;
            nav.SetDestination(player.transform.position);
        }


        //기존 위치에서 일정 거리 이상 갈 시 원위치
        if (Vector3.Distance(originPos, transform.position) > moveDistance * 2)
        {
            reset = true;
            isMove = false;
        }

        //원위치중 설정
        if (reset && !isDie)
        {
            rigid.isKinematic = true;
            if (!nav.enabled)
                nav.enabled = true;
            boxCollider.enabled = false;
            nav.stoppingDistance = 0;
            nav.SetDestination(originPos);
        }

        //원지점 돌아오면 초기화
        if (reset && Vector3.Distance(originPos, transform.position) < 1)
        {
            reset = false;
            nav.enabled = false;
            rigid.isKinematic = false;
            boxCollider.enabled = true;
            currentHp = maxHp;
            anim.SetBool("doMove", false);
            transform.position = originPos;
            transform.rotation = originRot;
        }
    }


    void FreezeVelocity()
    {
        //물리 보정
        if (nav.enabled)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
        
    }

    void Targeting()
    {
        //플레이어가 시선 앞에 있을시 공격

        float targetRadius = 1.5f;
        float targetRange = 2f;

        currentTime += Time.deltaTime;

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

        if(rayHits.Length > 0 && !isAttack && currentTime > attackDelay && !reset)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        transform.LookAt(player.transform.position);
        isMove = false;
        isAttack = true;
        anim.SetBool("doAttack", true);
       
        yield return new WaitForSeconds(0.3f);

        attackRect.SetActive(true);

        yield return new WaitForSeconds(0.2f);
        attackRect.SetActive(false);
        anim.SetBool("doAttack", false);
        isMove = true;
        isAttack = false;
        currentTime = 0;
    }

    private void FixedUpdate()
    {
        if(!isDamage && !isDie)
            Targeting();
        FreezeVelocity();

    }

    private void OnTriggerEnter(Collider other)
    {
        //공격 받음
        if(other.tag == "PlayerAttack" && !reset)
        {
            Weapon weapon = other.GetComponent<Weapon>();
            Player p = player.GetComponent<Player>();
            currentHp -= weapon.damage + p.atk;
            Vector3 reactVec = transform.position - other.transform.position;
            if(!isDie && !isDamage)
            StartCoroutine(OnDamage(reactVec));
        }
    }

    IEnumerator OnDamage(Vector3 reactVec)
    {
        isDamage = true;
        isMove = false;
        nav.enabled = false;

        damagedAudio.Play();

        reactVec = reactVec.normalized;
        reactVec += Vector3.up;

        rigid.AddForce(reactVec * 5, ForceMode.Impulse);

        foreach (SkinnedMeshRenderer mats in mat)
        {
            mats.material.color = Color.red;
        }
        yield return new WaitForSeconds(0.2f);
        
        if(currentHp > 0)
        {
            //빨갛게 만들기
            foreach (SkinnedMeshRenderer mats in mat)
            {
                if(!isAttack)
                anim.SetTrigger("doDamaged");
                mats.material.color = Color.white;
                isDamage = false;
            }
        }
        else
        {
            //죽이기
            isDie = true;
            anim.SetTrigger("doDie");

            foreach (SkinnedMeshRenderer mats in mat)
            {
                
                mats.material.color = Color.gray;
               
            }
            gameObject.layer = 12;
            GameObject instantItem = Instantiate(dropItem, transform.position, transform.rotation);
            Rigidbody itemRigid = instantItem.GetComponent<Rigidbody>();
            itemRigid.AddForce(Vector3.up * 5, ForceMode.Impulse);

            UIManager ui = KillUI.GetComponent<UIManager>();

            ui.MonsterA_KillCount++;

            Destroy(gameObject, 4);
        }

    }
}

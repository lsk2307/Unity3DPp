using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    Rigidbody rigid;
    Vector3 moveVec;
    Animator anim;
    Weapon equipWeapon;
    UIManager uiManager;
    SkinnedMeshRenderer[] meshs;
    AudioSource audio;
    public GameObject[] weapon;
    public GameObject dodgeEffect;
    public GameObject ui;
    public Image HpBar;
    public AudioSource weaponSound;
    public AudioSource getGoldSound;

    int weaponIndex = 0;

    public float speed;
    public int level;
    public int hpMax;
    public int currentHp;
    public int gold;
    public int atk = 5;

    bool allStop;
    bool isDodge;
    public bool isAttack;
    bool isAttacking;
    bool isBorder;
    bool isDamage;
    bool attackButton;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<SkinnedMeshRenderer>();
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {   
        if(!allStop)
        {
            //Dodge();
            //Move();

            if (attackButton)
                AttackButtonDown();

            Attack();
            //Swap();

            //예외처리
            if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")
                || anim.GetCurrentAnimatorStateInfo(0).IsName("Run")) && anim.GetInteger("AttackCount") != 0)
            {
                anim.SetInteger("AttackCount", 0);
                anim.SetTrigger("endAttack");
                isAttack = false;
            }
        }
        else
        {
            //플레이어 정지
            anim.SetInteger("AttackCount", 0);
            anim.SetTrigger("endAttack");
            isAttack = false;
            anim.SetBool("doMove", false);
        }
    }

    public void JoyMove(Vector3 angle)
    {
        transform.eulerAngles = angle;

        if(!isAttack && !isBorder)
        {
            anim.SetBool("doMove", true);
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
    }

    public void MoveAnimOff()
    {
        anim.SetBool("doMove", false);
    }
      
    
    //플레이어 이동
    void Move()
    {

        if (!isDodge)
            moveVec = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        //이동 애니메이션 관리
        if (moveVec != Vector3.zero)
            anim.SetBool("doMove", true);
        else
            anim.SetBool("doMove", false);

        //이동시키기
        if (!isAttack && !isBorder)
            transform.position += moveVec * (isDodge ? speed * 1.5f : speed) * Time.deltaTime;

        //플레이어 이동방향으로 시선처리
        if (!isDodge && !isAttack)
            transform.LookAt(transform.position + moveVec);

    }


    public void Dodge()
    {
        if (!isDodge)
        {
            //공격중에도 사용가능하도록
            if (isAttack)
            {
                isAttack = false;
                StopCoroutine("doAttack");
                weaponSound.Stop();
                audio.Play();
                anim.SetInteger("AttackCount", 0);
                equipWeapon = weapon[weaponIndex].GetComponent<Weapon>();
                equipWeapon.boxCollider.enabled = false;
            }

            rigid.AddForce(Vector3.up * 10, ForceMode.Impulse);
            StartCoroutine(doDodge());
            
        }
    }

    IEnumerator doDodge()
    {
        isDodge = true;
        anim.SetTrigger("doDodge");
        dodgeEffect.SetActive(true);

        yield return new WaitForSeconds(0.4f);
        dodgeEffect.SetActive(false);
        isDodge = false;
    }

    //애니메이션 재생중인지 확인하는 함수
    bool AnimatorIsPlaying()
    {
        return anim.GetCurrentAnimatorStateInfo(0).length >=
               anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public void ButtonAttackDown()
    {
        attackButton = true;
       
    }

    public void ButtonAttackUp()
    {
        attackButton = false;

    }

    void AttackButtonDown()
    {
        //콤보어택 스택
        if (isAttack)
        {
            if ((anim.GetCurrentAnimatorStateInfo(0).IsName("1") && anim.GetInteger("AttackCount") == 0) ||
                (anim.GetCurrentAnimatorStateInfo(0).IsName("2") && anim.GetInteger("AttackCount") == 1) ||
                (anim.GetCurrentAnimatorStateInfo(0).IsName("3") && anim.GetInteger("AttackCount") == 2) ||
                (anim.GetCurrentAnimatorStateInfo(0).IsName("4") && anim.GetInteger("AttackCount") == 2) ||
                (anim.GetCurrentAnimatorStateInfo(0).IsName("5") && anim.GetInteger("AttackCount") == 3))
            {
                anim.SetInteger("AttackCount", anim.GetInteger("AttackCount") + 1);

                if (anim.GetInteger("AttackCount") == 4)
                {
                    anim.SetInteger("AttackCount", 0);
                }

            }
        }

        if (!isDodge && !isAttack)
        {
            isAttack = true;
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit rayHit;
            //if (Physics.Raycast(ray, out rayHit, 100))
            //{
            //    Vector3 nextVec = rayHit.point - transform.position;
            //    nextVec.y = 0;
            //    transform.LookAt(transform.position + nextVec);
            //}


            anim.SetTrigger("doAttack");
            StartCoroutine("doAttack");
        }
    }

    void Attack()
    {
        /*
        //PC버전 마우스클릭
        //어택 스타트
        if (Input.GetMouseButtonDown(0) && !isDodge && !isAttack)
        {
            isAttack = true;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }


            anim.SetTrigger("doAttack");
            StartCoroutine(doAttack());
        }


        //콤보어택 스택
        if (isAttack && Input.GetMouseButton(0))
        {
            if ((anim.GetCurrentAnimatorStateInfo(0).IsName("1") && anim.GetInteger("AttackCount") == 0) ||
                (anim.GetCurrentAnimatorStateInfo(0).IsName("2") && anim.GetInteger("AttackCount") == 1) ||
                (anim.GetCurrentAnimatorStateInfo(0).IsName("3") && anim.GetInteger("AttackCount") == 2) ||
                (anim.GetCurrentAnimatorStateInfo(0).IsName("4") && anim.GetInteger("AttackCount") == 2) ||
                (anim.GetCurrentAnimatorStateInfo(0).IsName("5") && anim.GetInteger("AttackCount") == 3))
            {
                anim.SetInteger("AttackCount", anim.GetInteger("AttackCount") + 1);

                if (anim.GetInteger("AttackCount") == 4)
                {
                    anim.SetInteger("AttackCount", 0);
                }

            }
        }
        */
        
        //콤보어택 실행
        if (!AnimatorIsPlaying() &&
                ((anim.GetCurrentAnimatorStateInfo(0).IsName("1") && anim.GetInteger("AttackCount") != 0) ||
                 (anim.GetCurrentAnimatorStateInfo(0).IsName("2") && anim.GetInteger("AttackCount") != 1) ||
                 (anim.GetCurrentAnimatorStateInfo(0).IsName("4") && anim.GetInteger("AttackCount") != 2) ||
                 (anim.GetCurrentAnimatorStateInfo(0).IsName("5") && anim.GetInteger("AttackCount") != 3)) && !isAttacking && isAttack)
        {
            
            StartCoroutine("doAttack");
        }

        //어택 종료
        if (!AnimatorIsPlaying() &&
                ((anim.GetCurrentAnimatorStateInfo(0).IsName("1") && anim.GetInteger("AttackCount") == 0) ||
                 (anim.GetCurrentAnimatorStateInfo(0).IsName("2") && anim.GetInteger("AttackCount") == 1) ||
                 (anim.GetCurrentAnimatorStateInfo(0).IsName("4") && anim.GetInteger("AttackCount") == 2) ||
                 (anim.GetCurrentAnimatorStateInfo(0).IsName("5") && anim.GetInteger("AttackCount") == 3)) && isAttack)
        {
            anim.SetInteger("AttackCount", 0);
            isAttack = false;
            anim.SetTrigger("endAttack");
        }


       

    }

    IEnumerator doAttack()
    {
        isAttacking = true;
        equipWeapon = weapon[weaponIndex].GetComponent<Weapon>();
        if (anim.GetInteger("AttackCount") == 0)
        {
            yield return new WaitForSeconds(0.3f);
            equipWeapon.swing(0.3f);
            equipWeapon.boxCollider.enabled = true;

            yield return new WaitForSeconds(0.3f);
            equipWeapon.boxCollider.enabled = false;

        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("1"))
        {
            equipWeapon.swing(0.3f);
            equipWeapon.boxCollider.enabled = true;

            yield return new WaitForSeconds(0.3f);
            equipWeapon.boxCollider.enabled = false;
        }
        else if(anim.GetCurrentAnimatorStateInfo(0).IsName("2"))
        {
            yield return new WaitForSeconds(0.4f);
            equipWeapon.swing(0.3f);
            equipWeapon.boxCollider.enabled = true;

            yield return new WaitForSeconds(0.3f);
            equipWeapon.boxCollider.enabled = false;

            yield return new WaitForSeconds(0.3f);
            equipWeapon.swing(0.3f);
            equipWeapon.boxCollider.enabled = true;

            yield return new WaitForSeconds(0.3f);
            equipWeapon.boxCollider.enabled = false;
        }
        else if(anim.GetCurrentAnimatorStateInfo(0).IsName("4"))
        {
            yield return new WaitForSeconds(0.1f);
            equipWeapon.swing(0.5f);

            equipWeapon.boxCollider.enabled = true;

            yield return new WaitForSeconds(0.5f);
            equipWeapon.boxCollider.enabled = false;
        }

        isAttacking = false;
    }

    //무기 변경
    public void Swap()
    {
        /*
        uiManager = ui.GetComponent<UIManager>();
        if (Input.GetButtonDown("swap") && !isAttack && !isDodge)
        {
            if(weaponIndex == 1)
            {
                foreach (GameObject weapons in weapon)
                {
                    weapons.SetActive(false);
                }
                weapon[0].SetActive(true);
                weaponIndex = 0;
                uiManager.WeaponSwap();
            }
            else
            {
                foreach (GameObject weapons in weapon)
                {
                    weapons.SetActive(false);
                }
                weapon[1].SetActive(true);
                weaponIndex = 1;
                uiManager.WeaponSwap();
            } 
        }
        */

        uiManager = ui.GetComponent<UIManager>();
        if (!isAttack && !isDodge)
        {
            if (weaponIndex == 1)
            {
                foreach (GameObject weapons in weapon)
                {
                    weapons.SetActive(false);
                }
                weapon[0].SetActive(true);
                weaponIndex = 0;
                uiManager.WeaponSwap();
                speed *= 2;
            }
            else
            {
                foreach (GameObject weapons in weapon)
                {
                    weapons.SetActive(false);
                }
                weapon[1].SetActive(true);
                weaponIndex = 1;
                uiManager.WeaponSwap();
                speed *= 0.5f;
            }
        }
    }

    //플레이어 체력 감소시키기
    public void Damaged(int damage)
    {
        currentHp -= damage;
    }

    //벽과의 충돌
    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 1, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 1, LayerMask.GetMask("Wall", "Monster"));

    }

    private void FixedUpdate()
    {
        StopToWall();
        //rigid.MovePosition(transform.position + moveVec * (isDodge ? speed * 2f : speed) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //공격 받은 것 처리
        if(other.tag == "EnemyAttack" && !isDamage && !isDodge)
        {
            Attack attack = other.GetComponent<Attack>();
            currentHp -= attack.damage;
            HpBar.fillAmount = (float)currentHp / hpMax;
            StartCoroutine(OnDamage());
        }
        
        //아이템 획득
        if(other.tag == "Item")
        {
            if(other.name == "Gold(Clone)")
            {
                gold += 100;
                other.gameObject.layer = 14;
                Rigidbody goldRG = other.GetComponent<Rigidbody>();
                goldRG.AddForce(Vector3.up * 5, ForceMode.Impulse);
                goldRG.AddTorque(Vector3.up, ForceMode.Impulse);
                getGoldSound.Play();
                Destroy(other.gameObject, 1);
               
            }
        }
    }

    IEnumerator OnDamage()
    {
        isDamage = true;
        rigid.AddRelativeForce(Vector3.back * 10, ForceMode.Impulse);
        rigid.AddForce(Vector3.up * 10, ForceMode.Impulse);
        foreach (SkinnedMeshRenderer mesh in meshs)
        {
            mesh.material.color = new Color32(200, 0, 0, 50);
        }


        yield return new WaitForSeconds(1f);

        foreach (SkinnedMeshRenderer mesh in meshs)
        {
            mesh.material.color = new Color32(195,195,195,255);
        }
        isDamage = false;
    }

    //플레이어 정지 불값
    public void PlayerStop(bool stop)
    {
        allStop = stop;
    }
   
}

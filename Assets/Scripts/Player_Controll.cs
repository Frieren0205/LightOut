using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controll : MonoBehaviour
{
    public LevelManager levelManager;
    public InteractionManager interactionManager;
    public PlayerHP playerHP;
    public Animator animator;
    public GameObject spriteObject;
    private CapsuleCollider body;
    public float JumpPower;
    public Rigidbody rb;
    private Vector3 MoveDirection;
    [SerializeField]
    private float MoveSpeed;
    private  bool isflip; // 좌우 반전을 위해

    public float minLimit;
    public float maxLimit;

    public bool debugtest;

    private bool isPlayerDead;

    [SerializeField]
    private bool isGrounded;
    [Range(0,1)]
    public float raydistance; 



    private bool isCrawl;
    private bool isstuck;

    [Range(0,3)]
    public int ComboCount = 0;
    [SerializeField]
    private float ComboTime;
    [SerializeField,Range(0,0.5f)]
    private float timer;

    bool isHit;
    public bool CanInteraction = false;
    
    public InteractionPoint interactionPoint;
    public GameObject CanInteractionIcon;


    bool CanHit = true;
    [SerializeField]
    bool CanAttack = true;

    public int Movement()
    {
        if(MoveDirection.x > 0)
        {
            return 1;
        }
        else if(MoveDirection.x < 0)
        {
            return -1;
        }
        else
            return 0;
    }
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        body = this.GetComponent<CapsuleCollider>();
        animator = this.gameObject.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isPlayerDead = playerHP.HP_Point <= 0;
        if(isPlayerDead) StartCoroutine(OnPlayerDead());
        // if(!interactionManager.gameManager.isPause)
        // {
            bool hascontrol = (MoveDirection != Vector3.zero);
            if(hascontrol && !isHit && CanAttack)
            {
                transform.Translate(new Vector3(MoveDirection.x,0,MoveDirection.z) * MoveSpeed * Time.deltaTime);
                if(!debugtest)
                {
                if(transform.position.z > minLimit)
                {
                    transform.position = new Vector3(transform.position.x, 0, minLimit);
                }
                if(transform.position.z < maxLimit)
                {
                    transform.position = new Vector3(transform.position.x, 0, maxLimit);
                }
                }

                if(isGrounded && MoveDirection.x != 0 || MoveDirection.z != 0) 
                    animator.SetBool("isMove",true);
                else animator.SetBool("isMove",false);
                OnFlip();
            }
            else if(!hascontrol)        
            {
                animator.SetBool("isMove",false);
            }
            GroundCheck();
            OnCrawl(MoveDirection.y);
            OnJump(MoveDirection.y);


            if(timer <= 0)
            {
                ComboCount = 0;
                animator.SetInteger("AttackCombo", 0);
            }
            else
                timer -= Time.deltaTime;
        // }
    }
    public void OnFlip()
    {
        isflip = MoveDirection.x > 0; // 좌우 반전을 위해
        bool flipStay = MoveDirection.x == 0; // 좌우 반전을 유지하기 위해
        if(isflip && !flipStay)
        {
            isflip = true;
            spriteObject.transform.localScale = new Vector3(1,1,1);
        }
        else if(!isflip && !flipStay)
        {
            isflip = false;
            spriteObject.transform.localScale = new Vector3(-1,1,1);
        }
    }
    public void OnMove(InputAction.CallbackContext value)
    {
        Vector3 input = value.ReadValue<Vector3>();
        if(input != null)
        {
            MoveDirection = new Vector3(input.x,input.y,input.z);
        }

    }
    public void OnJump(float value)
    {
        if(value == 1 && isGrounded && !isCrawl) 
        {
            animator.SetTrigger("isJump");
            animator.SetBool("isGrounded", false);
            rb.AddForce(Vector3.up * JumpPower,ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void GroundCheck()
    {
        Ray ray = new Ray(this.transform.position, Vector3.down);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, raydistance))
        {
            isGrounded = true;
            animator.SetBool("isGrounded",true);
        }
    }
    public void OnCrawl(float value)
    {
        Ray ray = new Ray(this.transform.position + new Vector3(0,0.65f,0),Vector3.up);
        Ray ray2 = new Ray(this.transform.position + new Vector3(-0.3f,0.65f,0), Vector3.up);
        Ray ray3 = new Ray(this.transform.position + new Vector3(0.3f,0.65f,0), Vector3.up);
        RaycastHit hit;
        isstuck = (Physics.Raycast(ray, out hit, 0.01f) || Physics.Raycast(ray2,out hit, 0.01f) || Physics.Raycast(ray3, out hit, 0.01f));
        if(isstuck)
        {
            value = -1;
            isCrawl = true;
            Debug.DrawRay((this.transform.position + new Vector3(0,0.65f,0)),Vector3.up, Color.cyan);
            Debug.DrawRay((this.transform.position + new Vector3(-0.3f,0.65f,0)),Vector3.up, Color.cyan);
            Debug.DrawRay((this.transform.position + new Vector3(0.3f,0.65f,0)),Vector3.up, Color.cyan);
        }
        if(value == -1 && isGrounded)
        {
            //TODO : 포복전진
            isCrawl = true;
            animator.SetBool("isCrawl",true);
            body.center = new Vector3(0,0.345f,0);
            body.direction = 0;
            OnCrawlMovement();
        }
        else if(value == 0 && isGrounded && !isstuck)
        {
            animator.SetBool("isCrawl",false);
            MoveSpeed = 5;
            body.center = new Vector3(0,0.8f,0);
            body.direction = 1;
            isCrawl = false;
        }
    }
    public void OnCrawlMovement()
    {
        //TODO: 포복이동
        MoveSpeed = 1.25f;
        if(MoveDirection.x != 0 || MoveDirection.z != 0)
        {
            Debug.Log("애니메이터 켜기");
            //animator.SetBool("isCrawlMovement",true);
        }
        else
            Debug.Log("애니메이터 끄기");
            //animator.SetBool("isCrawlMovement",false);
    }

    public void OnAttack()
    {
        if(CanAttack)
        {
            animator.SetTrigger("isAttack");
            StartCoroutine(AttackTimer());
        }

    }
    IEnumerator AttackTimer()
    {
        CanAttack = false;
        // 공격 애니메이션 길이만큼 생성
        ComboCount += 1;
        timer = ComboTime;
        animator.SetInteger("AttackCombo", ComboCount);
        yield return new WaitForSecondsRealtime(0.35f);
        if(ComboCount == 3)
        {
            ComboCount = 0;
            animator.SetInteger("AttackCombo", 0);
        }
        CanAttack = true;
    }
    public void OnInteraction()
    {
        if(CanInteraction)
        {
            if(interactionPoint.gameObject != null)
            {
                // Debug.Log(interactionPoint.InteractionData);
                //interactionManager.ChangeEventLog(interactionPoint.InteractionData);
                interactionManager.PopUpUI();
            }
        }
    }

    // 컬라이더 관련 시작!!!
    private void OnCollisionEnter(Collision other) // 몬스터에게 가까이 붙어도 데미지 판정이 들어가도록
    {
        if(other.collider.GetComponent<Enemy_Test2>() && !isHit && CanHit)
        {
            CalculateHit();
        }
    }
    private void OnTriggerEnter(Collider other) 
    {
       /* if(other.gameObject.tag == "Ground")
        {
            isGrounded = true;
            animator.SetBool("isGrounded",true);
        }*/
        if(other.gameObject.tag == "Warp_Damage" && !isHit && CanHit)
        {
            WarpDamage();
        }
    }
    private void OnTriggerExit(Collider other) 
    {/*
        isGrounded = false;
        animator.SetBool("isGrounded",false);*/
    }
    private void OnCollisionStay(Collision other) // 히트 후 몬스터한테 비비고 있어도 데미지 판정이 들어가도록 Stay도 사용
    {
        if(other.collider.GetComponent<Enemy_Test2>() && !isHit && CanHit)
        {
            playerHP.HP_Point -= 1;
            if(isflip)
            {
                rb.AddForce(Vector3.left * 5, ForceMode.Impulse);
            }
            else
                rb.AddForce(Vector3.right * 5, ForceMode.Impulse);
            rb.AddForce(Vector3.up * 7.5f, ForceMode.Impulse);
        }
    }
    // 컬라이더 관련 끝


    private void CalculateHit()
    {
        playerHP.HP_Point -= 1;
        if(isflip)  rb.AddForce(Vector3.left * 2.5f, ForceMode.Impulse);
        else if(!isflip) rb.AddForce(Vector3.right * 2.5f, ForceMode.Impulse);
        rb.AddForce(Vector3.up * 7.5f, ForceMode.Impulse);
        StartCoroutine(OnHit());
        StartCoroutine(Hitable());
    }
    IEnumerator OnPlayerDead()
    {
        yield return new WaitForEndOfFrame();
        animator.SetBool("isDead", true);
        yield return new WaitForSecondsRealtime(2.5f);
        //TODO : 게임오버 UI 팝업
    }
    private void WarpDamage()
    {
        playerHP.HP_Point -= 1;
        rb.AddForce(Vector3.up * 15f, ForceMode.Impulse);
        if(isflip)  rb.AddForce(Vector3.left * 7.5f, ForceMode.Impulse);
        else if(!isflip) rb.AddForce(Vector3.right * 7.5f, ForceMode.Impulse);
        StartCoroutine(OnHit());
        StartCoroutine(Hitable());
    }
    IEnumerator OnHit()
    {
        isHit = true;
        yield return new WaitForSecondsRealtime(0.5f);
        isHit = false;
    }
    IEnumerator Hitable()
    {
        CanHit = false;
        yield return new WaitForSecondsRealtime(1.25f);
        CanHit = true;
    }
}


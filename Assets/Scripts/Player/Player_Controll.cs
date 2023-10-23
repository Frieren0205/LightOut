using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Yarn.Unity;
using Unity.VisualScripting;

public class Player_Controll : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    public LevelManager levelManager;
    public InteractionManager interactionManager;
    public PlayerHP playerHP;
    public Animator animator;
    public GameObject spriteObject;
    
    //이동관련 컴포넌트
    private CapsuleCollider body;
    public float JumpPower;
    public Rigidbody rb;
    [SerializeField]
    private Vector3 MoveDirection;
    
    [Range(5, 6.25f)]
    public float MoveSpeed;
    [SerializeField]
    private  bool isflip; // 좌우 반전을 위해

    
    [Header("맵 이동가능 거리(float)")]
    public float minLimit;
    public float maxLimit;

    [Header("개발용 맵 이동가능 거리 제한없게 하기")]
    public bool debugtest;
    [SerializeField]
    private bool isPlayerDead;
    bool isaleadycheck = false;

    [SerializeField]
    private bool isGrounded;
    [Range(0,1)]
    public float raydistance; 


    /// <summary>
    /// 포복상태 체크, 포복중 위에 장애물이 있는지 체크하기위함
    /// </summary>
    private bool isCrawl;
    private bool isstuck;

    // 전투&상호작용 조작 관련
   [Range(1,5)] // 플레이어 공격력은 초기값 1, 최대 5까지 상승 가능
    public int playerAttackDamage;

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

    [SerializeField]
    private bool isInteractionEnd;

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
    private void Awake() {
        if(gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>().GetComponent<GameManager>();
        }
    }
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        body = this.GetComponent<CapsuleCollider>();
        animator = this.gameObject.GetComponentInChildren<Animator>();
        isflip = true;
    }

    [YarnCommand("EventBoolSet")]
    private void isEventEnd(bool isEnd)
    {
        GameManager.Instance.isPause = isEnd;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        isPlayerDead = playerHP.HP_Point <= 0;
        if(isPlayerDead && !isaleadycheck)
        {
            isaleadycheck = true;
            StartCoroutine(OnPlayerDead());
        }
        if(!interactionManager.gameManager.isPause && !isPlayerDead)
        {
            bool hascontrol = (MoveDirection != Vector3.zero);
            if(hascontrol && !isHit && CanAttack)
            {
                transform.Translate(new Vector3(MoveDirection.x,0,MoveDirection.z) * MoveSpeed * Time.deltaTime);
                if(!debugtest)
                {
                if(transform.position.z > minLimit)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, minLimit);
                }
                if(transform.position.z < maxLimit)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, maxLimit);
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
        }
    }
    public void OnPause()
    {
        UIManager.Instance.OnPause();
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
            animator.SetBool("isGrounded",true);
            isGrounded = true;
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
            MoveSpeed = 1.25f;
        }
        else if(value == 0 && isGrounded && !isstuck)
        {
            animator.SetBool("isCrawl",false);
            body.center = new Vector3(0,0.8f,0);
            body.direction = 1;
            isCrawl = false;
            MoveSpeed = 5f;
        }
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
                CanInteraction = false;
                gameManager.isPause = true;
                if(interactionPoint.indexString != string.Empty && interactionPoint.interactiontype == InteractionPoint.Interactiontype.dialogue)
                {
                    interactionManager.PopUpUI();
                }
                else if(interactionPoint.interactiontype == InteractionPoint.Interactiontype.portal)
                {
                    // Debug.Log("Used Portal method this time");
                    GameManager.Instance.NextSceneLoad();
                }
                else if(interactionPoint.interactiontype == InteractionPoint.Interactiontype.teleport)
                {
                    //TODO : 같은 씬 안에서의 텔레포트
                    UIManager.Instance.castfadeout();
                    Invoke("OnInteraction_teleport", 1.2f);
                }
            }
        }
    }
    private void OnInteraction_teleport()
    {
        transform.position = interactionPoint.transformVec3;
        UIManager.Instance.castfadein();
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
        if(other.gameObject.tag == "EnemyAttack" && !isHit && CanHit)
        {
            CalculateHit();
        }
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
        if(other.collider.tag == "EnemyAttack" && !isHit && CanHit)
        {
            CalculateHit();
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
        animator.SetTrigger("isDead");
        GameManager.Instance.isPlayerDead = true;
        yield return new WaitForSeconds(2.5f);
        UIManager.Instance.OnGameover();
        //TODO : 게임오버 UI 팝업
    }
    private void WarpDamage()
    {
        playerHP.HP_Point -= 1;
        // rb.AddForce(Vector3.up * 17f, ForceMode.Impulse);
        // if(isflip)  rb.AddForce(Vector3.left * 7.5f, ForceMode.Impulse);
        // else if(!isflip) rb.AddForce(Vector3.right * 7.5f, ForceMode.Impulse);

        float Angle = 15;
        Quaternion Rotation = Quaternion.Euler(0,0,Angle);
        if(isflip)
            rb.AddForce(Rotation * Vector3.up * 20f, ForceMode.Impulse);
        else if(!isflip) 
        {
            Rotation = Quaternion.Euler(0,0,-Angle);
            rb.AddForce(Rotation * Vector3.up * 20f, ForceMode.Impulse);
        }
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


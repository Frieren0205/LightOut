using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Yarn.Unity;


public class Player_Controll : MonoBehaviour
{
    public Vector3 direction {get; private set;}
    [SerializeField]
    private GameManager gameManager;
    public LevelManager levelManager;
    public InteractionManager interactionManager;
    public PlayerHP playerHP;
    public Animator animator;
    public GameObject spriteObject;
    
    //이동관련 컴포넌트
    private CapsuleCollider body;
    private bool isJump = false;
    public float JumpPower;
    public Rigidbody rb;
    [SerializeField]
    private Vector3 MoveDirection;

    public bool isStay;
    private RaycastHit slopehit;
    public int maxslope = 50;
    public bool onSlope()
    {
        Ray ray = new Ray(transform.position + rayposition, Vector3.down);
        int slopelayermask = 1 << LayerMask.NameToLayer("Slope");
        Debug.DrawRay(transform.position + rayposition , Vector3.down * 0.3f, Color.red);
        if(Physics.Raycast(ray, out slopehit, 0.3f, slopelayermask))
        {
            var angle = Vector3.Angle(Vector3.up, slopehit.normal);
            return angle != 0f && angle < maxslope;
        }
        return false;
    }
    protected Vector3 DirectionToSlope(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopehit.normal).normalized;
    }
    
    // [Range(5, 6.25f)] 선택지로 올릴 수 있는 수치인데 왜 일케됐지 암튼;
    public float MoveSpeed;
    [Range(0, 1.25f)]
    public float Adventage_Speed = 0;
    [SerializeField]
    private  bool isflip; // 좌우 반전을 위해

    private enum PositionState
    {
        left,
        right
    }
    private PositionState positionState;
    private bool isbackattack;
    
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
    public Vector3 rayposition;
    [Range(0,1)]
    public float raydistance; 


    #region  #끼임 체크
    private bool isCrawl;
    private bool isstuck;
    #endregion

    // 전투&상호작용 조작 관련
   [Range(1,5)] // 플레이어 공격력은 초기값 1, 최대 5까지 상승 가능
    public int playerAttackDamage = 1;

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

    // [SerializeField]
    // private bool isInteractionEnd;

    public GameObject hit_vfx_prepab;

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
            isStay = !hascontrol;
            OnSlope();
            if(hascontrol && !isHit && CanAttack)
            {
                transform.Translate(new Vector3(MoveDirection.x,0,MoveDirection.z) * (MoveSpeed + Adventage_Speed) * Time.deltaTime);
                if(transform.position.z > minLimit && !debugtest)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, minLimit);
                }
                if(transform.position.z < maxLimit && !debugtest)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, maxLimit);
                }
                if(isGrounded && MoveDirection.x != 0 || MoveDirection.z != 0) 
                    animator.SetBool("isMove",true);
                else 
                    animator.SetBool("isMove",false);
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
    public void OnSlope()
    {
        bool isSlope = onSlope();
        Vector3 velocity = isSlope ? DirectionToSlope(direction) : direction;
        Vector3 gravity = Vector3.down * Mathf.Abs(rb.velocity.y);
        if(isSlope && isGrounded)
        {
            velocity = DirectionToSlope(direction);
            gravity = Vector3.zero;
            rb.useGravity = false;
            rb.velocity = velocity * MoveSpeed + gravity;
        }
        else
        {
            rb.useGravity = true;
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
            positionState = PositionState.right;
            spriteObject.transform.localScale = new Vector3(1,1,1);
        }
        else if(!isflip && !flipStay)
        {
            isflip = false;
            positionState = PositionState.left;
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
        if(value == 1 && isGrounded && !isCrawl && !isJump) 
        {
            rb.useGravity = true;
            isJump = true;
            StartCoroutine(Returnjumpcheck());
            animator.SetTrigger("isJump");
            animator.SetBool("isGrounded", false);
            rb.AddForce(Vector3.up * JumpPower,ForceMode.Impulse);
            isGrounded = false;
        }
    }
    IEnumerator Returnjumpcheck()
    {
        if(isJump)
        {
            yield return new WaitForSeconds(0.65f);
            isJump = false;
        }
    }
    private void GroundCheck()
    {
        Ray ray = new Ray(this.transform.position + rayposition, new Vector3(0,-raydistance, 0));
        Debug.DrawRay(this.transform.position + rayposition, new Vector3(0,-raydistance, 0));
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, raydistance) && hit.transform.tag == "Ground")
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
        if(CanInteraction && interactionPoint == null && interactionManager.runner.startAutomatically == false) 
        {
            Debug.LogError("상호 작용을 할 사물을 확인하지 못했습니다.");
            return;
        }
        if(CanInteraction)
        {
            if(interactionPoint.indexString != string.Empty && interactionPoint.interactiontype == InteractionPoint.Interactiontype.dialogue)
            {
                CanInteraction = false;
                gameManager.isPause = true;
                interactionManager.PopUpUI();
            }
            else if(interactionPoint.interactiontype == InteractionPoint.Interactiontype.portal)
            {
                // Debug.Log("Used Portal method this time");
                CanInteraction = false;
                gameManager.isPause = true;
                GameManager.Instance.NextSceneLoad();
            }
            else if(interactionPoint.interactiontype == InteractionPoint.Interactiontype.teleport)
            {
                //TODO : 같은 씬 안에서의 텔레포트
                CanInteraction = false;
                gameManager.isPause = true;
                StartCoroutine(UIManager.Instance.castfadeout());
                Invoke("OnInteraction_teleport", 1f);
            }
            else
                {
                    Debug.LogErrorFormat("상호작용 오브젝트의 정보를 불러올 수 없습니다. 다음을 확인하세요{0}의 {1} {2} {3}",interactionPoint, "interactionPoint.indexString", "interactionPoint.interactiontype", "interactionPoint.transformVec3");
                }
        }
    }

    [YarnCommand("OnInteraction_Security")]
    private void OnInteraction_Security()
    {
        StartCoroutine(UIManager.Instance.castfadeout());
        Invoke("OnInteraction_teleport", 1.5f);
    }
    [YarnCommand("Yarn_Jump")]
    private void Yarn_Jump()
    {
        animator.SetTrigger("isJump");
        animator.SetBool("isGrounded", false);
        rb.AddForce(Vector3.up * JumpPower,ForceMode.Impulse);
        isGrounded = false;
    }
    private void OnInteraction_teleport()
    {
        StartCoroutine(UIManager.Instance.castfadein());
        levelManager.cameraLimitedAreas = interactionPoint.NextCameraConfiner;
        levelManager.CameraAreasUpdate();
        transform.position = interactionPoint.transformVec3;
        gameManager.isPause = false;
    }
    // 컬라이더 관련 시작!!!
    private void OnCollisionEnter(Collision other) // 몬스터에게 가까이 붙어도 데미지 판정이 들어가도록
    {
        var hit_vector = other.contacts[0].point;
        if(other.collider.GetComponent<Enemy_Test2>() && !isHit && CanHit)
        {
            CalculateHit("EnemyAttack",hit_vector);
        }
        if(other.gameObject.tag == "Ground")
        {
            isGrounded = true;
            animator.SetBool("isGrounded",true);
        }
    }
    private void OnTriggerEnter(Collider other) 
    {
        var hit_vector = other.ClosestPoint(transform.position);
        if(other.gameObject.tag == "EnemyAttack" && !isHit && CanHit)
        {
            CalculateHit("EnemyAttack", hit_vector);
        }
        if(other.gameObject.tag == "ArmHammer" && !isHit && CanHit)
        {
            CalculateHit("ArmHammer", hit_vector);
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
    // private void OnCollisionStay(Collision other) // 히트 후 몬스터한테 비비고 있어도 데미지 판정이 들어가도록 Stay도 사용
    // {
    //     if(other.collider.tag == "EnemyAttack" && !isHit && CanHit)
    //     {
    //         BackattackCheck(other.gameObject);
    //         CalculateHit("EnemyAttack");
    //     }
    //     if(other.collider.tag == "ArmHammer" && !isHit && CanHit)
    //     {
    //         CalculateHit("ArmHammer");
    //     }
    // }
    // 컬라이더 관련 끝


    private void CalculateHit(string str , Vector3 hit_pos)
    {
        switch(str)
        {
            case "EnemyAttack":
            {
                playerHP.HP_Point -= 1;
                break;
            }
            case "ArmHammer":
            {
                playerHP.HP_Point = 0;
                break;
            }      
        }
        animator.SetBool("isGrounded", false);
        animator.SetTrigger("isHit");
        Debug.Log(isbackattack);
        if(isflip)  rb.AddForce(Vector3.left * 2.5f, ForceMode.Impulse);
        else if(!isflip) rb.AddForce(Vector3.right * 2.5f, ForceMode.Impulse);
        rb.AddForce(Vector3.up * 7.5f, ForceMode.Impulse);
        StartCoroutine(OnHit(hit_pos));
        StartCoroutine(Hitable());
    }
    private void BackattackCheck(GameObject gameObject)
    {
        if((this.gameObject.transform.position.x - gameObject.transform.position.x) < 0)
        {
            isbackattack = true;
        }
        else
            isbackattack = false;
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
        animator.SetBool("isGrounded", false);
        animator.SetTrigger("isHit");
        // rb.AddForce(Vector3.up * 17f, ForceMode.Impulse);
        // if(isflip)  rb.AddForce(Vector3.left * 7.5f, ForceMode.Impulse);
        // else if(!isflip) rb.AddForce(Vector3.right * 7.5f, ForceMode.Impulse);

        if(isflip)
        {
            rb.AddForce(Vector3.left * 5, ForceMode.Impulse);
        }
        else if(!isflip) 
        {
            rb.AddForce(Vector3.right * 5, ForceMode.Impulse);
        }
        rb.AddForce(Vector3.up * 20f, ForceMode.Impulse);
        StartCoroutine(OnHit(Vector3.zero));
        StartCoroutine(Hitable());
    }
    IEnumerator OnHit(Vector3 hit_pos)
    {
        isHit = true;
        if(hit_pos != Vector3.zero)
        {
            GameObject hit_vfx_clone = Instantiate(hit_vfx_prepab, hit_pos, Quaternion.identity);
            Destroy(hit_vfx_clone, 1f);
        }
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


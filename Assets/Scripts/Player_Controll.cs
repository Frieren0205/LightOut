using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controll : MonoBehaviour
{
    public UIManager uIManager;
    public LevelManager levelManager;
    public PlayerHP playerHP;
    public Animator animator;
    public GameObject spriteObject;
    public float JumpPower;
    public Rigidbody rb;
    [SerializeField]
    private Vector3 MoveDirection;
    public float minLimit;
    public float maxLimit;
    public Vector3 MinMoveLimited;
    public Vector3 MaxMoveLimited;
    [SerializeField]
    private float MoveSpeed;
    private  bool isflip; // 좌우 반전을 위해
    [SerializeField]
    bool isGrounded;
    bool isHit;
    bool CanInteraction = false;
    [SerializeField]
    private GameObject InteractionObject;
    public GameObject CanInteractionIcon;
    bool CanHit = true;
    bool CanAttack = true;
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        animator = this.gameObject.GetComponentInChildren<Animator>().GetComponent<Animator>();
        uIManager = FindObjectOfType<UIManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool hascontrol = (MoveDirection != Vector3.zero);
        if(hascontrol && !isHit)
        {
            //rb.AddForce(new Vector3(MoveDirection.x, 0, MoveDirection.z) * MoveSpeed * Time.deltaTime, ForceMode.);
            transform.Translate(new Vector3(MoveDirection.x,0,MoveDirection.z) * MoveSpeed * Time.deltaTime);
            if(transform.position.z > minLimit)
            {
                transform.position = new Vector3(transform.position.x, 0, minLimit);
            }
            if(transform.position.z < maxLimit)
            {
                transform.position = new Vector3(transform.position.x, 0, maxLimit);
            }

            if(isGrounded) animator.SetBool("isMove",true);
            else animator.SetBool("isMove",false);
            OnFlip();
            OnJump(MoveDirection.y);
        }
        else if(!hascontrol)        
        {
            animator.SetBool("isMove",false);
        }
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
        if(value == 1 && isGrounded) 
        {
            animator.SetTrigger("isJump");
            rb.AddForce(Vector3.up * JumpPower,ForceMode.Impulse);
            isGrounded = false;
        }
    }

    public void OnAttack()
    {
        if(CanAttack)
        {
            StartCoroutine(AttackTimer());
            Debug.Log("공격 입력 확인");
            animator.SetTrigger("isAttack");
        }

    }
    public void OnInteraction()
    {
        if(CanInteraction)
        {
            if(InteractionObject != null)
            {
                Debug.Log(InteractionObject);
            }
        }
    }
    public void OnPause()
    {
        uIManager.OnPause();
    }

    public void ExitPause()
    {
        uIManager.ExitPause();
    }
    // 컬라이더 관련 시작!!!
    private void OnCollisionEnter(Collision other) // 몬스터에게 가까이 붙어도 데미지 판정이 들어가도록
    {
        if(other.collider.GetComponent<Enemy_Test2>() && !isHit && CanHit)
        {
            CalculateHit();
        }
        if(other.collider.name == "Warp_Damage" && !isHit && CanHit)
        {
            WarpDamage();
        }
    }
    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "Ground")
        {
            isGrounded = true;
            animator.SetBool("isGrounded",true);
        }
        if(other.gameObject.tag == "InteractionPosition")
        {
            CanInteraction = true;
            InteractionObject = other.gameObject;
            CanInteractionIcon.SetActive(true);
        }
    }
    private void OnTriggerStay(Collider other) 
    {
        if(other.gameObject.tag == "Ground")
        {
            isGrounded = true;
            animator.SetBool("isGrounded",true);
        }
    }
    private void OnTriggerExit(Collider other) 
    {
        if(other.gameObject.tag == "Ground")
        {
            isGrounded = false;
            animator.SetBool("isGrounded",false);
        }
        if(InteractionObject != null)
        {
            InteractionObject = null;
            CanInteractionIcon.SetActive(false);
        }
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
    private void WarpDamage()
    {
        playerHP.HP_Point -= 1;
        if(isflip)  rb.AddForce(Vector3.left * 5f, ForceMode.Impulse);
        else if(!isflip) rb.AddForce(Vector3.right * 5f, ForceMode.Impulse);
        rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
        StartCoroutine(OnHit());
        StartCoroutine(Hitable());
    }
    IEnumerator AttackTimer()
    {
        CanAttack = false;
        // 공격 애니메이션 길이만큼 생성
        yield return new WaitForSecondsRealtime(0.3f);
        CanAttack = true;
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
        yield return new WaitForSecondsRealtime(2f);
        CanHit = true;
    }
}

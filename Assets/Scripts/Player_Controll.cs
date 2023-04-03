using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controll : MonoBehaviour
{
    public PlayerHP playerHP;
    public Animator animator;
    public GameObject spriteObject;
    public float JumpPower;
    public Rigidbody rb;
    [SerializeField]
    private Vector3 MoveDirection;
    [SerializeField]
    private float MoveSpeed;
    [SerializeField]
    private  bool isflip; // 좌우 반전을 위해
    bool isGrounded;
    bool isHit;
    bool CanHit = true;
    bool CanAttack = true;
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        animator = this.gameObject.GetComponentInChildren<Animator>().GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool hascontrol = (MoveDirection != Vector3.zero);
        if(hascontrol && !isHit)
        {
            //rb.AddForce(new Vector3(MoveDirection.x, 0, MoveDirection.z) * MoveSpeed * Time.deltaTime, ForceMode.);
            transform.Translate(new Vector3(MoveDirection.x,0,MoveDirection.z) * MoveSpeed * Time.deltaTime);
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
    private void OnCollisionExit(Collision other) {
        isGrounded = false;
        animator.SetBool("isGrounded",false);
    }
    private void OnCollisionEnter(Collision other) // 몬스터에게 가까이 붙어도 데미지 판정이 들어가도록
    {
        isGrounded = true;
        animator.SetBool("isGrounded",true);
        if(other.collider.GetComponent<Enemy_Test2>() && !isHit && CanHit)
        {
            CalculateHit();
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
    private void CalculateHit()
    {
        playerHP.HP_Point -= 1;
        if(isflip)  rb.AddForce(Vector3.left * 2.5f, ForceMode.Impulse);
        else if(!isflip) rb.AddForce(Vector3.right * 2.5f, ForceMode.Impulse);
        rb.AddForce(Vector3.up * 7.5f, ForceMode.Impulse);
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

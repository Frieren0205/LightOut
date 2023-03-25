using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controll : MonoBehaviour
{
    public Animator animator;
    public GameObject spriteObject;
    public float JumpPower;
    public Rigidbody rb;
    [SerializeField]
    private Vector3 MoveDirection;
    [SerializeField]
    private float MoveSpeed;

    bool isGrounded;
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        animator = this.gameObject.GetComponentInChildren<Animator>().GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool hascontrol = (MoveDirection != Vector3.zero);
        if(hascontrol)
        {
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
        bool isflip = MoveDirection.x > 0; // 좌우 반전을 위해
        bool flipStay = MoveDirection.x == 0; // 좌우 반전을 유지하기 위해
        if(isflip && !flipStay)
        {
            spriteObject.transform.localScale = new Vector3(1,1,1);
        }
        else if(!isflip && !flipStay) spriteObject.transform.localScale = new Vector3(-1,1,1);
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
    private void OnCollisionEnter(Collision other) 
    {
        isGrounded = true;
        animator.SetBool("isGrounded",true);
    }
}

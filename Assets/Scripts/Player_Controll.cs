using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controll : MonoBehaviour
{
    [SerializeField]
    private Vector3 MoveDirection;
    [SerializeField]
    private float MoveSpeed;
    public float JumpPower;
    public Rigidbody rb;

    bool isGrounded;
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        bool hascontrol = (MoveDirection != Vector3.zero);

        if(hascontrol)
        {
            transform.Translate(new Vector3(MoveDirection.x,0,MoveDirection.z) * MoveSpeed * Time.deltaTime);
            OnFlip();
            OnJump(MoveDirection.y);
        }
    }
    public void OnFlip()
    {
        bool isflip = MoveDirection.x > 0; // 좌우 반전을 위해
        bool flipStay = MoveDirection.x == 0; // 좌우 반전을 유지하기 위해
        if(isflip && !flipStay)
        {
            transform.localScale = new Vector3(-3,3,3);
        }
        else if(!isflip && !flipStay) transform.localScale = new Vector3(3,3,3);
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
            rb.AddForce(Vector3.up * JumpPower,ForceMode.Impulse);
            isGrounded = false;
        }
    }
    private void OnCollisionEnter(Collision other) 
    {
        isGrounded = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent(typeof(Animator))]
public class OnetimeInteractions_Object : MonoBehaviour
{
    public enum TypeofInteraction
    {
        Waypoint,
        fall_bridge
    }
    [Header("상호작용 타입별 분류를 위함")]
    public TypeofInteraction type;
    //TODO : 애니메이터가 들어간 오브젝트들 상호작용 할 용도
    public float animaitingTime;
    private Player_Controll player; 
    // void OnTriggerEnter(Collider other)
    // {
    //     // 플레이어와의 충돌체크를 우선적으로 
    //     if(other.gameObject.GetComponent<Player_Controll>())
    //     {
    //         player = other.gameObject.GetComponent<Player_Controll>();
    //         Debug.Log($"{other.gameObject.name}과 상호작용");
    //         // 상호작용 타입별 분류
    //         switch(type)
    //         {
    //             case TypeofInteraction.Waypoint:
    //             {
    //                 StartCoroutine(OnInteractied());
    //                 break;
    //             }
    //             case TypeofInteraction.fall_bridge:
    //             {

    //                 break;
    //             }
    //         }
    //     }
    // }
    private void OnTriggerExit(Collider other) 
    {
        if(other.gameObject.GetComponent<Player_Controll>())
        {
            player = other.gameObject.GetComponent<Player_Controll>();
            Debug.Log($"{other.gameObject.name}과 상호작용");
            // 상호작용 타입별 분류
            switch(type)
            {
                case TypeofInteraction.fall_bridge:
                {
                    Rigidbody rb = this.GetComponent<Rigidbody>();
                    rb.useGravity = true;
                    Destroy(this.gameObject, 3f);
                    break;
                }
            }
        }    
    }
    IEnumerator OnInteractied()
    {
        Animator animator = this.GetComponent<Animator>();
        animator.SetTrigger("isContact");
        yield return new WaitForSecondsRealtime(animaitingTime);
    }
}

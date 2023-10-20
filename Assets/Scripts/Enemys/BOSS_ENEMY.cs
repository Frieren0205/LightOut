using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;


public class BOSS_ENEMY : MonoBehaviour
{
    [Serializable]
    public struct AttackPattern
    {
        public string pattername;
        public AttackState attackmode;
        public GameObject attackobject;
    }
    private enum positonState
    {
        left,
        right
    }
    public enum AttackState
    {
        Melee,
        Range
    }

    [Header("공격 패턴에 필요한 요소들")]
    public AttackPattern[] attackPattenlist;
    [SerializeField]
    private positonState positonstate;
    private Animator animator;
    private bool isaction_running;
    [SerializeField]
    private bool ismelee_attack;
    private AttackState attackState;
    private Player_Controll player;
    private void OnEnable() 
    {
        animator = this.gameObject.GetComponent<Animator>();
        player = LevelManager.Instance.player;
    }
    private void Start() {
        
    }
    private void FixedUpdate() 
    {
        Rotatetoplayer();
        if(ismelee_attack)
        {
            StartCoroutine(meleeAttackroutine());
        }
    }
    // 플레이어와의 위치 차이에 따라 좌우 회전 (백어택 불가능)
    private void Rotatetoplayer()
    {
        if(!isaction_running)
        {
            bool backattackposition = transform.position.x > player.transform.position.x;
            if(!backattackposition)
            {
                positonstate = positonState.right;
                transform.localScale = new Vector3(1,1,1);
            }
            else
            {
                positonstate = positonState.left;
                transform.localScale = new Vector3(-1,1,1);
            }
        }
    }
    IEnumerator meleeAttackroutine()
    {
        ismelee_attack = false;
        animator.SetTrigger("ismelee_attack");
        yield return new WaitForSeconds(0.1f);
        DOTween.To(() => transform.position.x, x => transform.position = new Vector3(x,transform.position.y,transform.position.z), (transform.position.x + -1),0.05f);
        yield return new WaitForSeconds(0.3f);
        DOTween.To(() => transform.position.x, x => transform.position = new Vector3(x,transform.position.y,transform.position.z), (transform.position.x + -0.25f),0.05f);
    }
}

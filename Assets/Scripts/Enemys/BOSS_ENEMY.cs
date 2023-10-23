using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;


public class BOSS_ENEMY : MonoBehaviour
{
    [Serializable]
    public struct AttackPattern
    {
        public string pattername;
        public AttackState attackmode;
        public GameObject attackobject;
        public Transform attackpoint;
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
    private Rigidbody rb;
    private bool isaction_running;

    private bool ismelee_attack;
    [SerializeField]
    private bool isrange_attack;

    public bool canRandom;
    private List<GameObject> range_obj_list = new List<GameObject>();
    public  List<bool> attack_bool_list = new List<bool>();

    private AttackState attackState;
    private Player_Controll player;
    private static float toPlayerAngle(Vector3 vstart, Vector3 vEnd)
    {
        return Quaternion.FromToRotation(Vector3.up, vEnd - vstart).eulerAngles.z;
    }
    public float Angle;
    private void OnEnable() 
    {
        animator = this.gameObject.GetComponent<Animator>();
        rb = this.gameObject.GetComponent<Rigidbody>();

        player = LevelManager.Instance.player;

        Invoke("StartBattle", 3);
    }
    private void StartBattle()
    {
        canRandom = true;
    }
    private void FixedUpdate() 
    {
        Rotatetoplayer();
        if(canRandom && !ismelee_attack && !isrange_attack) // 랜덤 패턴을 할 수 있고, 근&원거리 공격 전부 하고있지 않을때.
        {
            StartCoroutine(Random_Call());
            AttackPatternChange();
        }
    }
    IEnumerator Random_Call()
    {
        canRandom = false;
        for(int i = 0; i < 5; i++)
        {
            attack_bool_list.Add(false);
            attack_bool_list.Add(false);
        }
        attack_bool_list[Random.Range(0,10)] = true;
        int index = attack_bool_list.FindIndex(a => a == true);
        if(index % 2 == 1)
        {
            Debug.Log("Start melee attack");
            ismelee_attack = true;
            yield return StartCoroutine(meleeAttackroutine());
        }
        else if(index % 2 == 0 && index != 0)
        {
            Debug.Log("Start range attack");
            isrange_attack = true;
            yield return StartCoroutine(rangeAttackroutine());
        }
        else
        {
            Debug.Log("패턴 없음");
        }
        yield return new WaitForSeconds(3);
        canRandom = true;
        attack_bool_list.Clear();
    }
    private void AttackPatternChange()
    {
        switch(attackState)
        {
            case AttackState.Melee :
            {
                break;
            }
            case AttackState.Range :
            {
                break;
            }
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
        attackState = AttackState.Melee;
        animator.SetTrigger("ismelee_attack");
        yield return new WaitForSeconds(0.1f);
        if(positonstate == positonState.right)
        {
            DOTween.To(() => transform.position.x, x => transform.position = new Vector3(x,transform.position.y,transform.position.z), (transform.position.x + 1),0.05f);
        }
        else
            DOTween.To(() => transform.position.x, x => transform.position = new Vector3(x,transform.position.y,transform.position.z), (transform.position.x + -1),0.05f);
        yield return new WaitForSeconds(0.3f);

        if(positonstate == positonState.right)
        {
            DOTween.To(() => transform.position.x, x => transform.position = new Vector3(x,transform.position.y,transform.position.z), (transform.position.x + 0.25f),0.05f);
        }
        else
            DOTween.To(() => transform.position.x, x => transform.position = new Vector3(x,transform.position.y,transform.position.z), (transform.position.x + -0.25f),0.05f);
        ismelee_attack = false;
    }
    IEnumerator rangeAttackroutine()
    {
        attackState = AttackState.Range;
        animator.SetTrigger("isrange_attack");
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.41f);
        Vector3 attackpoint = attackPattenlist[1].attackpoint.position;
        for(int i = 0; i < 3; i++)
        {
            GameObject rangeattack_clone = Instantiate(attackPattenlist[1].attackobject, new Vector3(attackpoint.x, attackpoint.y, this.transform.position.z),Quaternion.identity);
            range_obj_list.Add(rangeattack_clone);
            float random_positon = player.transform.position.x + UnityEngine.Random.Range(-5,5);
            range_obj_list[i].transform.DOMove(new Vector3(random_positon, player.transform.position.y, player.transform.position.z), 1f).SetEase(Ease.Unset);
            Angle = 360f - toPlayerAngle(new Vector3(random_positon,player.transform.position.y, player.transform.position.z), attackPattenlist[1].attackpoint.position);
            range_obj_list[i].transform.rotation = Quaternion.Euler(0,0,-Angle);
        }
        yield return new WaitForSeconds(1f);
        for(int i = 0; i < 3; i++)
        {
            DOTween.Kill(range_obj_list[i]);
        }
        range_obj_list.Clear();
        isrange_attack = false;
    }
}

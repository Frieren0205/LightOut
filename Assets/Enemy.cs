using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public Animator EnemyAnimator;
    public GameObject SpriteObject;
    public Player_Controll target;
    public float Distance; 
    private NavMeshAgent agent;

    // 플레이어와의 거리
    enum State
    {
        idle,
        Chase,
        Move,
        Attack
    }
    State state;
    // Start is called before the first frame update
    void Start()
    {
        agent = this.gameObject.GetComponent<NavMeshAgent>();
        target = GameObject.FindObjectOfType<Player_Controll>();
    }

    // Update is called once per frame
    void Update()
    {
        Distance = Vector3.Distance(target.transform.position, this.transform.position);
        bool isfilp = 0 <= (target.transform.position.x - this.transform.position.x);
        if(isfilp)
        {
            SpriteObject.transform.localScale = new Vector3(1,1,1);
            if(Distance <= 1)
            {
                FlipSetting();
            }
            else
                agent.isStopped = false;
        }
        else
        {
            SpriteObject.transform.localScale = new Vector3(-1,1,1);
            if(Distance <= 1)
            {
                FlipSetting();
            }
            else
                agent.isStopped = false;
        }

        // 대기 상태인 상황에서, 거리가 5 이하일 경우
        if(state == State.idle || state == State.Chase && Distance <= 5)
        {
            state = State.Chase;
            agent.destination = target.transform.position;
            ToChase();
        }
    }

    private void FlipSetting()
    {
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
    }

    private void ToChase()
    {
        //TODO 추적 상태로 전환
        Debug.Log("추적 상태 돌입");
        EnemyAnimator.SetTrigger("Move");
        if(state == State.Chase)
        {
            if(Distance <= 1)
            {
                state = State.Attack;
                //ToAttack();
            }
        }
    }

    private void ToAttack()
    {
        throw new NotImplementedException();
    }
}

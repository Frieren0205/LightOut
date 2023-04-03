using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Test2 : MonoBehaviour
{
    public enum State // 적 개체 상태머신
    {
        idle,
        Chase,
        Attak,
    }
    // 적 스크립트 구조 관련
    private EnemySight sight; // 시야 스크립트
    public Animator animator; // 적 스프라이트 애니메이터
    public GameObject SpriteObject; // 분리한 스프라이트 오브젝트
    public State state; // 상태머신 변수화
    private Rigidbody rb; // 리지드바디

    // 움직임 및 추적 관련
    public float MovementSpeed = 5;
    private float PathRefreshTime = 0.0f;
    private float WayPointsArrivalDistance = 1.5f;

    [SerializeField]
    private int currentWayPointIndex = 0;
    public NavMeshPath path;
    public Transform target_Transform;
    public Vector3[] WayPoints;

    // 공격 관련

    private float AttackDistance = 1.5f;

    void Start()
    {
        path = new NavMeshPath();
        animator = this.GetComponent<Animator>();
        sight = this.gameObject.GetComponentInChildren<EnemySight>();
        target_Transform = FindObjectOfType<Player_Controll>().transform;
        state = State.idle;
    }

    void FixedUpdate()
    {
       if(state == State.idle)
       {
            OnMoveStop();
       }
       if(state == State.Chase)
       {
            MovementSpeed = 2.5f;
       }
    }
    public void UpdateFollwingPath()
    {
        this.UpdateFollwingPath_Navigate();
        //this.UpdateFollwingPath_Follow();
    }

    private void UpdateFollwingPath_Follow()
    {
       
    }
    private void UpdateSight()
    {
        this.UpdateSight_FindPlayer();
    }

    private void UpdateSight_FindPlayer()
    {
        throw new NotImplementedException();
    }

    bool IsWayPointArrived(Vector3 currentWayPoint)
    {
        return Vector3.Distance(transform.position, currentWayPoint) <= WayPointsArrivalDistance;
    }
    public void UpdateFollwingPath_Navigate()
    {
       // 갱신 주기 
       PathRefreshTime += Time.deltaTime;
       if(PathRefreshTime >= 0.0025f)
       {
            // 주기 리셋
            PathRefreshTime = 0.0f;
            // 경로 재계산
            NavMesh.CalculatePath(transform.position, target_Transform.position, NavMesh.AllAreas, path);

            // 각 코너까지의 라인을 디버깅하여 그림
            for(int i = 0; i < path.corners.Length -1; i++)
            {
                Debug.DrawLine(path.corners[i], path.corners[i+1], Color.red);
            }
            if(path.status == NavMeshPathStatus.PathComplete)
            {
                WayPoints = path.corners;
                state = State.Chase;
            }
            else if(path.status == NavMeshPathStatus.PathPartial) 
            {
                Debug.Log("안돼~");
                OnMoveStop();
                WayPoints = null;
                currentWayPointIndex = 0;
            }
            else 
                WayPoints = null;
                currentWayPointIndex = 0;
       }
       if(WayPoints != null && currentWayPointIndex < path.corners.Length)
       {
            Vector3 currentWayPoint = WayPoints[currentWayPointIndex];
            if(IsWayPointArrived(currentWayPoint))
            {
                currentWayPointIndex++;
                {
                    if(currentWayPointIndex >= path.corners.Length)
                    {
                        //TODO 도착확인
                    }
                    else
                        currentWayPoint = WayPoints[currentWayPointIndex];
                }
                UpdateFollwingPath_Navigate_OnMove();
            }
       }
    }
    public void UpdateFollwingPath_Navigate_OnMove()
    {
        //TODO 웨이포인트로 움직이는 로직
        if(state == State.Chase)
        {
            animator.SetBool("Move",true);
            bool isfilp = 0 <= (target_Transform.position.x - this.transform.position.x);
            if(isfilp)
            {
                SpriteObject.transform.localScale = new Vector3(1,1,1);
            }
            else
                SpriteObject.transform.localScale = new Vector3(-1,1,1);
            transform.position = Vector3.MoveTowards(transform.position, WayPoints[currentWayPointIndex], MovementSpeed * Time.deltaTime);
            if(Vector3.Distance(transform.position, target_Transform.position) <= AttackDistance)
            {
                state = State.Attak;
                OnAttack();
            }
        }
    }

    public void OnAttack()
    {
        MovementSpeed = 0;
    }
    public void OnMoveStop()
    {
        state = State.idle;
        animator.SetBool("Move", false);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Test2 : MonoBehaviour
{
    public enum State
    {
        idle,
        Chase,
    }
    public State state;
    public Animator animator;
    public Transform target_Transform;
    private Rigidbody rb;
    private EnemySight sight; 
    public GameObject SpriteObject;

    private NavMeshPath path;
    private float PathRefreshTime = 0.0f;
    public float MovementSpeed = 5;

    public Vector3[] WayPoints;
    [SerializeField]
    private int currentWayPointIndex = 0;
    private float WayPointsArrivalDistance = 1.5f;

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
       if(state == State.Chase)
       {
            state = State.idle;
            OnMoveStop();
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
        }
        bool isfilp = 0 <= (target_Transform.position.x - this.transform.position.x);
        if(isfilp)
        {
            SpriteObject.transform.localScale = new Vector3(1,1,1);
        }
        else
            SpriteObject.transform.localScale = new Vector3(-1,1,1);
        transform.position = Vector3.MoveTowards(transform.position, WayPoints[currentWayPointIndex], MovementSpeed * Time.deltaTime);
    }
    public void OnMoveStop()
    {
        animator.SetBool("Move", false);
    }
}

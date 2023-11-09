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
        hit,
        Chase,
        Attak,
        Emergency,
        Dead
    }
    public bool isdo_something;
    [Range(0,5)]
    public int EnemyHP;
    bool isattakable = true;
    bool ishitable = true;
    public bool isfilp;

    bool leftbackattack;
    bool rightbackattack;

    bool isplayerdead = false;
    private Rigidbody Rb;

    // 적 스크립트 구조 관련
    private EnemySight sight; // 시야 스크립트
    [SerializeField]
    // 발전기 지키기 메소드 연결
    private Generator generator;
    private List<Generator> Generators;
    [SerializeField]
    private int generatorRange; // 발전기 인식 범위
    private float shortDis;
    public Animator animator; // 적 스프라이트 애니메이터
    public GameObject SpriteObject; // 분리한 스프라이트 오브젝트
    public State state; // 상태머신 변수화
    // 움직임 및 추적 관련
    public float MovementSpeed = 1;
    private float PathRefreshTime = 0.0f;
    private float WayPointsArrivalDistance = 1.5f;

    [SerializeField]
    private int currentWayPointIndex = 0;
    public NavMeshPath path;
    public Transform target_Transform;
    [SerializeField]
    private GameObject playersprite;
    private Vector3 Attackpoint;
    public Vector3[] WayPoints;

    // 공격 관련
    [SerializeField]
    private float AttackDistance = 1.5f;

    void Start()
    {
        path = new NavMeshPath();
        animator = this.GetComponentInChildren<Animator>();
        sight = this.gameObject.GetComponentInChildren<EnemySight>();
        target_Transform = FindObjectOfType<Player_Controll>().transform;
        Rb = this.gameObject.GetComponent<Rigidbody>();
        playersprite =  target_Transform.GetComponentInChildren<Animator>().gameObject;
        state = State.idle;
       // FindGenerator();
    }

    public void FindGenerator()
    {
        Generators = new List<Generator>(GameObject.FindObjectsOfType<Generator>());
        shortDis = Vector3.Distance(gameObject.transform.position, Generators[0].transform.position);
        if(shortDis <= generatorRange)
        {
            generator = Generators[0];

            foreach (Generator minGenerator in Generators)
            {
                float Distance = Vector3.Distance(gameObject.transform.position, minGenerator.transform.position);

                if(Distance < shortDis)
                {
                    generator = minGenerator;
                    shortDis = Distance;
                }
            }
            //Debug.Log(generator);
            //Debug.DrawLine(this.transform.position, generator.transform.position, Color.blue, 10f);
        }
        else
            generator = null;
    }
    private void CheckingBackAttack()
    {
        // 왼쪽 오른쪽 백어택을 구분해야함
        // isflip = 오른쪽을 보고 있음
        // !isflip = 왼쪽을 보고있음

        // 오른쪽을 보고 있을때의 백어택 조건은, 플레이어의 위치가 현재 몹의 위치보다 -여야함.
        // 따라서 왼쪽 백어택 조건은, 오른쪽을 보고 있을 때 몬스터의 좌표값이 더 클 경우

        // 왼쪽을 보고 있을때의 백어택 조건은, 플레이어의 위치가 더 커야함
        // 따라서 오른쪽 백어택 조건은, 왼쪽을 보고있을때의 플레이어의 좌표값이 더 클 경우다.
        leftbackattack = isfilp && this.transform.position.x > target_Transform.position.x;
        rightbackattack = !isfilp && target_Transform.position.x > this.transform.position.x;
    }

    void FixedUpdate()
    {
        isplayerdead = GameManager.Instance.isPlayerDead;
        isdo_something = !isattakable || !ishitable;
        if(isplayerdead)
        {
            StopAllCoroutines();
        }
        if(EnemyHP <= 0 && state != State.Dead)
        {
            StartCoroutine(deadroutine());
        }
        if(state == State.idle || isplayerdead)
        {
            OnMoveStop();
        }
        if(state == State.Chase && !isplayerdead)
        {
            MovementSpeed = 1;
            
        }
    }
    public void UpdateFollwingPath()
    {
        if(state != State.Dead)
        this.UpdateFollwingPath_Navigate();
    }
    bool IsWayPointArrived(Vector3 currentWayPoint)
    {
        return Vector3.Distance(transform.position, currentWayPoint) <= WayPointsArrivalDistance;
    }
    public void UpdateFollwingPath_Navigate()
    {
       // 갱신 주기 
       PathRefreshTime += Time.deltaTime;
       if(PathRefreshTime >= 0.00025f)
       {
            // 주기 리셋
            PathRefreshTime = 0.0f;
            // 경로 재계산
            // bool playerisflip = playersprite.transform.localScale == new Vector3(-1,1,1);
            // Debug.Log(playerisflip);
            Attackpoint = new Vector3(target_Transform.position.x+1,target_Transform.position.y,target_Transform.position.z);
            NavMesh.CalculatePath(transform.position, Attackpoint, NavMesh.AllAreas, path);

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
                        currentWayPointIndex = 0;
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
        if(!isdo_something && state == State.Chase && state != State.Dead || !isplayerdead)
        {
            animator.SetBool("Move",true);
            isfilp = target_Transform.position.x > this.transform.position.x;
            if(isfilp)
            {
                SpriteObject.transform.localScale = new Vector3(1,1,1);
            }
            else
                SpriteObject.transform.localScale = new Vector3(-1,1,1);
            transform.position = Vector3.MoveTowards(transform.position, WayPoints[currentWayPointIndex], MovementSpeed * Time.deltaTime);
            if(Math.Round(Vector3.Distance(transform.position, Attackpoint),2) <= AttackDistance)
            {
                state = State.Attak;
                OnAttack();
            }
        }
    }
    public void OnAttack()
    {
        MovementSpeed = 0;
        animator.SetBool("Move", false);
        if(isattakable)
        {
            StartCoroutine(AttackRoutine());
        }
    }
    IEnumerator AttackRoutine()
    {
        isattakable = false;
        animator.SetTrigger("isAttack");
        yield return new WaitForSeconds(1.5f);
        isattakable = true;
    }
    public void OnMoveStop()
    {
        state = State.idle;
        animator.SetBool("Move", false);
    }
    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.name == "Attack_Col" && ishitable)
        {
            StartCoroutine(hitroutine());
        }
    }
    private IEnumerator hitroutine()
    {
        state = State.hit;
        ishitable = false;
        EnemyHP -= 1;
        animator.SetTrigger("isHit");
        if(isfilp)
        {
            Rb.AddForce(Vector3.left * 5,ForceMode.Impulse);
        }
        else
        {
            Rb.AddForce(Vector3.right * 5, ForceMode.Impulse);
        }
        Rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
        yield return new WaitForSeconds(0.25f);
        ishitable = true;
    }
    private IEnumerator deadroutine()
    {
        state = State.Dead;
        ishitable = false;
        animator.SetBool("isDead", true);
        yield return new WaitForSeconds(1);
        LevelManager.Instance.isLevel1Clear = true;
        Destroy(gameObject);
    }
    // public void OnEmergency()
    // {
    //     // 이동속도 2배, 그 외 무언가
    //     state = State.Emergency;
    //     MovementSpeed = 5;

    //     if(!generator.gameObject.activeInHierarchy)
    //     {
    //         StartCoroutine(OnGeneratorDestory());
    //     }
    // // }
    // IEnumerator OnGeneratorDestory()
    // {
    //     yield return new WaitForSeconds(1.5f);
    //     this.gameObject.SetActive(false);
    // }
}

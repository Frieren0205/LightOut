using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Cinemachine;
using UnityEngine.AI;

public class BOSS_ENEMY : MonoBehaviour
{
    public enum State
    {
        idle,
        Chase,
        Attack,
        Dead
    }
    public State state;
    [Header("보스 체력"), Range(0,20)]
    public int Boss_HP;
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
        Range,
        Arm_Hammer
    }

    [Header("공격 패턴에 필요한 요소들")]
    public AttackPattern[] attackPattenlist;
    [SerializeField]
    private AttackState attackState;
    private GameObject attackobject;
    private Transform attackpoint;
    [SerializeField]
    private positonState positonstate;
    private Animator animator;
    private Rigidbody rb;
    private bool isaction_running;

    private bool ismelee_attack;
    private bool isrange_attack;

    private bool isdo_something;
    private bool attackable = true;

    public bool isArm_Hammer;
    public bool canRandom;

    private bool ishitable = true;

    private List<GameObject> range_obj_list = new List<GameObject>();
    public  List<bool> attack_bool_list = new List<bool>();
    [SerializeField]
    private Player_Controll player;
    private static float toPlayerAngle(Vector3 vstart, Vector3 vEnd)
    {
        return Quaternion.FromToRotation(Vector3.up, vEnd - vstart).eulerAngles.z;
    }
    public float Angle;

    
    private NavMeshPath path;
    private float WayPointsArrivalDistance = 1.5f;
    private bool isplayerdead = false;

    private float PathRefreshTime = 0.0f;
    private Vector3 attack_point;
    [SerializeField]
    private Vector3[] WayPoints;
    [SerializeField]
    private int currentWayPointIndex;
    [SerializeField]
    private int MovementSpeed = 2;
    public int AttackDistance;

    private void Start() 
    {
        path = new NavMeshPath();
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
        if(Boss_HP <= 0)
        {
            LevelManager.Instance.isbossdead = true;
            StartCoroutine(deadroutine());
        }
        if(state == State.idle || isplayerdead || GameManager.Instance.isPause == true)
        {
            OnMoveStop();
        }
        if(!GameManager.Instance.isPlayerDead && !LevelManager.Instance.isbossdead && !GameManager.Instance.isPause)
        {
            Rotatetoplayer();
        }
        if(!isdo_something && state == State.Chase && !isplayerdead && GameManager.Instance.isPause != true)
        {
            MovementSpeed = 2;
        }
        else if(GameManager.Instance.isPlayerDead)
        {
            player = null;
        }
        // else if()
        // {
        //     Debug.LogError($"플레이어에 해당하는 객체를 찾을 수 없습니다. 다음을 참조하세요 GameManager.Player,{0}",LevelManager.Instance.player);
        // }

    }
    private bool IsWayPointArrived(Vector3 currentWayPoint)
    {
        return Vector3.Distance(transform.position, currentWayPoint) <= WayPointsArrivalDistance;
    }
    public void UpdateFollwingPath()
    {
        if((state != State.Dead || isplayerdead != true || GameManager.Instance.isPause != true) && !isdo_something)
        {
            UpdateFollwingPath_Navigate();
        }
    }

    private void UpdateFollwingPath_Navigate()
    {
        PathRefreshTime += Time.deltaTime;
        if(PathRefreshTime >= 0.00025f)
        {
            PathRefreshTime = 0.0f;
            attack_point = new Vector3(player.transform.position.x,player.transform.position.y,player.transform.position.z);
            NavMesh.CalculatePath(transform.position, attack_point, NavMesh.AllAreas, path);
            for(int i = 0; i < path.corners.Length-1; i++)
            {
                Debug.DrawLine(path.corners[i], path.corners[i+1], Color.blue);
            }
            if(path.status == NavMeshPathStatus.PathComplete)
            {
                WayPoints = path.corners;
                state = (!isdo_something) ? State.Chase : State.Attack;
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
                if(currentWayPointIndex >= path.corners.Length)
                {
                    currentWayPointIndex = 0;   
                }
                else
                    currentWayPoint = WayPoints[currentWayPointIndex];
                UpdateFollwingPath_Navigate_OnMove();
            }
        }
    }

    private void UpdateFollwingPath_Navigate_OnMove()
    {
        if((!isdo_something && state == State.Chase) || state != State.Dead || !isplayerdead)
        {
            animator.SetBool("isMove", true);
            if(!isdo_something && attackable && state == State.Chase)
            {
                transform.position = Vector3.MoveTowards(transform.position, WayPoints[currentWayPointIndex], MovementSpeed * Time.deltaTime);
                Debug.DrawLine(transform.position, attack_point, Color.white);
            }
            if(Math.Round(Vector3.Distance(transform.position, attack_point), 2) <= AttackDistance)
            {
                state = State.Attack;
                OnMoveStop();
                BattleRoutine();
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
    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.name == "Attack_Col" && ishitable)
        {
            StartCoroutine(hitroutine());
        }
    }
    public void OnMoveStop()
    {
        MovementSpeed = 0;
        state = State.idle;
        animator.SetBool("isMove", false);
    }
    private IEnumerator hitroutine()
    {
        ishitable = false;
        Boss_HP -= 1;
        // animator.SetTrigger("isHit");
        if(positonstate == positonState.right)
        {
            rb.AddForce(Vector3.left * 5,ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(Vector3.right * 5, ForceMode.Impulse);
        }
        rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
        yield return new WaitForSeconds(0.25f);
        ishitable = true;
    }
    private IEnumerator deadroutine()
    {
        animator.SetTrigger("isDead");
        yield return new WaitForSecondsRealtime(1);
        // Destroy(this.gameObject);
        if(GameManager.Instance.isGameClear == false)
        {
            GameManager.Instance.interactionManager.if_Boss_Clear();
            GameManager.Instance.isGameClear = true;

        }
        // GameManager.Instance.NextSceneLoad();
    }
    private void BattleRoutine()
    {
        if(canRandom && !ismelee_attack && !isrange_attack) // 랜덤 패턴을 할 수 있고, 근&원거리 공격 전부 하고있지 않을때.
        {
            animator.SetBool("isMove",false);
            StartCoroutine(Random_Call());
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
            isArm_Hammer = true;
            yield return StartCoroutine(armhammerroutine());
        }
        yield return new WaitForSeconds(3);
        canRandom = true;
        attack_bool_list.Clear();
    }
    IEnumerator meleeAttackroutine()
    {
        attackState = AttackState.Melee;
        attackable = false;
        AttackPatternChange();
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
        attackable = true;
    }
    IEnumerator rangeAttackroutine()
    {
        attackState = AttackState.Range;
        attackable = false;
        AttackPatternChange();
        animator.SetTrigger("isrange_attack");
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.41f);

        Vector3 attackVec3 = attackpoint.position;
        for(int i = 0; i < 3; i++)
        {
            GameObject rangeattack_clone = Instantiate(attackobject, new Vector3(attackVec3.x, attackVec3.y, this.transform.position.z),Quaternion.identity);
            range_obj_list.Add(rangeattack_clone);
            float random_positon = player.transform.position.x + UnityEngine.Random.Range(-5,5);
            range_obj_list[i].transform.DOMove(new Vector3(random_positon, player.transform.position.y, player.transform.position.z), 1f).SetEase(Ease.Unset);
            Angle = 360f - toPlayerAngle(new Vector3(random_positon,player.transform.position.y, player.transform.position.z), attackpoint.position);
            range_obj_list[i].transform.rotation = Quaternion.Euler(0,0,-Angle);
        }
        yield return new WaitForSeconds(1f);
        for(int i = 0; i < 3; i++)
        {
            DOTween.Kill(range_obj_list[i]);
        }
        range_obj_list.Clear();
        isrange_attack = false;
        attackable = true;
    }
    IEnumerator armhammerroutine()
    {
        attackState = AttackState.Arm_Hammer;
        attackable = false;

        AttackPatternChange();
        animator.SetTrigger("isSummonArm");
        yield return new WaitForSeconds(0.5f);
        GameObject cloneArm = Instantiate(attackobject, attackpoint.transform);
        yield return new WaitForSeconds(2);
        ImpactShake();
        yield return new WaitForSeconds(4.5f);
        Destroy(attackpoint.gameObject);
        Destroy(cloneArm);
        attackpoint = null;
        attackable = true;
    }
    private void AttackPatternChange()
    {
        switch(attackState)
        {
            case AttackState.Melee :
            {
                attackobject = attackPattenlist[0].attackobject;
                attackpoint = attackPattenlist[0].attackpoint;
                break;
            }
            case AttackState.Range :
            {
                attackobject = attackPattenlist[1].attackobject;
                attackpoint = attackPattenlist[1].attackpoint;
                break;
            }
            case AttackState.Arm_Hammer :
            {
                attackobject = attackPattenlist[2].attackobject;
                GameObject ArmAttackPoint_gameObejct = new GameObject();
                ArmAttackPoint_gameObejct.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 3, player.transform.position.z + 4);
                attackpoint = ArmAttackPoint_gameObejct.transform;
                break;
            }
        }
    }
    private void ImpactShake()
    {
        GameObject target = FindFirstObjectByType<CinemachineVirtualCamera>().m_Follow.gameObject;
        target.transform.DOShakePosition(1.5f,new Vector3(5,1,0));
    }
}


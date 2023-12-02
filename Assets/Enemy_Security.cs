using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
public class Enemy_Security : MonoBehaviour
{

    public enum State
    {
        idle,
        hit,
        Chase,
        Attack,
        Emergency,
        Dead
    }
    public State state;
    [SerializeField]
    private bool isdo_something;
    private bool isplayerdead = false;
    [SerializeField]
    private bool Attackable = true;
    [SerializeField]
    private bool hitable = true;

    #region 백어택
    private bool leftbackattack;
    private bool rightbackattack;
    #endregion

    private Animator animator;
    private Rigidbody rb;

    private int MovementSpeed;
    private EnemySight Sight;
    private float PathRefreshTime = 0.0f;
    private float WayPointsArrivalDistance = 1.5f;
    private int currentWayPointIndex = 0;
    public NavMeshPath path;
    public Player_Controll target;
    private GameObject playersprite;
    private Vector3 attack_point;
    private Vector3[] WayPoints;

    [SerializeField]
    private int attack_count = 0;

    [SerializeField]
    private float AttackDistance = 1.5f;
    [SerializeField]
    private float BashDistance = 5f;

    public bool isflip;
    
    [Range(0,20)]
    public int SecurityHP;

    public GameObject HitEffectPrepab;

    private void OnEnable() 
    {
        path = new NavMeshPath();
        animator = this.GetComponent<Animator>();
        rb = this.gameObject.GetComponent<Rigidbody>();
        Sight = this.gameObject.GetComponentInChildren<EnemySight>();
        Sight.enemyType = EnemySight.EnemyType.Security;
        state = State.idle;
    }
    private void Start() {
        target = FindFirstObjectByType<Player_Controll>();
        playersprite = target.GetComponentInChildren<Animator>().gameObject;
    }
    private void CheckingBackAttack()
    {
        leftbackattack = (transform.localScale.x == 1 && target.transform.position.x >= this.transform.position.x) || (transform.localScale.x < 1 && target.transform.position.x > this.transform.position.x);
        rightbackattack = (transform.localScale.x < 1 && target.transform.position.x < this.transform.position.x) || (transform.localScale.x == 1 && target.transform.position.x < this.transform.position.x);
    }
    // Update is called once per frame
    void Update()
    {
        if(SecurityHP <= 0 && state != State.Dead)
        {
            StartCoroutine(deadroutine());
        }
        isplayerdead = GameManager.Instance.isPlayerDead;
        CheckingBackAttack();
        isdo_something = Checking_Do_Something();
        if(isplayerdead || GameManager.Instance.isPause == true)
        {
            StopAllCoroutines();
        }
        if(state == State.idle || isplayerdead || GameManager.Instance.isPause == true)
        {
            Moving_Stop();
        }
        if(!isdo_something && state == State.Chase && !isplayerdead && GameManager.Instance.isPause != true)
        {
            flipCheck();
            MovementSpeed = 2;
        }
    }
    private bool Checking_Do_Something()
    {
        if((!Attackable && hitable) || (Attackable && !hitable))
        {
            return true;
        }
        else 
            return false;
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
            flipCheck();
        } 
    }
    public void UpdateFollwingPath_Navigate()
    {
        PathRefreshTime += Time.deltaTime;
        if(PathRefreshTime >= 0.00025f)
        {
            PathRefreshTime = 0.0f;
            attack_point = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
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
                Moving_Stop();
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
        if((!isdo_something && state == State.Chase) && (state != State.Dead || !isplayerdead))
        {
            animator.SetBool("isMove", true);
            if(!isdo_something && Attackable)transform.position = Vector3.MoveTowards(transform.position, WayPoints[currentWayPointIndex], MovementSpeed * Time.deltaTime);
            
            if(Math.Round(Vector3.Distance(transform.position, attack_point), 2) <= BashDistance  && attack_count >= 5)
            {
                state = State.Attack;
                OnBash();
            }
            else if(Math.Round(Vector3.Distance(transform.position, attack_point), 2) <= AttackDistance)
            {
                state = State.Attack;
                OnAttack();
            }
        }
    }

    private void OnAttack()
    {
        MovementSpeed = 0;
        animator.SetBool("isMove", false);
        if(Attackable && !isdo_something)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private void OnBash()
    {
        MovementSpeed = 0;
        animator.SetBool("isMove", false);
        if(Attackable && !isdo_something)
        {
            StartCoroutine(BashRoutine());
        }  
    }
    public IEnumerator AttackRoutine()
    {
        Attackable = false;
        animator.SetTrigger("isAttack");
        yield return new WaitForSeconds(1.5f);
        Attackable = true;
        attack_count += 1;
    }
    public IEnumerator BashRoutine()
    {
        Attackable = false;
        animator.SetBool("isSpecial", true);
        animator.SetTrigger("isAttack");
        yield return new WaitForSeconds(0.3f);
        if(isflip == false)
        {
            DOTween.To(() => transform.position.x, x => transform.position = new Vector3(x, transform.position.y, transform.position.z), (transform.position.x + 5),0.7f);
        }
        else
            DOTween.To(() => transform.position.x, x => transform.position = new Vector3(x, transform.position.y, transform.position.z), (transform.position.x - 5),0.7f);
        yield return new WaitForSeconds(1f);
        Attackable = true;
        animator.SetBool("isSpecial", false);
        attack_count = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Attack_Col" && hitable)
        {
            var hit_vector = other.ClosestPoint(transform.position) + new Vector3(Random.Range(-0.5f,0.5f),Random.Range(-0.2f,1f),-0.1f);
            StartCoroutine(hitroutine(hit_vector));
        }    
    }

    private IEnumerator hitroutine(Vector3 position)
    {
        StopCoroutine(BashRoutine());
        StopCoroutine(AttackRoutine());
        state = State.hit;
        attack_count += 1;
        hitable = false;
        SecurityHP -= target.playerAttackDamage;
        var Hit_vfx_clone = Instantiate(HitEffectPrepab, position, Quaternion.identity);
        animator.SetTrigger("isHit");
        Vector3 player_normal_vec3 = new Vector3(playersprite.transform.localScale.x,1,0);
        rb.AddForce(player_normal_vec3 * 5, ForceMode.Impulse);
        if(rightbackattack)
        {
            rightbackattack = false;
            this.transform.localScale = new Vector3(-1,1,1);
        }
        else if(leftbackattack)
        {
            leftbackattack = false;
            this.transform.localScale = new Vector3(1,1,1);
        }
        yield return new WaitForSeconds(0.25f);
        hitable = true;
        Destroy(Hit_vfx_clone, 1f);
    }

    private IEnumerator deadroutine()
    {
        StopCoroutine(AttackRoutine());
        state = State.Dead;
        hitable = false;
        animator.SetTrigger("isDead");
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
    public void Moving_Stop()
    {
        state = State.idle;
        animator.SetBool("isMove", false);
    }

    private void flipCheck()
    {
        isflip = target.transform.position.x < this.transform.position.x;
        if(isflip)
        {
            this.gameObject.transform.localScale = new Vector3(-1,1,1);
        }
        else
        {
            this.gameObject.transform.localScale = new Vector3(1,1,1);
        }
    }


}

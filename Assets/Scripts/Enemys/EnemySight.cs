using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour 
{
    // static EnemySight _instance;
    // public static EnemySight Instance
    // {
    //     get{
    //         if(_instance)
    //         {
    //             GameObject container = FindFirstObjectByType<EnemySight>().gameObject;
    //             _instance = container.GetComponent<EnemySight>();
    //         }
    //         return _instance;
    //     }
    // }
    //TODO AI의 "시야"만 구현

    public enum EnemyType
    {
        normal,
        Security
    }
    public EnemyType enemyType;
    [SerializeField]
    [Range(0f, 360f)]private float viewAngle; //시야의 각도
    public float viewDistance; // 시야 거리
    [SerializeField]
    private LayerMask targetMask; // 타겟의 레이어 마스크(플레이어)

    private Enemy_Test2 enemy; //AI 본체 스크립트
    private Enemy_Security security;
     // Start is called before the first frame update
    void Start()
    {
        switch(enemyType)
        {
            case EnemyType.normal:
            {
                enemy = this.gameObject.GetComponentInParent<Enemy_Test2>();
                break;
            }
            case EnemyType.Security:
            {
                security = this.gameObject.GetComponentInParent<Enemy_Security>();
                break;
            }

        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(GameManager.Instance.isPause == false)
        {
            switch(enemyType)
            {
                case EnemyType.normal:
                {
                    if(enemy.state != Enemy_Test2.State.Dead)
                    {
                        Enemy_Normal_View();
                    }
                    break;
                }
                case EnemyType.Security:
                {
                    if(security.state != Enemy_Security.State.Dead)
                    {
                        Enemy_Security_View();
                    }
                    break;
                }
            }
        }
    }

    private Vector3 BoundaryAngle(float _angle)
    {
        _angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    }

    private void Enemy_Normal_View()
    {
        Vector3 _leftBoundary = BoundaryAngle(-viewAngle * 0.5f);  // z 축 기준으로 시야 각도의 절반 각도만큼 왼쪽으로 회전한 방향 (시야각의 왼쪽 경계선)
        Vector3 _rightBoundary = BoundaryAngle(viewAngle * 0.5f);  // z 축 기준으로 시야 각도의 절반 각도만큼 오른쪽으로 회전한 방향 (시야각의 오른쪽 경계선)

        Debug.DrawRay(transform.position + transform.up, _leftBoundary, Color.red);
        Debug.DrawRay(transform.position + transform.up, _rightBoundary, Color.cyan);

        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance, targetMask);
        if(_target.Length == 0)
        {
            enemy.OnMoveStop();
        }
        for (int i = 0; i < _target.Length; i++)
        {
            Transform _targetTf = _target[i].transform;
            if (_targetTf.name == "Player")
            {
                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                float _angle = Vector3.Angle(_direction, transform.forward);

                if (_angle < viewAngle * 0.5f)
                {
                    RaycastHit _hit;
                    if(Physics.Raycast(transform.position + transform.up, _direction, out _hit, viewDistance))
                    {
                        if (_hit.transform.name == "Player")
                        {
                            Debug.DrawRay(transform.position + transform.up, _direction, Color.blue);
                            enemy.UpdateFollwingPath();
                        }
                        else
                        {
                            enemy.OnMoveStop();
                        }
                    }
                }
            }
        }
    }

    private void Enemy_Security_View()
    {
        Vector3 _leftBoundary = BoundaryAngle(-viewAngle * 0.5f);  // z 축 기준으로 시야 각도의 절반 각도만큼 왼쪽으로 회전한 방향 (시야각의 왼쪽 경계선)
        Vector3 _rightBoundary = BoundaryAngle(viewAngle * 0.5f);  // z 축 기준으로 시야 각도의 절반 각도만큼 오른쪽으로 회전한 방향 (시야각의 오른쪽 경계선)

        Debug.DrawRay(transform.position + transform.up, _leftBoundary, Color.red);
        Debug.DrawRay(transform.position + transform.up, _rightBoundary, Color.cyan);

        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance, targetMask);
        if(_target.Length == 0)
        {
            security.Moving_Stop();
        }
        for (int i = 0; i < _target.Length; i++)
        {
            Transform _targetTf = _target[i].transform;
            if (_targetTf.name == "Player")
            {
                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                float _angle = Vector3.Angle(_direction, transform.forward);

                if (_angle < viewAngle * 0.5f)
                {
                    RaycastHit _hit;
                    if(Physics.Raycast(transform.position + transform.up, _direction, out _hit, viewDistance))
                    {
                        if (_hit.transform.name == "Player")
                        {
                            Debug.DrawRay(transform.position + transform.up, _direction, Color.blue);
                            security.UpdateFollwingPath();
                        }
                        else
                        {
                            security.Moving_Stop();
                        }
                    }
                }
            }
        }
    }
}

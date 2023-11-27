using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOSS_SIGHT : MonoBehaviour
{
    [SerializeField][Range(0,360)]
    private float viewAngle;
    [SerializeField]
    private float viewDistance;
    [SerializeField]
    private LayerMask target_layermask;

    private BOSS_ENEMY boss;

    // Start is called before the first frame update
    void Start()
    {
        boss = this.gameObject.GetComponentInParent<BOSS_ENEMY>();
    }


    void FixedUpdate()
    {
        if(GameManager.Instance.isPause == false)
        {
            Boss_View();
        }
    }

    private Vector3 BoundaryAngle(float _angle)
    {
        _angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0, Mathf.Cos(_angle * Mathf.Deg2Rad));
    }
    private void Boss_View()
    {
        Vector3 _leftBoundary = BoundaryAngle(-viewAngle * 0.5f);  // z 축 기준으로 시야 각도의 절반 각도만큼 왼쪽으로 회전한 방향 (시야각의 왼쪽 경계선)
        Vector3 _rightBoundary = BoundaryAngle(viewAngle * 0.5f);  // z 축 기준으로 시야 각도의 절반 각도만큼 오른쪽으로 회전한 방향 (시야각의 오른쪽 경계선)

        Debug.DrawRay(transform.position + transform.up, _leftBoundary, Color.red);
        Debug.DrawRay(transform.position + transform.up, _rightBoundary, Color.cyan);

        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance, target_layermask);
        if(_target.Length == 0)
        {
            boss.OnMoveStop();
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
                            boss.UpdateFollwingPath();
                        }
                        else
                        {
                            boss.OnMoveStop();
                        }
                    }
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour 
{
    //TODO AI의 "시야"만 구현

    [SerializeField]
    [Range(0f, 360f)]private float viewAngle; //시야의 각도
    [SerializeField]
    private float viewDistance; // 시야 거리
    [SerializeField]
    private LayerMask targetMask; // 타겟의 레이어 마스크(플레이어)

    private Enemy_Test2 enemy; //AI 본체 스크립트

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy_Test2>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

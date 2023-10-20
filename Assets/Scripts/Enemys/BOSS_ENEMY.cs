using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BOSS_ENEMY : MonoBehaviour
{
    [Serializable]
    public struct AttackPattern
    {
        public string pattername;
        public AttackState attackmode;
        public GameObject attackobject;
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

    private positonState positonstate;
    private AttackState attackState;
    private Player_Controll player;
    private void OnEnable() 
    {
        player = LevelManager.Instance.player;
    }
    private void FixedUpdate() 
    {
        Rotatetoplayer();
    }
    // 플레이어와의 위치 차이에 따라 좌우 회전 (백어택 불가능)
    private void Rotatetoplayer()
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

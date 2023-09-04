using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;


[RequireComponent(typeof (Player_Controll))]
public class StatUpgrade : MonoBehaviour
{
    [System.Serializable]
    public struct ChooseStat
    {
        [Header("선택 시 업그레이드할 스탯")]
        public string statstring;
    }
    public ChooseStat[] targetstats;
    [YarnCommand("StatusUpgrade")]
    public void UpgradeStat(string upgradestatstring)
    {
        Player_Controll player = this.gameObject.GetComponent<Player_Controll>();
        if(player == null)
        {
            Debug.LogErrorFormat("플레이어 컴포넌트를 찾을 수 없습니다 {0}", player);
            return;
        }

        //TOOD : 각 선택지 별 스테이터스 분리
        switch (upgradestatstring)
        {
            case "playerAttackDamage" :
            {
                Debug.Log("플레이어 공격력 증가");
                player.playerAttackDamage += 1;
                break;
            }
            case "MoveSpeed" :
            {
                player.MoveSpeed += 0.25f;
                break;
            }
            
            case "HP_Point" :
            {
                Debug.Log("플레이어 체력 증가");
                break;
            }
        }
    }

}

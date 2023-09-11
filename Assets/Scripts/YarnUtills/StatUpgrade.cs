using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;


public class StatUpgrade : MonoBehaviour
{
    [YarnCommand("ATKUpgrade")]
    public void APupgrade()
    {
        //TODO : 공격력 업그레이드 
        this.gameObject.GetComponent<Player_Controll>().playerAttackDamage += 1;
    }
    [YarnCommand("HPUpgrade")]
    public void HPupgrade()
    {
        this.gameObject.GetComponent<PlayerHP>().shieldpoint += 1;
    }
    [YarnCommand("SPUpgrade")]
    public void SPupgrade()
    {
        Debug.Log("SP + 0.25f");
    }
}

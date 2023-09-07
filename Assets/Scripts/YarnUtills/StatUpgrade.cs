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
    }
    [YarnCommand("HPUpgrade")]
    public void HPupgrade()
    {
        Debug.Log("HP +1");
        //TODO : HP업그레이드
    }
    [YarnCommand("SPUpgrade")]
    public void SPupgrade()
    {
        Debug.Log("SP + 0.25f");
    }
}

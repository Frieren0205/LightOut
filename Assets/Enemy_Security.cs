using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Enemy_Security : MonoBehaviour
{

    private enum State
    {
        idle,
        hit,
        Chase,
        Attack,
        Emergency,
        Dead
    }
    private bool isdo_something;
    private bool Attackable = true;
    private bool hitable = true;
    private bool leftbackattack;
    private bool rightbackattack;
    [Range(0,10)]
    public int SecurityHP;


    public bool isflip;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

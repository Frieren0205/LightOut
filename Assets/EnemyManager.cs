using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    static GameObject container;
    static EnemyManager _instance;
    public static EnemyManager Instance
    {
        get{
            if(!_instance)
            {
                container = GameObject.FindFirstObjectByType<EnemyManager>().gameObject;
                _instance = container.GetComponent<EnemyManager>();
            }
            return _instance;
        }
    }

    public List<Generator> generators;

}

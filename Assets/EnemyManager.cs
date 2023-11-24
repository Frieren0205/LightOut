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
    public void FindingGenerator()
    {
        var Generator_list_var = FindObjectsByType<Generator>(FindObjectsSortMode.None);
        Array.Sort(Generator_list_var, (a,b) =>
        {
            return a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex());
        });
        generators = new List<Generator>(Generator_list_var);
    }
}

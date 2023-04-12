using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public enum Level
    {
        Underground,
        Sub_Tera,
        In_Tera
    }
    public Level level;
    public Vector3[] LimitiedPositionZ;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(level == Level.Underground)
        {

        }
    }
}

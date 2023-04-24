using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LevelManager : MonoBehaviour
{
    public enum Level
    {
        Title,
        Underground,
        Sub_Tera,
        In_Tera
    }
    public Level level;
    public Player_Controll player;

    public (int minPosition, int maxPosition)[] LimitiedPosition;
    [Serializable]
    public struct LimitiedPositions
    {
        public float minPosition;
        public float maxPosition;
    }

    [Header("맵 별 포지션 제한도")]
    public LimitiedPositions[] limitiedPositions;
    public Vector3[] MinLimitiedPosition;
    public Vector3[] MaxLimitiedPosition;
    private CinemachineConfiner confiner;
    public BoxCollider cameraLimitedAreas;
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    private void Update() 
    {
        LevelSetting(level);
    }
    // Update is called once per frame
    void LateUpdate()
    {
        CameraAreasUpdate();
    }
    public void LevelSetting(Level level)
    {
        if(level == Level.Underground)
        {
            player = FindObjectOfType<Player_Controll>();
            player.minLimit = limitiedPositions[0].minPosition;
            player.maxLimit = limitiedPositions[0].maxPosition;
        }
    }
    public void CameraAreasUpdate()
    {
        if(level == Level.Underground)
        {   
            //카메라 세팅부터
            cameraLimitedAreas = GameObject.Find("UnderGround_Camera_Area").GetComponent<BoxCollider>();
            confiner = FindFirstObjectByType<CinemachineConfiner>();
            confiner.m_BoundingVolume = cameraLimitedAreas;
        }
    }
}

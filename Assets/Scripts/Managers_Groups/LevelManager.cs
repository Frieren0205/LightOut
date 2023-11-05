using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LevelManager : MonoBehaviour
{
    static GameObject container;
    static LevelManager _instance;
    public  static LevelManager Instance
    {
        get
        {
            if(!_instance)
            {
                container = GameObject.FindFirstObjectByType<LevelManager>().gameObject;
                _instance = container.GetComponent<LevelManager>();
            }
            return _instance;
        }
    }
    public enum Level
    {
        Title,
        Underground,
        Sub_Tera,
        In_Tera,
        Boss_Battle
    }
    public Level level;
    public Player_Controll player;
    //TODO : 중간보스

    public BOSS_ENEMY boss_object;
    public bool isbossdead;

    public (int minPosition, int maxPosition)[] LimitiedPosition;
    [Serializable]
    public struct LimitiedPositions
    {
        public float minPosition;
        public float maxPosition;
    }

    [Header("맵 별 포지션 제한도")]
    public LimitiedPositions[] limitiedPositions;
    #region 
    [Header("레벨 클리어 조건 달성여부")]
    public bool isLevel1Clear;
    public bool isLevel2Clear;
    private bool isLevel2ClearCheck()
    {
        if(level2ClearCheckPoints[0] && level2ClearCheckPoints[1] && level2ClearCheckPoints[2])
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool[] level2ClearCheckPoints;
    public bool isLevel3Clear;
    #endregion
    private CinemachineVirtualCamera cinevirtualcam;
    private CinemachineConfiner confiner;
    [Header("카메라 영역 제한용 컬라이더")]
    public BoxCollider cameraLimitedAreas;
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    private void Update() {
       isLevel2Clear = isLevel2ClearCheck();
    }
    public void LevelSetting(Level level)
    {
        if(level == Level.Underground)
        {
            player.minLimit = limitiedPositions[0].minPosition;
            player.maxLimit = limitiedPositions[0].maxPosition;
        }
        if(level == Level.Sub_Tera)
        {
            player.minLimit = limitiedPositions[1].minPosition;
            player.maxLimit = limitiedPositions[1].maxPosition;
        }
        if(level == Level.In_Tera)
        {
            player.minLimit = limitiedPositions[2].minPosition;
            player.maxLimit = limitiedPositions[2].maxPosition;
        }
        if(level == Level.Boss_Battle)
        {
            player.minLimit = limitiedPositions[3].minPosition;
            player.maxLimit = limitiedPositions[3].maxPosition;

            if(!GameManager.Instance.isGameClear)
            {
                boss_object = FindFirstObjectByType<BOSS_ENEMY>();
                isbossdead = false;
            }
        }
    }
    public void CameraTrackingUpdate()
    {
        cinevirtualcam = FindFirstObjectByType<CinemachineVirtualCamera>();
        cinevirtualcam.Follow = player.GetComponentInChildren<Transform>().Find("Followed_This").transform;
    }
    public void CameraAreasUpdate()
    {
        switch(level)
        {
            case Level.Underground:
            {
                cameraLimitedAreas = GameObject.Find("UnderGround_Camera_Area").GetComponent<BoxCollider>();
                confiner = FindFirstObjectByType<CinemachineConfiner>();
                confiner.m_BoundingVolume = cameraLimitedAreas;
                break;
            }
            case Level.Sub_Tera:
            {
                cameraLimitedAreas = GameObject.Find("SubTera_Camera_Area").GetComponent<BoxCollider>();
                if(level2ClearCheckPoints[0])
                {
                    cameraLimitedAreas = GameObject.Find("SubTera_Camera_Area_2").GetComponent<BoxCollider>();
                }
                else if(level2ClearCheckPoints[1])
                {
                    
                }
                confiner = FindFirstObjectByType<CinemachineConfiner>();
                confiner.m_BoundingVolume = cameraLimitedAreas;
                break;
            }
            case Level.In_Tera:
            {
                break;
            }
            case Level.Boss_Battle:
            {
                cameraLimitedAreas = GameObject.Find("Boss_Camera_Area").GetComponent<BoxCollider>();
                confiner = FindFirstObjectByType<CinemachineConfiner>();
                confiner.m_BoundingVolume = cameraLimitedAreas;
                break;
            }
        }
    }
}

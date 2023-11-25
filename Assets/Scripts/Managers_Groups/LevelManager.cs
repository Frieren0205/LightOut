using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

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

    #region 일반 몬스터 그룹
    public List<Enemy_Test2> normal_enemy_list;
    public Enemy_Security enemy_Security;
    #endregion
    #region 발전기 그룹
    public List<Generator> generator_list;
    #endregion
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
    private void Update() 
    {
        Clearcheckpoint();
    }
    public void LevelSetting(Level level)
    {
        if(level == Level.Underground)
        {
            player.minLimit = limitiedPositions[0].minPosition;
            player.maxLimit = limitiedPositions[0].maxPosition;
            FindEnemy();

        }
        if(level == Level.Sub_Tera)
        {
            player.minLimit = limitiedPositions[1].minPosition;
            player.maxLimit = limitiedPositions[1].maxPosition;
            
            FindGenerator();
            FindEnemy();

            enemy_Security = FindFirstObjectByType<Enemy_Security>();
        }
        if(level == Level.In_Tera)
        {
            player.minLimit = limitiedPositions[2].minPosition;
            player.maxLimit = limitiedPositions[2].maxPosition;

            FindGenerator();
            FindEnemy();
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
    public void FindGenerator()
    {
        var subtera_generator_list_var = FindObjectsByType<Generator>(FindObjectsSortMode.None);
        Array.Sort(subtera_generator_list_var,(a,b) =>
        {
            // 발전기 오름차순으로 재정렬
            return a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex());
        });
        generator_list = new List<Generator>(subtera_generator_list_var);
    }
    public void FindEnemy()
    {
        var normal_enemy_list_var = FindObjectsByType<Enemy_Test2>(FindObjectsSortMode.None);
        Array.Sort(normal_enemy_list_var, (a,b) =>
        {
            return a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex());
        });
        normal_enemy_list = new List<Enemy_Test2>(normal_enemy_list_var);
    }
    private void Clearcheckpoint()
    {
        switch(level)
        {
            case Level.Underground:
            {
                
                if(normal_enemy_list.Any() == false)
                {
                    isLevel1Clear = true;
                }
                else
                {
                    isLevel1Clear = false;
                }
                break;
            }
            case Level.Sub_Tera:
            {
                isLevel2Clear = isLevel2ClearCheck();
                if(generator_list.Count == 6)
                {
                    level2ClearCheckPoints[0] = true;
                }
                else if(generator_list.Count == 3)
                {
                    level2ClearCheckPoints[0] = true;
                    level2ClearCheckPoints[1] = true;
                }
                else if(generator_list.Count == 0 || !generator_list.Any() || enemy_Security == null )
                {
                    level2ClearCheckPoints[0] = true;
                    level2ClearCheckPoints[1] = true;
                    level2ClearCheckPoints[2] = true;
                }
                else
                {
                    level2ClearCheckPoints[0] = false;
                    level2ClearCheckPoints[1] = false;
                    level2ClearCheckPoints[2] = false;
                }
                break;
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
                confiner = FindFirstObjectByType<CinemachineConfiner>();
                if(cameraLimitedAreas == null) cameraLimitedAreas = GameObject.Find("SubTera_Camera_Area").GetComponent<BoxCollider>();
                confiner.m_BoundingVolume = cameraLimitedAreas;
                break;
            }
            case Level.In_Tera:
            {
                confiner = FindFirstObjectByType<CinemachineConfiner>();
                if(cameraLimitedAreas == null) cameraLimitedAreas = GameObject.Find("InTera_Camera_Area").GetComponent<BoxCollider>();
                confiner.m_BoundingVolume = cameraLimitedAreas;
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

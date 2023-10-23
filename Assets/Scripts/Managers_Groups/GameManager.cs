using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class GameManager : MonoBehaviour
{
    static GameObject container;
    static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if(!_instance)
            {
                container = GameObject.FindFirstObjectByType<GameManager>().gameObject;
                _instance = container.GetComponent<GameManager>();
            }
            return _instance;
        }
    }
    public LevelManager levelManager;
    public InteractionManager interactionManager;
    public UIManager uIManager;


    public bool isPause;
    public bool isPlayerDead = false;
    [SerializeField]
    private Player_Controll player;
    // Start is called before the first frame update
    private void Awake() 
    {
        var managerinstance = FindObjectsOfType<GameManager>();
        if(Instance != this && Instance != null && managerinstance.Length > 1)
        {
            Destroy(managerinstance[0].gameObject, 1.5f);
            return;
        }
        else
        {
            _instance  = this;
        }
    }
    void OnEnable()
    {
        DataManager.Instance.LoadFromSaveData();
        uIManager = this.gameObject.GetComponent<UIManager>();
        levelManager = this.gameObject.GetComponent<LevelManager>();
        PauseStateReset();
        OnSwitchLevel();
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(uIManager.castfadein());
        if(player != null)
        {
            //TODO : 플레이어 레벨별 포지션 로드 변경
            player.transform.position = DataManager.Instance.saveData.lastest_p_transform;
            switch (DataManager.Instance.saveData.lastest_p_level)
            {
                case LevelManager.Level.Underground:
                {
                    break;
                }
                case LevelManager.Level.Sub_Tera:
                {
                    break;
                }
                case LevelManager.Level.In_Tera:
                {
                    break;
                }
                case LevelManager.Level.Boss_Battle:
                {
                    break;
                }
            }
        }
    }
    private void OnDisable() 
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void Start() 
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void PauseStateReset()
    {
        isPause = false;
        UIManager.Instance.isPause = false;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        OnSwitchLevel();
        StartCoroutine(UIManager.Instance.castfadein());
    }

    private void OnSwitchLevel()
    {
        switch(SceneManager.GetActiveScene().buildIndex)
        {
            case 2:
            {
                levelManager.level = LevelManager.Level.Underground;
                if(!player) player = SpawnManager.Instance.SpawnPlayer(SpawnManager.Instance.spawnpoints[0]);
                LevelManager.Instance.player = player;
                LevelManager.Instance.LevelSetting(LevelManager.Level.Underground);
                LevelSetting();
                LevelManager.Instance.CameraAreasUpdate();
                break;
            }
            case 3:
            {
                levelManager.level = LevelManager.Level.Sub_Tera;
                if(!player) player = SpawnManager.Instance.SpawnPlayer(SpawnManager.Instance.spawnpoints[1]);
                levelManager.player = player;
                player.gameObject.transform.position = SpawnManager.Instance.spawnpoints[1].spawnpositionVec3;
                playerinit();
                LevelManager.Instance.LevelSetting(LevelManager.Level.Sub_Tera);
                LevelSetting();
                LevelManager.Instance.CameraTrackingUpdate();
                LevelManager.Instance.CameraAreasUpdate();
                break;
            }
            // case 4:
            // {
            //     levelManager.level = LevelManager.Level.In_Tera;
            //     break;
            // }
            case 4:
            {
                levelManager.level = LevelManager.Level.Boss_Battle;
                levelManager.player = player;
                player.gameObject.transform.position = SpawnManager.Instance.spawnpoints[3].spawnpositionVec3;
                playerinit();
                LevelManager.Instance.LevelSetting(LevelManager.Level.Boss_Battle);
                LevelSetting();
                LevelManager.Instance.CameraTrackingUpdate();
                LevelManager.Instance.CameraAreasUpdate();
                break;
            }
        }
    }
    private void playerinit()
    {
        player.CanInteraction = false;
        player.interactionPoint = null;
        player.CanInteractionIcon.SetActive(false);
    }
    // private void OnEnable() 
    // { 
    //     /// 새롭게 바꿀 때 준비해야 할 것
    //     /// 1. 타이틀화면에서는 플레이어를 스폰하면 안됨
    //     /// 2. 플레이어를 생성과 동시에 싱글톤화 된 LevelManager의 Player컴포넌트에 인풋
    //     /// 3. 각 레벨별 플레이어의 위치를 변경할 수 있도록
        
    //     // 1. 타이틀화면에서는 플레이어를 스폰하면 안됨
    //     if(levelManager.level != LevelManager.Level.Title)
    //     {
    //         switch(levelManager.level)
    //         {
    //             case LevelManager.Level.Underground:
    //             {
    //                 // 플레이어가 없다면 플레이어를 초기 스폰 위치에 생성시키기
    //                 // if(!player) player = SpawnManager.Instance.SpawnPlayer(SpawnManager.Instance.spawnpoints[0]);
    //                 // 플레이어를 생성과 동시에 LevelManager의 Player 컴포넌트에 인풋
    //                 LevelManager.Instance.player = player;
    //                 break;
    //             }
    //             case LevelManager.Level.Sub_Tera:
    //             {
    //                 player.gameObject.transform.position = SpawnManager.Instance.spawnpoints[1].spawnpositionVec3;
    //                 break;
    //             }
    //             case LevelManager.Level.In_Tera:
    //             {
    //                 break;
    //             }
    //         }
    //     }
    // }
    // void Start()
    // {
    //     DontDestroyOnLoad(this.gameObject);
    //     switch(SceneManager.GetActiveScene().buildIndex)
    //     {
    //         case 2:
    //         {
    //             levelManager.level = LevelManager.Level.Underground;
    //             LevelManager.Instance.LevelSetting(LevelManager.Level.Underground);
    //             LevelSetting();
    //             LevelManager.Instance.CameraAreasUpdate();
    //             break;
    //         }
    //         case 3:
    //         {
    //             levelManager.level = LevelManager.Level.Sub_Tera;
    //             LevelSetting();
    //             break;
    //         }
    //         case 4:
    //         {
    //             levelManager.level = LevelManager.Level.In_Tera;
    //             LevelSetting();
    //             break;
    //         }
    //     }
    // }
    private void LevelSetting()
    {
        interactionManager.player = player;
        player.interactionManager = this.interactionManager;
        switch(levelManager.level)
        {
            case LevelManager.Level.Underground:
            {
                player.playerHP.volume = GameObject.Find("UnderWater_Volume").GetComponent<Volume>();
                break;
            }
            case LevelManager.Level.Sub_Tera:
            {
                player.playerHP.volume = GameObject.Find("SubTera_Volume").GetComponent<Volume>();   
                break;
            }
            case LevelManager.Level.In_Tera:
            {
                break;
            }
        }
        LevelManager.Instance.CameraTrackingUpdate();
    }
        //TODO : 플레이 데이터 저장
    public void SavePlayerData()
    {
        //TODO : 게임 종료시의 자동저장 정보 제한
        DataManager.Instance.saveData.lastest_p_transform = player.transform.position;
        DataManager.Instance.saveData.lastest_p_level = LevelManager.Instance.level;
    }   
    void OnApplicationQuit()
    {
        if(player !=  null)
        {
            SavePlayerData();
            //if(levelManager.level == LevelManager.Level.Title) DataManager.Instance.saveData.lastest_p_level = levelManager.level;
            DataManager.Instance.SavetoSaveData();
        }
    }
    [YarnCommand("LOADNEXTSCENE")]
    public void NextSceneLoad()
    {
        //TODO : 페이드 인 아웃으로 다음 씬 넘어갈 수 있도록
        Invoke("NextSceneLoadfromindex",1);
        StartCoroutine(UIManager.Instance.castfadeout());
    }
    [YarnCommand("LOAD_BOSS_SCENE")]
    public void BossSceneLoad()
    {
        Invoke("BossSceneLoad_test", 1);
        StartCoroutine(UIManager.Instance.castfadeout());
    }
    public void BossSceneLoad_test()
    {
        SceneManager.LoadScene(4);
    }
    public void NextSceneLoadfromindex()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    internal void PlayerObjectDestroy()
    {
        Destroy(player.gameObject, 1);
    }
}

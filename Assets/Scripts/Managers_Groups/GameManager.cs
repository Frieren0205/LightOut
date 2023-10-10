using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if(!_instance)
            {
                GameObject container = GameObject.FindFirstObjectByType<GameManager>().gameObject;
                _instance = container.GetComponent<GameManager>();
            }
            return _instance;
        }
    }
    private LevelManager levelManager;
    public InteractionManager interactionManager;
    public UIManager uIManager;


    public bool isPause;
    [SerializeField]
    private Player_Controll player;
    // Start is called before the first frame update
    private void Awake() 
    {
        DontDestroyOnLoad(this.gameObject);
        levelManager = this.gameObject.GetComponent<LevelManager>();
        DataManager.Instance.LoadFromSaveData();
        PauseStateReset();
        OnSwitchLevel();
        SceneManager.sceneLoaded += OnSceneLoaded;
        if(player != null) player.transform.position = DataManager.Instance.saveData.lastest_p_transform;
    }

    public void PauseStateReset()
    {
        isPause = false;
        UIManager.Instance.isPause = false;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        OnSwitchLevel();
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
                player.gameObject.transform.position = SpawnManager.Instance.spawnpoints[1].spawnpositionVec3;
                LevelManager.Instance.CameraTrackingUpdate();
                LevelManager.Instance.CameraAreasUpdate();
                break;
            }
            case 4:
            {
                levelManager.level = LevelManager.Level.In_Tera;
                break;
            }
        }
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
        player.playerHP.volume = GameObject.Find("UnderWater_Volume").GetComponent<Volume>();
        LevelManager.Instance.CameraTrackingUpdate();
    }
    void OnApplicationQuit()
    {
        if(player !=  null)
        {
            DataManager.Instance.saveData.lastest_p_transform = player.transform.position;
            //if(levelManager.level == LevelManager.Level.Title) DataManager.Instance.saveData.lastest_p_level = levelManager.level;
            DataManager.Instance.SavetoSaveData();
        }
    }
    [YarnCommand("LOADNEXTSCENE")]
    public void NextSceneLoad()
    {
        //TODO : 페이드 인 아웃으로 다음 씬 넘어갈 수 있도록
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}

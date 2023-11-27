using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
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

    public bool isGameClear = false;
    [SerializeField]
    private Player_Controll player;

    public Player_Controll[] player_list;
    // Start is called before the first frame update

    private bool[] isfirstplay = new bool[4];
    private void Awake() 
    {
        var managerinstance = FindObjectsOfType<GameManager>();
        if(Instance != this && Instance != null && managerinstance.Length > 1)
        {
            // managerinstance[0].gameObject.GetComponentInChildren<EventSystem>().enabled = false;
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
    // 이벤트 끝났을때의 체크
    public void PauseStateReset()
    {
        isPause = false;
        UIManager.Instance.isPause = false;
        interactionManager.advanceInput.enabled = false;
        if(player != null)
        {
            player.CanInteractionIcon.SetActive(false);
            player.CanInteraction = true;
        }
    }
    public void GameClearCheck()
    {
        if(isGameClear)
        {
            Destroy(player.gameObject);
            Destroy(FindFirstObjectByType<BOSS_ENEMY>().gameObject);
            NextSceneLoad();
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        OnSwitchLevel();
        StartCoroutine(UIManager.Instance.castfadein());
        player_duplicate_check();
    }

    private void OnSwitchLevel()
    {
        switch(SceneManager.GetActiveScene().buildIndex)
        {
            case 2:
            {
                levelManager.level = LevelManager.Level.Underground;
                if(!player)
                {
                    player = SpawnManager.Instance.SpawnPlayer(SpawnManager.Instance.spawnpoints[0]);
                }
                    LevelManager.Instance.player = player;
                    player.levelManager = levelManager;
                    player.gameObject.transform.position = SpawnManager.Instance.spawnpoints[0].spawnpositionVec3;
                    playerinit();
                    LevelManager.Instance.LevelSetting(LevelManager.Level.Underground);
                    LevelSetting();
                    LevelManager.Instance.CameraAreasUpdate();
                    if(isfirstplay[0] == false)
                    {
                        interactionManager.After_Prologue();
                        isfirstplay[0] = true;
                    }
                break;
            }
            case 3:
            {
                levelManager.level = LevelManager.Level.Sub_Tera;
                if(!player)
                {
                    player = SpawnManager.Instance.SpawnPlayer(SpawnManager.Instance.spawnpoints[1]);
                }
                levelManager.player = player;
                player.levelManager = levelManager;
                player.gameObject.transform.position = SpawnManager.Instance.spawnpoints[1].spawnpositionVec3;
                playerinit();
                LevelManager.Instance.LevelSetting(LevelManager.Level.Sub_Tera);
                LevelSetting();
                LevelManager.Instance.CameraTrackingUpdate();
                LevelManager.Instance.CameraAreasUpdate();
                if(isfirstplay[1] == false)
                {
                    isfirstplay[1] = true;
                }
                break;
            }
            case 4:
            {
                levelManager.level = LevelManager.Level.In_Tera;
                if(!player)
                    player = SpawnManager.Instance.SpawnPlayer(SpawnManager.Instance.spawnpoints[2]);
                    playerinit();
                    levelManager.player = player;
                    player.levelManager = levelManager;
                    player.gameObject.transform.position = SpawnManager.Instance.spawnpoints[2].spawnpositionVec3;
                    LevelManager.Instance.LevelSetting(LevelManager.Level.In_Tera);
                    LevelSetting();
                    LevelManager.Instance.CameraAreasUpdate();
                    LevelManager.Instance.CameraAreasUpdate();
                    if(isfirstplay[2] == false)
                    {
                        isfirstplay[2] = true;
                    }
                break;
            }
            case 5:
            {
                levelManager.level = LevelManager.Level.Boss_Battle;
                if(!player)
                    player = SpawnManager.Instance.SpawnPlayer(SpawnManager.Instance.spawnpoints[3]);
                    // playerinit();
                    levelManager.player = player;
                    player.levelManager = levelManager;
                    player.gameObject.transform.position = SpawnManager.Instance.spawnpoints[3].spawnpositionVec3;
                    LevelManager.Instance.LevelSetting(LevelManager.Level.Boss_Battle);
                    LevelSetting();
                    LevelManager.Instance.CameraTrackingUpdate();
                    LevelManager.Instance.CameraAreasUpdate();
                    if(isfirstplay[3] == false)
                    {
                        interactionManager.BossApear();
                        isfirstplay[3] = true;
                    }
                break;
            }
        }
    }
    public void Player_respawn()
    {
        UIManager.Instance.DisableGameover();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        playerinit();
    }
    private void player_duplicate_check()
    {
        var playerlist = FindObjectsOfType<Player_Controll>();
        player_list = playerlist;
        if(playerlist.Length > 1)
        {
            Debug.Log("플레이어 재지정");
            player = playerlist[0];
            Destroy(playerlist[1].gameObject);
        }
        else
            return;
    }
    private void playerinit()
    {
        isPause = false;
        isPlayerDead = false;
        player.CanInteraction = true;
        player.interactionPoint = null;
        player.CanInteractionIcon.SetActive(false);
    }
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
                player.playerHP.volume = GameObject.Find("In_Tera_Volume").GetComponent<Volume>();
                break;
            }
            case LevelManager.Level.Boss_Battle:
            {
                player.playerHP.volume = GameObject.Find("Boss_Volume").GetComponent<Volume>();
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

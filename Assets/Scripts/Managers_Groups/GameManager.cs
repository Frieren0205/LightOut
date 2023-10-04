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
        levelManager = this.gameObject.GetComponent<LevelManager>();
        DataManager.Instance.LoadFromSaveData();
        if(player != null) player.transform.position = DataManager.Instance.saveData.lastest_p_transform;
    }
    private void OnEnable() 
    {
        if(!player)
        {
            // SpawnManager.Instance.SetPlayerSpawn(SpawnManager.Instance.spawnpoints[0]);
            player = SpawnManager.Instance.SpawnPlayer(SpawnManager.Instance.spawnpoints[0]);
        }
    }
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        switch(SceneManager.GetActiveScene().buildIndex)
        {
            case 2:
            {
                levelManager.level = LevelManager.Level.Underground;
                LevelManager.Instance.LevelSetting(LevelManager.Level.Underground);
                LevelSetting();
                LevelManager.Instance.CameraAreasUpdate();
                break;
            }
            case 3:
            {
                levelManager.level = LevelManager.Level.Sub_Tera;
                LevelSetting();
                break;
            }
            case 4:
            {
                levelManager.level = LevelManager.Level.In_Tera;
                LevelSetting();
                break;
            }
        }
    }
    private void LevelSetting()
    {
        player = FindFirstObjectByType<Player_Controll>().GetComponent<Player_Controll>();
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}

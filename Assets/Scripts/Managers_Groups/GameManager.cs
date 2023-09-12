using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private LevelManager levelManager;
    public InteractionManager interactionManager;
    public UIManager uIManager;


    public bool isPause;
    private Player_Controll player;
    // Start is called before the first frame update
    void Start()
    {
        levelManager = this.gameObject.GetComponent<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        DontDestroyOnLoad(this.gameObject);
        switch(SceneManager.GetActiveScene().buildIndex)
        {
            case 1:
            {
                levelManager.level = LevelManager.Level.Underground;
                player = FindAnyObjectByType<Player_Controll>().GetComponent<Player_Controll>();
                interactionManager.player = player;
                player.interactionManager = this.interactionManager;
                break;
            }
            case 2:
            {
                levelManager.level = LevelManager.Level.Sub_Tera;
                break;
            }
            case 3:
            {
                levelManager.level = LevelManager.Level.In_Tera;
                break;
            }
        }
    }
    public void NextSceneLoad()
    {
        SceneManager.LoadScene(1);
    }
}

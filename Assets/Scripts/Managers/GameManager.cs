using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   
    [SerializeField]
    private LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = this.gameObject.GetComponent<LevelManager>();
    }
    public void Init()
    {
        
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

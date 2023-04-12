using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Define;
public class Managers : MonoBehaviour
{
    public static Managers managers_instance = null;
    public static Managers Instance{ get { return managers_instance;}}
    GameManager gameManager = new GameManager();
    LevelManager levelManager = new LevelManager();
    UIManager uiManager = new UIManager();
    public static GameManager Game{get { Init(); return Instance?.gameManager;}}
    public static LevelManager level{get { Init(); return Instance?.levelManager;}}
    public static UIManager UI{get { Init(); return Instance?.uiManager;}}
    void Start()
    {
        
    }
    public static void Init()
    {
        GameObject manager = GameObject.Find("GameManagers");
        if(manager == null)
            manager = new GameObject { name = "GameManagers"};
        managers_instance = Extension.GetOrAddComponent<Managers>(manager);
        DontDestroyOnLoad(manager);

        //TODO : 각 데이터 이니셜라이즈
        managers_instance.gameManager.Init();
        managers_instance.levelManager.Init();
    }
}

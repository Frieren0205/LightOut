using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if(!_instance)
            {
                GameObject container = GameObject.FindFirstObjectByType<UIManager>().gameObject;
                _instance = container.GetComponent<UIManager>();
            }
            return _instance;
        }
    }
    [SerializeField]
    private GameObject PauseWindow;

    public GameObject DialogueEventUI;
    public GameObject GameoverWindows;

    [SerializeField]
    private Image gameoverbgimg;
    [SerializeField]
    private Image gameoverfrtimg;

    public Image fadeimg;
    

    // [SerializeField]
    // private GameObject UIClone;
    
    public bool isPause;
    [SerializeField]
    private bool cancallback = true;
    public void OnPause()
    {
        if(!isPause && cancallback)
        {
            PauseWindow.SetActive(true);
            cancallback = false;
            StartCoroutine(callbacktime());
            Time.timeScale = 0;
            isPause = true;
            GameManager.Instance.isPause = true;
        }
        if(isPause && cancallback)
        {
            PauseWindow.SetActive(false);
            cancallback = false;
            StartCoroutine(callbacktime());
            Time.timeScale = 1;
            isPause = false;
            GameManager.Instance.isPause = false;
        }
    }
    public void OnSetting()
    {
        PauseWindow.SetActive(true);
    }
    public void OnGameover()
    {
        GameoverWindows.SetActive(true);
        //TODO : 게임오버 연출 어떻게 할지

    }
    public void toTitleScene()
    {
        StartCoroutine(castfadeout());
        Invoke("toTitleScenewithFade",1);
    }
    public void toGameExit()
    {
        StartCoroutine(castfadeout());
        Invoke("toGameExitwithFade", 1);
    }
    void toGameExitwithFade()
    {
        // 데이터 날리기
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    void toTitleScenewithFade()
    {
        SceneManager.LoadScene(0);
        GameManager.Instance.PlayerObjectDestroy();
        GameoverWindows.SetActive(false);
    }
    private IEnumerator callbacktime()
    {
        yield return new WaitForSecondsRealtime(0.15f);
        cancallback = true;
    }

    public IEnumerator castfadeout()
    {
        fadeimg.gameObject.SetActive(true);
        fadeimg.DOFade(1, 1);
        yield return new WaitForSeconds(1.1f);
        fadeimg.gameObject.SetActive(false);
    }
    public IEnumerator castfadein()
    {
        fadeimg.gameObject.SetActive(true);
        fadeimg.DOFade(0,1);
        yield return new WaitForSeconds(1);
        fadeimg.gameObject.SetActive(false);
    }
}

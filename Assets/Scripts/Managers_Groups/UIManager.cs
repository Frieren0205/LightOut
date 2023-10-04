using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private IEnumerator callbacktime()
    {
        yield return new WaitForSecondsRealtime(0.15f);
        cancallback = true;
    }
    public void OnFadeStart()
    {
        
    }
}

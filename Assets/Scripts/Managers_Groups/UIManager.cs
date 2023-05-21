using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject PauseWindow;

    public GameObject DialogueEventUI;

    // [SerializeField]
    // private GameObject UIClone;
    
    [SerializeField]
    private bool isPause;
    // Start is called before the first frame update
    void Start()
    {
        // PauseWindow = Resources.Load<GameObject>("Prepabs/UI/Pause_Window");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPause()
    {
        if(isPause == false)
        {
            PauseWindow.SetActive(true);
            Debug.Log("정지 실행");
            Time.timeScale = 0;
            isPause = true;
        }
    }
    public void ExitPause()
    {
        if(isPause == true)
        {
            Debug.Log("정지 종료");
            Time.timeScale = 1;
            // Destroy(UIClone);
            isPause = false;
            PauseWindow.SetActive(false);
        }
    }
    public void OnSetting()
    {
    
    }
}

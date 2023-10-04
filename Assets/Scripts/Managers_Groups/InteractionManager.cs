using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Yarn.Unity;
using Unity.VisualScripting;

public class InteractionManager : MonoBehaviour
{


    [SerializeField]
    private UIManager uIManager;
    public GameManager gameManager;
    public InteractionObject interactionData;
    public Player_Controll player;

    public Image EventCharacterIMG;
    public TextMeshProUGUI EventCharacterName;
    // [SerializeField]
    // private string SavedLog;


    // private string[] testlog;
    // // private bool isPlaying = false;
    // public TextMeshProUGUI EventLog;

    public DialogueRunner runner;

    public  LineView lineView => FindFirstObjectByType<LineView>();

    // int count = 0;

    public void PopUpUI()
    {
        if(player.interactionPoint.indexString != string.Empty)
        {
            uIManager.DialogueEventUI.SetActive(true);
            DialogueEvent();
        }
    }

    private void DialogueEvent()
    {   
        gameManager.isPause = true;
        runner.StartDialogue(player.interactionPoint.indexString);
        
    }

    // public void ChangeEventLog(InteractionObject Data)
    // {
    //     PopUpUI();
    //     interactionData = Data;
    //     EventCharacterIMG.sprite = Data.EventCharacterIllust[0];
    //     EventCharacterName.text = Data.TextEventName[0];
    //     testlog = Data.TextEventLog;
    //     if(isPlaying == false) 
    //     {
    //         count = 0;
    //         StartCoroutine(DialogueEvent());
    //     }
    // }
    // IEnumerator DialogueEvent()
    // {
    //     EventLog.text = null;
    //     isPlaying = true;
    //     while(count < testlog.Length)
    //     {
    //         SavedLog = testlog[count];
    //         for(int i = 0; i < SavedLog.Length; i++)
    //         {
    //             EventLog.text = SavedLog.Substring(0, i+1);
    //             yield return new WaitForSecondsRealtime(0.2f);
    //             if(EventLog.text.Length >= SavedLog.Length)
    //             {
    //                 count++;
    //             }
    //         }
    //     }
    // }
}

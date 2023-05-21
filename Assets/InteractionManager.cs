using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    [SerializeField]
    private UIManager uIManager;
    private GameManager gameManager;
    public InteractionObject interactionData;
    public Player_Controll player;

    public Image EventCharacterIMG;
    public TextMeshProUGUI EventCharacterName;
    [SerializeField]
    private string SavedLog;


    private string[] testlog;
    private bool isPlaying = false;
    public TextMeshProUGUI EventLog;
    [SerializeField]
    private bool isOverlength;

    public void PopUpUI()
    {
        uIManager.DialogueEventUI.SetActive(true);
    }

    public void ChangeEventLog(InteractionObject Data)
    {
        PopUpUI();
        EventCharacterIMG.sprite = Data.EventCharacterIllust[0];
        EventCharacterName.text = Data.TextEventName[0];
        testlog = Data.TextEventLog;
        if(isPlaying == false) 
            StartCoroutine(DialogueEvent());
    }
    IEnumerator DialogueEvent()
    {
        EventLog.text = null;
        isPlaying = true;

        for(int t = 0; t < testlog.Length;)
        {
            SavedLog = testlog[t];
            if(!isOverlength)
            for(int i = 0; i < SavedLog.Length; i++)
            {
                EventLog.text = SavedLog.Substring(0, i+1);
                yield return new WaitForSeconds(0.2f);

            }
        }
    }
}

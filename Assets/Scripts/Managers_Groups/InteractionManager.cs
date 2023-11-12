using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Yarn.Unity;
using UnityEngine.SceneManagement;

public class InteractionManager : MonoBehaviour
{


    [SerializeField]
    private UIManager uIManager;
    public GameManager gameManager;
    public InteractionObject interactionData;
    public Player_Controll player;

    public Image EventCharacterIMG;
    public TextMeshProUGUI EventCharacterName;

    public DialogueRunner runner;
    public DialogueAdvanceInput advanceInput;

    public  LineView lineView => FindFirstObjectByType<LineView>();

    // int count = 0;
    public void After_Prologue()
    {
        if(SceneManager.GetActiveScene().buildIndex == 2)
        {
            runner.startNode = "After_Prologue";
            runner.StartDialogue(runner.startNode);
            player.CanInteraction = false;
            gameManager.isPause = true;
            StartCoroutine(advancedInput_set());
        }
    }
    public void BossApear()
    {
        if(SceneManager.GetActiveScene().buildIndex == 4)
        {
            runner.startNode = "if_Player_meet_Boss";
            runner.StartDialogue(runner.startNode);
            player.CanInteraction = false;
            gameManager.isPause = true;
        }
    }
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
        StartCoroutine(advancedInput_set());
    }
    IEnumerator advancedInput_set()
    {
        yield return new WaitForSeconds(0.25f);
        advanceInput.enabled = true;
    }
}

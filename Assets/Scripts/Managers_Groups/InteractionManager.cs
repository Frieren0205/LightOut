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
    public void if_interaction_start()
    {
        GameManager.Instance.isPause = true;
    }
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
    public void if_UnderGround_Clear()
    {
        if(SceneManager.GetActiveScene().buildIndex == 2)
        {
            runner.startNode = "if_Complete_Underground";
            runner.StartDialogue(runner.startNode);
            player.CanInteraction = false;
            gameManager.isPause = true;
            StartCoroutine(advancedInput_set());
        }
    }
    public void if_Enter_Subtera()
    {
        if(SceneManager.GetActiveScene().buildIndex == 3)
        {
            runner.startNode = "if_Enter_SubTera";
            runner.StartDialogue(runner.startNode);
            player.CanInteraction = false;
            gameManager.isPause = true;
            StartCoroutine(advancedInput_set());
        }
    }

    public void if_Player_meet_Security()
    {
        if(SceneManager.GetActiveScene().buildIndex == 3)
        {
            runner.startNode = "if_Player_meet_Security";
            runner.StartDialogue(runner.startNode);
            player.CanInteraction = false;
            gameManager.isPause = true;
            StartCoroutine(advancedInput_set());
        }
    }

    public void if_Enter_In_tera()
    {
        if(SceneManager.GetActiveScene().buildIndex == 4)
        {
            runner.startNode = "if_Player_in_INTERA";
            runner.StartDialogue(runner.startNode);
            player.CanInteraction = false;
            gameManager.isPause = true;
            StartCoroutine(advancedInput_set());
        }
    }

    public void if_Clear_Dialogue(string Clear_logue)
    {
        runner.startNode = Clear_logue;
        runner.StartDialogue(runner.startNode);
        player.CanInteraction = false;
        gameManager.isPause = true;
        StartCoroutine(advancedInput_set());
    }
    public void if_SubTera_Clear()
    {
        if(SceneManager.GetActiveScene().buildIndex == 3 && LevelManager.Instance.isLevel2Clear)
        {
            runner.startNode = "if_Level2_AllClear";
            runner.StartDialogue(runner.startNode);
            player.CanInteraction = false;
            gameManager.isPause = true;
            StartCoroutine(advancedInput_set());
        }
    }
    public void BossApear()
    {
        if(SceneManager.GetActiveScene().buildIndex == 5)
        {
            runner.startNode = "if_Player_meet_Boss";
            runner.StartDialogue(runner.startNode);
            player.CanInteraction = false;
            gameManager.isPause = true;
            StartCoroutine(advancedInput_set());

        }
    }
    public void if_Boss_Clear()
    {
        runner.startNode = "if_Boss_Clear";
        runner.StartDialogue(runner.startNode);
        player.CanInteraction = false;
        gameManager.isPause = true;
        StartCoroutine(advancedInput_set());
    }
    public void if_Generator_Destory()
    {
        runner.startNode = "if_Generator_Destroy";
        runner.StartDialogue(runner.startNode);
        player.CanInteraction = false;
        gameManager.isPause = true;
        StartCoroutine(advancedInput_set());
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

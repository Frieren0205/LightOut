using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Yarn.Unity;

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

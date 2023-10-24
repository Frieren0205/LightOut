using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPoint : MonoBehaviour
{
    public enum Interactiontype
    {
        dialogue,
        portal
    }
    public Interactiontype interactiontype;
    private Player_Controll player;

    [Header("재생시킬 Yarn 파일 이름")]
    public string indexString;
    // 참조
    // interactionManager -> DialogueEvent()
    // Player_Controll -> OnInteraction()

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.GetComponent<Player_Controll>() && interactiontype == Interactiontype.dialogue)
        {
            player = other.gameObject.GetComponent<Player_Controll>();
            player.interactionPoint = this;
            player.CanInteractionIcon.SetActive(true);
            player.CanInteraction = true;
        }
        else if(other.gameObject.GetComponent<Player_Controll>() && interactiontype == Interactiontype.portal)
        {
            player = other.gameObject.GetComponent<Player_Controll>();
            player.interactionPoint =  this;
            player.CanInteractionIcon.SetActive(true);
            player.CanInteraction = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.GetComponent<Player_Controll>())
        {
            player.CanInteraction = false;
            player.interactionPoint = null;
            player.CanInteractionIcon.SetActive(false);
            player = null;
        }
    }
}

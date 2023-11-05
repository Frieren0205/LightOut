using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPoint : MonoBehaviour
{
    public enum Interactiontype
    {
        dialogue,
        portal,
        teleport
    }
    public Interactiontype interactiontype;
    private Player_Controll player;

    [Header("재생시킬 Yarn 파일 이름")]
    public string indexString;
    [Header("가려고 하는 위치 벡터값")]
    public Vector3 transformVec3;
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
        else if(other.gameObject.GetComponent<Player_Controll>() && interactiontype == Interactiontype.teleport)
        {
            //TODO : 같은 씬 안에서의 텔레포트
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

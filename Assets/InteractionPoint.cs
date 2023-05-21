using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPoint : MonoBehaviour
{
    public InteractionObject InteractionData;
    private Player_Controll player;

    public int  Pointindex;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.GetComponent<Player_Controll>())
        {
            player = other.gameObject.GetComponent<Player_Controll>();
            player.interactionPoint = this;
            player.CanInteractionIcon.SetActive(true);
            player.CanInteraction = true;
            Debug.Log(InteractionData);
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.gameObject.GetComponent<Player_Controll>())
        {
            player.CanInteraction = false;
            player.interactionPoint = null;
            player.CanInteractionIcon.SetActive(false);
            player = null;
        }
    }
}

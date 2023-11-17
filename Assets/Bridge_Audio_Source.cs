using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge_Audio_Source : MonoBehaviour
{   
    [SerializeField]
    private bool alreadycall_stay_method = false;
    public AudioSource bridge_audio_source;
    
    [Header("흔들다리 소리 배열")]
    public AudioClip[] bridge_audioclip_list;

    public AudioClip bridge_stay_Audioclip;

    private void OnCollisionEnter(Collision other) 
    {
        if(other.collider.GetComponent<Player_Controll>())
        {
            bridge_audio_source.PlayOneShot(bridge_audioclip_list[Random.Range(0,2)]);
        }    
    }
    private void OnCollisionStay(Collision other) {
        if(other.collider.GetComponent<Player_Controll>())
        {
            Player_Controll player = other.collider.GetComponent<Player_Controll>();
            if(player.isStay == true && alreadycall_stay_method == false)
            {
                alreadycall_stay_method = true;
                bridge_audio_source.Stop();
                bridge_audio_source.PlayOneShot(bridge_stay_Audioclip);
            }
            else if(player.isStay == false && alreadycall_stay_method == true)
            {
                alreadycall_stay_method = false;
            }
        }
    }
}

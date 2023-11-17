using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arm_Hammer_Sound : MonoBehaviour
{
    public AudioSource arm_audio_source;
    public AudioClip portal_appear_audioclip;
    public AudioClip arm_hit_audioclip;
    public AudioClip portal_disappear_audioclip;

    public void Portal_Appear()
    {
        arm_audio_source.PlayOneShot(portal_appear_audioclip);
    }

    public void Play_Arm_Hit()
    {
        arm_audio_source.PlayOneShot(arm_hit_audioclip);
    }

    public void Portal_Disapper()
    {
        arm_audio_source.PlayOneShot(portal_disappear_audioclip);
    }
}

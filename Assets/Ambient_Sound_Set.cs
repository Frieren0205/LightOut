using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ambient_Sound_Set : MonoBehaviour
{
    public AudioSource audioSource;
    private bool aleadycall = false;
    private void Update() 
    {
        if(aleadycall == false)
        StartCoroutine(AudioLoopRoutine());
    }

    private IEnumerator AudioLoopRoutine()
    {
        aleadycall = true;
        audioSource.Play();
        yield return new WaitForSecondsRealtime(4);
        aleadycall = false;
    }
}

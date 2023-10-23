using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Boss_range_attack : MonoBehaviour
{   
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.name != "Tikke_boss")
        {
            Destroy(this.gameObject,1f);
        }
    }
}

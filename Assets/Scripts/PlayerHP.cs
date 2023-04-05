using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PlayerHP : MonoBehaviour
{
    public Slider hpslider;

    public int HP_Point;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        hpslider.value = HP_Point;      
    }
}

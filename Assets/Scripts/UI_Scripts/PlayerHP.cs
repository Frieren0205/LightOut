using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Rendering;

public class PlayerHP : MonoBehaviour
{

    [Range(0,4)]
    public int HP_Point;
    public Sprite[] HP_sprites;
    public Image HPImage;

    public Volume volume;
    public VolumeProfile isNormalVolume;
    public VolumeProfile isDangerousVolume;
    void Update()
    {
        OnValueChanged();
    }
    public void OnValueChanged()
    {
        HPImage.sprite = HP_sprites[HP_Point];
        if(HP_Point <= 1)
        {
            volume.profile = isDangerousVolume;
        }
        else
            volume.profile = isNormalVolume;
    }
}

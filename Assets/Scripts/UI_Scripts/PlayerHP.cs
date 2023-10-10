using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class PlayerHP : MonoBehaviour
{
    [Range(0,4)]
    public int HP_Point;
    [Range(0,2)]
    public int shieldpoint;
    public Sprite[] HP_sprites;
    public Image HPImage;

    public Volume volume;
    public VolumeProfile isNormalVolume;
    public VolumeProfile isDangerousVolume;


    // public int HP
    // {
    //     get{return this.HP_Point;}
    //     set{this.HP = value;}
    // }

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

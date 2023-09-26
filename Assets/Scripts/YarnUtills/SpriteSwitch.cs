using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using DG.Tweening;


[RequireComponent (typeof (Image))]
public class SpriteSwitch : MonoBehaviour
{
    [System.Serializable]
    public struct SpriteInfo
    {
        public string spritename;
        public Sprite sprite;
    }

    public SpriteInfo[] sprites;
    [YarnCommand("SpriteSwitch")]
    public void UseSprite(string spriteName)
    {
        Sprite s = null;
        foreach(var info in sprites)
        {
            if(info.spritename == spriteName)
            {
                s = info.sprite;
                break;
            }
        }
        if(s == null)
        {
            Debug.LogErrorFormat("스프라이트를 찾을 수 없습니다 {0}", spriteName);
            return;
        }

        GetComponent<Image>().sprite = s;
    }
    [YarnCommand("Fade_in")]
    public void FadeinSprite(float time)
    {
        Image img = GetComponent<Image>();
        Debug.Log(img);
        img.DOFade(0, time);
    }

    [YarnCommand("Fade_out")]
    public void FadeOutSprite(float time)
    {
        Image img = GetComponent<Image>();
        Debug.Log(img);
        img.DOFade(1, time);
    }
}

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
                GetComponent<Image>().color = new Color(1,1,1);
                break;
            }
        }
        if(s == null)
        {
            Debug.LogErrorFormat("스프라이트를 찾을 수 없거나, 비어있습니다. 의도하신건가요? {0}", spriteName);
            GetComponent<Image>().sprite = null;
            GetComponent<Image>().color = new Color(1,1,1,0);
            return;
        }

        GetComponent<Image>().sprite = s;
    }
    [YarnCommand("Fade_in")]
    public void FadeinSprite(float time)
    {
        
        Image img = GetComponent<Image>();
        img.DOFade(0, time);
    }

    [YarnCommand("Fade_out")]
    public void FadeOutSprite(float time)
    {
        Image img = GetComponent<Image>();
        img.DOFade(1, time);
    }
}

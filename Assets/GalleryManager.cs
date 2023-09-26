using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{
    static GalleryManager galleryManager_instance = null;
    public static GalleryManager Instance
    {
        get
        {
            if(galleryManager_instance == null)
            {
                return null;
            }
            return galleryManager_instance;
        }
    }

    public Image for_change_img;
    public void Ifclickitembutton(GameObject gameObject)
    {
        Image image = gameObject.GetComponent<Image>();
        for_change_img.sprite = image.sprite;
        for_change_img.color = image.color;
    }
}
